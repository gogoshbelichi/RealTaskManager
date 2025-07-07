using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealTaskManager.Core.Entities;

namespace RealTaskManager.Infrastructure.Data;

public class CustomIdentityDbContext(DbContextOptions<CustomIdentityDbContext> options)
    : IdentityDbContext<TaskManagerUser>(options);