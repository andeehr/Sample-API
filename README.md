# API Example

This is a simple example of an API that aims to integrate multiple behaviors in a straightforward and concise manner.

The API emulates a registration and login system for an application that has minimal functionality. This application stores users along with their respective roles and permissions. For simplicity, the permissions are predefined (via script), and the roles are sent at the time of registration.

## Technical Details and Considerations

The application is built on .NET Core 8 with a simple layered architecture and a small database in SQL Server.

### Register

```json
{
  "username": "john.berry",
  "password": "John.Berry.123!",
  "firstName": "John",
  "lastName": "Berry",
  "roleId": 1
}
```

The registration endpoint validates some required fields using `FluentValidation`, hashes the password with `BCrypt.Net-Next`, and stores it in the database.
> **Note**: All endpoints receive and send DTOs. The layer responsible for mapping DTOs to Entities and vice versa is the Core layer.

### Login
```json
{
  "username": "john.berry",
  "password": "John.Berry.123!"
}
```

The login endpoint delegates authentication to an intermediate service called `AuthManager`. This service calls the `UserService` to retrieve user data and validate the credentials, generating a JWT token and responding with the token in the header.
For token creation, a wrapper has been implemented to handle reading and writing the JWT token.

### Get Users
**Endpoint:** `http://baseUrl/v1/user?{filters}`
This endpoint is secured with the `Authorize` annotation, which is a custom and configurable filter for its use. The properties for filtering are received via query parameters. If no parameters are passed, the search will return all records.

To model this, an abstract class `Filter` is used, which contains global pagination and sorting attributes common to any search endpoint. Since the response is a DTO that may have attributes different from the properties of the entities (an example is provided in the code), both database filtering and an in-memory alternative have been implemented if needed. The response is a `PagedResult` object that contains the requested information, the actual number of rows, and the number displayed. This is because there is a fixed limit to prevent overloading the query when handling large volumes of data.

### JWT Middleware and Filter Usage
A middleware has been implemented to verify the authentication of a user. This middleware checks the validity of the token and maps the user information obtained from the token to a global variable within the context. Together with the filter, this helps determine whether the user is authenticated and has permission to access a particular endpoint, using the Authorize annotation along with the corresponding permission.
