# TaskManager

**GraphQL task manager with JWT authentication and authorization via ASP.NET Core 9.x, Entity Framework Core 9.x, HotChocolate v15**

---

## Table of Contents
- [Task](#task)
- [Requirements](#requirements)
- [Results](#results)
- [Data Models](#data-models)
- [GraphQL API](#graphql-api)
- [Authentication & Authorization](#authentication--authorization)
- [UserType](#usertype)
- [TaskType](#tasktype)
- [Queries](#queries)
- [Mutations](#mutations)
- [DataLoaders & Node](#dataloaders--node)
- [Examples of Queries and Mutations](#some-examples-of-queries-and-mutations)

---

## Task
**Develop GraphQL API for task management with support for authentication and role-based authorization.**

### Requirements
**Implement GraphQL API using HotChocolate**

Implement the following types:

- **Task** (Id, Title, Description, Status, CreatedAt, CreatedBy) ✅
- **User** (Id, Username, Email, Role) ✅

**Query Support (Queries):**

- Getting a list of tasks (filtered by status and user) ✅
- Getting a task by ID ✅
- Getting a list of users (for the administrator only) ✅
- *All queries must support pagination and sorting.* ✅

**Mutations Support:**

- Creating / updating / deleting an issue (only for authorized users) ✅
- Implement authentication by obtaining a JWT token. ✅
- Enter two roles. ✅
  - *User*: Can create and edit their own tasks. ✅
  - *Administrator*: Can create, edit, and delete any tasks, and can also assign a task to a user. ✅
- To store data, use **PostgreSQL**. ✅

---

## Results
I've made several versions and sandboxes during GraphQL and Hot Chocolate research. One with *IdentityServer*, one with *Firebase Auth*. And this customization is final.

I explored the capabilities of building a **GraphQL API** with *Hot Chocolate*, focusing on **performance** and **extensibility**.

### Data Models
I implemented the following entities:

- **TaskEntity** and **UserEntity**, related via *CreatedBy* (one-to-many) and *TasksAssignedToUser* (many-to-many).
- For tracking task statuses, I used **TaskStatusEnum**, stored in the database as a string via `HasConversion`.

### GraphQL API
I developed **queries** and **mutations** for managing tasks and users.

- Implemented *field-level authorization* using policies and roles.
- To solve the **N+1 problem**, I used *DataLoader* and *batching*.
- For lists, I added custom *pagination*, *sorting*, and *filtering* mechanisms using:

```csharp
.AddDbContextCursorPagingProvider()
.AddCursorKeySerializer(new EnumCursorKeySerializer<TaskStatusEnum>())
```

This allowed proper handling of cursors and enum fields.

### Authentication & Authorization
I implemented several approaches:

- **Custom service** — JWT with endpoints.
- **IdentityServer** — moved auth logic into a separate microservice for scalability and flexibility. [With IdentityServer](https://github.com/gogoshbelichi/UserTasks.GraphQL.IdentityServer)
- **Firebase** — experimented with authentication in a monolithic architecture. [With Firebase](https://github.com/gogoshbelichi/UserTasks.GraphQL.Firebase)

### UserType
- Implements *Node*.
- **tasksCreated** — tasks created by the user, resolved with *DataLoader* and *pagination*.
- **tasksAssigned** — tasks assigned to the user, resolved via *DataLoader* with *QueryContext*.

### TaskType
- Implements *Node*.
- **createdBy** — linked to *UserEntity*.
- **assignedTo** — users assigned to a task, resolved with *DataLoader* and *pagination*.

### Queries
**For Users (UserEntity):**

- **GetUsersAsync** — paginated user list with filtering and sorting (*UserFilterInputType*, *UserSorting*).
- **GetUserByIdAsync** — fetch by ID using *NodeResolver* and *DataLoader* (fixing N+1).
- **GetUsersByIdAsync** — batch fetch users by IDs with pagination and *DataLoader*.
- **GetMe** — get the current user by JWT token, including created and assigned tasks.

**For Tasks (TaskEntity):**

- **GetTasksAsync** — paginated list of tasks with filtering and sorting (*TaskFilterInputType*, *TaskSorting*).
- **GetTaskByIdAsync** — fetch a task by ID via *NodeResolver* and *DataLoader*.
- **GetTasksByIdAsync** — batch fetch tasks by IDs.

These queries demonstrate the combined use of *UsePaging* + *UseFiltering* + *UseSorting* with *QueryContext* and custom ordering, along with practical application of *global object identification* (*NodeResolver*) and *field-level authorization* (*AdminPolicy*, *UserPolicy*).

### Mutations
- **AddTaskAsync** — create a new task. Uses DB transaction, with error handling (e.g., *TaskNotCreatedError*).
- **UpdateTaskDetailsAsync** — update task title, description, or status. Authorization: only task creator or admin can modify.
- **UpdateTaskAssignment** — assign or unassign users to/from a task. Works with transactions, returns errors (e.g., *UsersNotAssignedError*).
- **DeleteTaskAsync** — delete a task. Permission check: admin or task creator.
- **TakeTaskAsync** — user can "take ownership" of a task. Includes a check that the task is not already assigned.

Mutations are implemented via *FieldResult<TSuccess, TError...>*, enabling strongly-typed error handling (e.g., *TaskNotFoundError*, *PermissionException*) and predictable contracts for clients.

### DataLoaders & Node
**TaskDataLoaders:**

- **TaskByIdAsync** — fetch tasks by ID.
- **UsersAssignedToTasksAsync** — batch users assigned to tasks, with pagination.

**UserDataLoaders:**

- **UserByIdAsync**, **PagedUsersByIdAsync** — fetch users by ID, with batching and pagination.
- **TasksCreatedByUserAsync** — tasks created by a user (batching + pagination).
- **TasksAssignedToUsersAsync** — tasks assigned to a user.

*DataLoaders* were implemented via *GreenDonut*, supporting cursor-paginated batching, which enables efficient loading of nested collections.

## Some examples of queries and mutations
```graphql
# Получаем список пользователей, у которых есть назначенные задачи.
# Для каждого пользователя возвращаем: id, роли, 
# первую созданную задачу и первую назначенную задачу.
query Test1 {
  users(first: 10, where: { tasksAssignedToUser: { any: true } }) {
    edges {
      node {
        id
        roles
        tasksCreatedlol(first: 1) {
          edges {
            node {
              title
            }
          }
        }
        tasksAssigned(first: 1) {
          edges {
            node {
              task {
                title
              }
            }
          }
        }
      }
    }
    totalCount
  }
}

# Получаем пользователей по ID (batch-запрос с пагинацией).
# Для каждого юзера выводим username, первые 2 созданные задачи и список назначенных.
query Test4 {
  usersById(
    ids: [
      "VXNlckVudGl0eToSOJgB3dTUcZ6OOH/ygy4F"
      "VXNlckVudGl0eToSOJgBAdaKdIUUhqKVwnRb"
      "VXNlckVudGl0eToSOJgBYtZsdqhndxnbDsQ2"
      "VXNlckVudGl0eToSOJgBH9dAdrmyuIZYF+p5"
    ]
    first: 2
    after: "e30wMTk4MzgxMi1kNjAxLTc0OGEtODUxNC04NmEyOTVjMjc0NWI="
  ) {
    edges {
      cursor
      node {
        username
        tasksCreatedlol(first: 2) {
          edges {
            node {
              title
              id
            }
          }
        }
        tasksAssigned {
          edges {
            node {
              task {
                title
              }
            }
          }
        }
      }
    }
    pageInfo {
      hasNextPage
      endCursor
    }
  }
}

# Получаем задачи без исполнителей (any: false).
# Возвращаем ключевые поля, включая автора и его роли.
query Test6 {
  tasks(
    first: 3
    after: "e31Wb2x1cHRhdGVtIHF1aWJ1c2RhbSBwZXJzcGljaWF0aXMgZXQgcmVydW0u"
    where: { and: [{ tasksAssignedToUser: { any: false } }] }
  ) {
    edges {
      node {
        id
        title
        description
        status
        createdAt
        createdBy {
          username
          roles
        }
        createdByUserId
        assignedTo(first: 1) {
          edges {
            node {
              user {
                username
              }
            }
          }
        }
      }
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
      startCursor
      endCursor
    }
  }
}

# Получаем задачи с назначенными пользователями, отсортированные по статусу (DESC).
query Test7 {
  tasks(
    order: [{ status: DESC }]
    where: { tasksAssignedToUser: { any: true } }
    first: 25
  ) {
    edges {
      node {
        id
        title
        description
        status
        createdAt
        createdBy {
          username
          roles
        }
        createdByUserId
        assignedTo(first: 1) {
          edges {
            node {
              user {
                username
              }
              userId
            }
          }
        }
      }
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
      startCursor
      endCursor
    }
  }
}

# Получаем задачи, где хотя бы один исполнитель завершил её (DONE).
# Возвращаем детальную инфу о назначенных пользователях.
query Test8 {
  tasks(
    order: [{ description: ASC }]
    where: { tasksAssignedToUser: { some: { task: { status: { eq: DONE } } } } }
    first: 3
  ) {
    edges {
      node {
        id
        title
        description
        status
        createdAt
        createdBy {
          username
          roles
        }
        createdByUserId
        assignedTo(first: 2) {
          edges {
            node {
              user {
                username
                email
              }
              userId
            }
          }
        }
      }
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
      startCursor
      endCursor
    }
    totalCount
  }
}

# Получаем первых 5 пользователей, у которых есть задачи в статусе DONE.
# Используем сортировку по username и пагинацию.
query Test2 {
  users(
    order: [{ username: ASC }]
    where: { tasksAssignedToUser: { some: { task: { status: { eq: DONE } } } } }
    first: 5
    after: "e31EZW9uZHJlMjNAaG90bWFpbC5jb20="
  ) {
    edges {
      node {
        id
        username
        email
        tasksCreatedlol(first: 1) {
          edges {
            node {
              title
              status
            }
          }
        }
        tasksAssigned(first: 3) {
          edges {
            node {
              task {
                title
                status
              }
              taskId
            }
          }
        }
      }
    }
    pageInfo {
      endCursor
      hasNextPage
      hasPreviousPage
    }
  }
}

# Получаем одного пользователя по ID + его назначенные задачи.
query Test9 {
  userById(id: "VXNlckVudGl0eToSOJgB+vTEf5oWDtgAs4Ce") {
    username
    tasksAssigned (first: 2) {
      edges {
        node {
          task {
            title
            description
            status
          }
        }
        cursor
      }
    }
  }
}

# Создание новой задачи (без исполнителей).
# Возвращает созданный таск и возможные ошибки.
mutation CreateTask {
  addTask(input: { title: "Tesr1ts", description: "Teeesr1ts" }) {
    taskEntity {
      id
      status
      title
      assignedTo {
        edges {
          node {
            user {
              username
            }
          }
        }
      }
    }
    errors {
      __typename
    }
  }
}

# Удаление задачи по ID.
# Если задача не найдена — вернется TaskNotFoundError.
mutation DeleteTask {
  deleteTask(input: { id: "VGFza0VudGl0eTpDpZgBChTufphYlaXkX1OC" }) {
    resultTaskPayload {
      result
    }
    errors {
      ... on TaskNotFoundError {
        message
      }
      ... on UserNotFoundError {
        message
      }
    }
  }
}

# Получение задачи по ID с деталями.
query TaskById {
  taskById(id: "VGFza0VudGl0eTrloZgBm8orfbudzCkDwKfP") {
    id
    title
    status
    description
    createdBy {
      username
      id
    }
    assignedTo {
      edges {
        node {
          user {
            email
            username
            id
          }
        }
      }
    }
  }
}

# Обновление деталей задачи: статус и заголовок.
mutation UpdateTaskDetails {
  updateTaskDetails(
    detailsInput: {
      taskId: "VGFza0VudGl0eTo0oZgBiVLWfIMPQVc3hVTv"
      status: TODO
      title: "Test14"
    }
  ) {
    taskEntity {
      title
      description
      status
    }
    errors {
      ... on PermissionError {
        message
      }
      ... on TaskNotFoundError {
        message
      }
      ... on UserNotFoundError {
        message
      }
    }
  }
}

# Взятие задачи пользователем (self-assign).
# Ошибки: задача уже назначена, не найдена, или пользователь не найден.
mutation TakeTask {
  takeTask(input: { id: "VGFza0VudGl0eTqjn5gBGkficLczxvmERTtY" }) {
    taskEntity {
      id
      title
      status
      description
      createdByUserId
      createdAt
      assignedTo {
        edges {
          node {
            user {
              username
              id
            }
          }
        }
      }
    }
    errors {
      ... on TaskAlreadyAssignedError {
        message
      }
      ... on TaskNotFoundError {
        message
      }
      ... on UserNotFoundError {
        message
      }
    }
  }
}

# Массовое обновление назначений задачи.
# Можно добавить новых исполнителей и убрать старых.
mutation UpdateTaskAssignment {
  updateTaskAssignment(
    input: {
      taskId: "VGFza0VudGl0eTrloZgBm8orfbudzCkDwKfP"
      assignByUserIds: [
        "VXNlckVudGl0eToSOJgBF+Hwc7dvHAZw7jT/"
        "VXNlckVudGl0eToSOJgB3d0pebMUjndmjSxQ"
        "VXNlckVudGl0eToSOJgBrPirfoWjCp8SsPWx"
      ]
      userIdsToRemove: [
        "VXNlckVudGl0eToSOJgBSvgmeZn87ON6rnYU"
        "VXNlckVudGl0eToSOJgBlesmcawkrB4B+TzD"
        "VXNlckVudGl0eToSOJgBkOMwc6gzGD/U2GvT"
      ]
    }
  ) {
    taskEntity {
      title
      assignedTo {
        edges {
          node {
            user {
              username
              email
            }
            userId
          }
        }
      }
      createdBy {
        username
        email
      }
    }
    errors {
      ... on TaskNotFoundError {
        message
      }
      ... on PermissionError {
        message
      }
      ... on UsersNotAssignedError {
        message
      }
      __typename
    }
  }
}

# Получение всех задач, созданных пользователем с username = "Mertie93".
query Test10 {
  tasks(where: { createdBy: { username: { eq: "Mertie93" } } }) {
    edges {
      node {
        title
        id
        assignedTo {
          edges {
            node {
              user {
                username
              }
            }
          }
        }
      }
    }
  }
}

# Запрос текущего пользователя (me).
# Возвращает последние назначенные и первые созданные задачи.
query Me {
  me {
    id
    username
    email
    roles
    tasksAssigned(last: 2) {
      edges {
        node {
          task {
            title
            status
          }
        }
      }
    }
    tasksCreatedlol(first: 3) {
      edges {
        node {
          title
          status
          createdAt
        }
      }
    }
  }
}
```


