
# API Example

This is a simple example of an API that aims to integrate multiple behaviors in a straightforward and concise manner.

The API emulates a registration and login system for an application that has minimal functionality. This application stores users along with their respective roles and permissions. For simplicity, the permissions are predefined (via script), and the roles are sent at the time of registration.

---

## 🛠️ Technical Details and Considerations

The application is built on **.NET Core 8** with a simple layered architecture and a small database in **SQL Server**.

### 📄 Register

Here is an example request body for the **Register** endpoint:

```json
{
  "username": "john.berry",
  "password": "John.Berry.123!",
  "firstName": "John",
  "lastName": "Berry",
  "roleId": 1
}
```

- The registration endpoint validates some required fields using **FluentValidation**.
- The password is hashed with **BCrypt.Net-Next** before being stored in the database.
> **Note:** All endpoints send and receive DTOs. The Core layer is responsible for mapping DTOs to Entities and vice versa.

---

### 🔑 Login

An example login request body:

```json
{
  "username": "john.berry",
  "password": "John.Berry.123!"
}
```

The login endpoint delegates authentication to the **AuthManager** helper service. This service:

- Retrieves data from the already validated user from the user service.
- Generates a **JWT token** and includes it in the response header.

For token handling, a wrapper is implemented to manage **JWT token** creation and validation.

---

### 🧑‍💻 Get Users

#### **Endpoint:** `GET http://baseUrl/v1/user?{filters}`

This endpoint is secured with the **Authorize** annotation (a custom, configurable filter). You can filter the users by passing query parameters. If no parameters are provided, all users are returned.

#### Example Query Parameters:
- `pageNumber`: The page number to retrieve.
- `pageSize`: The number of records per page.
- `sortingProperty`: The field to sort by.
- `sortingType`: Ascending | Descending. Default is ascending.

Example request: 
```
GET http://baseUrl/v1/user?pageNumber=1&pageSize=10&sortingProperty=firstName
```

- The abstract class `Filter` is used to handle common attributes like pagination and sorting.
- **In-memory filtering** is also implemented as an alternative when necessary.

#### Response Structure:
```json
{
  "realRows": 100,
  "limitRows": 10,
  "data": [
    {
      "username": "john.berry",
      "firstName": "John",
      "lastName": "Berry",
      "role": "SuperUser",
      "permissions": [
        "user.list",
        "user.manage"
      ]
    }
  ]
}
```

The response includes:
- `PagedResult`: Contains the requested data, the total number of rows, and the number of rows shown.

---

### 🔒 JWT Middleware and Filter Usage

A custom **middleware** verifies user authentication by checking the JWT token. The token data is then mapped to a global variable in the context.

- The **Authorize** annotation helps determine if the user is authenticated and has the necessary permissions to access specific endpoints.
- The middleware and the filter ensures that the user’s token is valid and that their roles and permissions are correctly mapped before accessing protected resources.
