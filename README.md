# TaskManager
GraphQL task manager with JWT authentication and authorization via ASP.NET Core 9.x, Entity Framework Core 9.x, HotChocolate v15

## Task:
## Develop GraphQL API for task management with support for authentication and role-based authorization.

### Requirements:
### Implement GraphQL API using HotChocolate
Implement the following types:

Task (Id, Title, Description, Status, CreatedAt, CreatedBy) ✅

User (Id, Username, Email, Role) ✅

Query Support (Queries):

Getting a list of tasks (filtered by status and user) ✅

Getting an issue by ID ✅

Getting a list of users (for the administrator only) ✅

All queries must support pagination and sorting. ✅

Mutations Support:

Creating / updating / deleting an issue (only for authorized users) ✅

Implement authentication by obtaining a JWT token. ✅

Enter two roles. ✅

The user can create and edit their own tasks.✅

Administrator – can create, edit, and delete any tasks, and can also assign a task to a user. ✅

To store data, use PostgreSQL ✅

## Results: 
I've made several versions and sandboxes during GraphQL and Hot Chocolate research. One with IdentityServer, one with Firebase Auth. And this customization is final.

## Some examples of queries and mutations
```graphql
uery Test1 {
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

query Test5 {
  users(where: { tasksAssignedToUser: { any: true } }, first: 19) {
    edges {
      node {
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
        tasksAssigned(first: 1) {
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
  }
}

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

mutation UpdateTaskAssignment {
  updateTaskAssignment(
    input: {
      taskId: "VGFza0VudGl0eTrloZgBm8orfbudzCkDwKfP"
      assignByUserIds: [
        "VXNlckVudGl0eToSOJgBF+Hwc7dvHAZw7jT/"
        "VXNlckVudGl0eToSOJgB3d0pebMUjndmjSxQ"
        "VXNlckVudGl0eToSOJgBrPirfoWjCp8SsPWx"
        "VXNlckVudGl0eToSOJgBrPirfoWjCp8SsPWx"
      ]
      userIdsToRemove: [
        "VXNlckVudGl0eToSOJgBSvgmeZn87ON6rnYU"
        "VXNlckVudGl0eToSOJgBlesmcawkrB4B+TzD"
        "VXNlckVudGl0eToSOJgBkOMwc6gzGD/U2GvT"
        "VXNlckVudGl0eToSOJgBNNOUer08rr286L6i"
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
