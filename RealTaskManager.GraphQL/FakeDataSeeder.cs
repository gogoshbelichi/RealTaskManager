using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;
using RealTaskManager.GraphQL.AuthEndpoints;
using RealTaskManager.Infrastructure.Data;

namespace RealTaskManager.GraphQL;

public static class FakeDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var userManager = scopedProvider.GetRequiredService<UserManager<TaskManagerUser>>();
        var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userService = scopedProvider.GetRequiredService<CustomUserService>();
        var db = scopedProvider.GetRequiredService<RealTaskManagerDbContext>();
        var faker = new Bogus.Faker();
        
        if (db.Users.Any()) return;
        
        // Ensure roles exist
        var roles = new[] { "Administrator", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Create users and assign roles
        for (int i = 0; i < 100; i++)
        {
            var email = faker.Internet.Email();
            var username = faker.Internet.UserName();
            CustomRegisterRequest request = new()
            {
                Email = email,
                Username = username,
                Password = "Qwerty_123!"
            };
            
            await userService.CreateUser(request);

            if (i >= 4) continue;
            var role = "Administrator";
            AssignRoleRequest roleRequest = new AssignRoleRequest(role, email);
            var taskManagerUser = await userManager.FindByEmailAsync(request.Email);
            if (taskManagerUser is null) throw new Exception("User not found");
            await userService.AssignRole(roleRequest, taskManagerUser);
        }

        // Get EF users for foreign keys
        var users = await db.UserProfiles.ToListAsync();

        var tasks = new List<TaskEntity>();
        var rnd = new Random();

        foreach (var user in users)
        {
            for (int i = 0; i < 15; i++)
            {
                var task = new TaskEntity
                {
                    Id = Guid.NewGuid(),
                    Title = faker.Lorem.Sentence(5),
                    Description = faker.Lorem.Paragraph(),
                    Status = (TaskStatusEnum)rnd.Next(Enum.GetValues<TaskStatusEnum>().Length),
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedByUserId = user.Id,
                    CreatedBy = user
                };

                tasks.Add(task);
            }
        }

        await db.Tasks.AddRangeAsync(tasks);
        await db.SaveChangesAsync();

        // Assign admin tasks to random users
        var adminEfUsers = users.Where(u => u.Roles.Contains("Administrator")).ToList();
        
        var normalUsers = users.Except(adminEfUsers).ToList();

        var allTasks = await db.Tasks.ToListAsync();
        var assignPairs = new List<TasksAssignedToUser>();

        foreach (var task in allTasks.Where(t => adminEfUsers.Any(u => u.Id == t.CreatedByUserId)))
        {
            if (rnd.NextDouble() < 0.7) // 70% chance to assign
            {
                var assignedUser = normalUsers[rnd.Next(normalUsers.Count)];
                assignPairs.Add(new TasksAssignedToUser
                {
                    TaskId = task.Id,
                    UserId = assignedUser.Id,
                    LastAssignedAt = DateTimeOffset.UtcNow
                });
            }
        }

        await db.TasksAssignedToUsers.AddRangeAsync(assignPairs);
        await db.SaveChangesAsync();
    }
}