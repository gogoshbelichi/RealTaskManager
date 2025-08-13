# TaskManager
GraphQL task manager with JWT authentication and authorization via ASP.NET Core 9.x, Entity Framework Core 9.x, HotChocolate v15

Task:
Develop GraphQL API for task management with support for authentication and role-based authorization.
Requirements:
Implement GraphQL API using HotChocolate:
Implement the following types:
Task (Id, Title, Description, Status, CreatedAt, CreatedBy) ✅
User (Id, Username, Email, Role) ✅
Query Support (Queries):
Getting a list of tasks (filtered by status and user) 
Getting an issue by ID
Getting a list of users (for the administrator only)
All queries must support pagination and sorting.
Mutations Support:
Creating / updating / deleting an issue (only for authorized users) ✅
Implement authentication by obtaining a JWT token.
, Enter two roles. 
The user can create and edit their own tasks.
Administrator – can create, edit, and delete any tasks, and can also assign a task to a user.

To store data, use PostgreSQL
