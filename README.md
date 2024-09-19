# Overview
Store-API is a REST API for an online store that serves as the backend for the frontend application, store-front. The API handles operations related to data management, business logic, authentication, authorization, and communication with the database. It acts as the central component of the system, forming a comprehensive e-commerce solution together with the store-front application. It allows guests to make purchases using a temporary ID that matches their temporary cart ID, and logged-in users to make purchases using JWT tokens. By clearly separating the frontend and backend, the API enhances scalability, security, and maintainability, enabling independent development and updates of both parts of the system.

# Tech Stack
## [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-6.0) ([.NET 6](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-6) + [C# 10](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10))
Used as the main framework for building the API (handling HTTP requests, managing routing, implementing middleware for request processing, and facilitating authentication and authorization processes).
### [Entity Framework Core](https://learn.microsoft.com/en-us/ef/)
Used as the Object-Relational Mapper (ORM) to facilitate data access and management within the application. This technology simplifies interactions with the SQL Server database and handles database migrations, data modeling, and CRUD operations.
### [SQL Server](https://www.microsoft.com/pl-pl/sql-server)
Used as the primary relational database management system for storing and managing the application's data.
### [Azure Key Vault](https://azure.microsoft.com/en-us/products/key-vault) + [Azure Identity](https://azure.microsoft.com/en-us/products/category/identity)
Used for securely managing secrets, keys, and certificates, and enabling secure access through Azure Active Directory, without hardcoding credentials in the application.
### [AutoMapper](https://automapper.org/)
Used for automating the mapping between different object models, such as mapping data transfer objects (DTOs)
### [FluentValidation](https://docs.fluentvalidation.net/en/latest/)
Used for validating data inputs through a fluent API, ensuring that incoming data meets defined rules.
### [Swagger](https://swagger.io/)
Used for generating interactive API documentation that imitates the frontend of the site. 
### [SignalR](https://dotnet.microsoft.com/en-us/apps/aspnet/signalr)
Used for enabling real-time communication between the server and clients to get response from API about stripe payment status.
### [SendGrid](https://sendgrid.com/en-us)
Used for sending emails within the application (register link, order summary etc.)
### [Stripe](https://stripe.com/en-pl/payments?gad_source=1&gclid=Cj0KCQjwrp-3BhDgARIsAEWJ6SzEJY3t2l_U8Hdfr-6kuBQf6xmxxmZKtARm5GSB6Zrquclw8qkIKSYaAgS1EALw_wcB)
Used for processing payments and handling transactions.
### [JwtBearer](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer?view=aspnetcore-8.0)
Used for implementing JSON Web Token (JWT) authentication, which is used in the process of user authorization.

# Architecture
### Clean Architecture
The project utilizes the Clean Architecture design pattern, which divides the project into Presentation, Application, Domain, and Infrastructure layers, thereby separating business logic from infrastructure. This architecture adheres to SOLID principles, improving the structure of the code and reducing its complexity. By implementing this pattern, the code has become scalable and easily extendable with new functionalities. <br></br>
![image](https://github.com/user-attachments/assets/0f8ace32-f8bc-45fe-9401-2156c135f0cb)

### Domain-driven design (DDD)
Implementing DDD in the project allowed us to focus on business logic by encapsulating it within entity classes, where each entity has its own methods that perform data operations according to business rules. This separation ensures that the business logic is distinct from the rest of the system, making it easier to manage and maintain.

### Layers
By implementing a layered architecture in accordance with Clean Architecture, including the use of interfaces, we have the ability to swap out individual layers of the application. The layers are not dependent on specific implementations but rely on abstractions provided by interfaces. This allows us, for example, to replace the presentation layer with Blazor components or another frontend technology, or to change the infrastructure layer to switch to MongoDB, or simply to replace specific repository implementations. <br></br> 
![image](https://github.com/user-attachments/assets/1b4bbc6c-8a67-4f56-aeae-925f7b66ca1a)

  
  - **Presentation Layer (`Store.Api`)** - This layer is responsible for handling requests from front-end application. We can find here controllers, extension functions used for clear presentation of API responses, application startup configuration (Program.cs) and validation for inputs and form data submitted in requests. <br></br>
 ![image](https://github.com/user-attachments/assets/923f59c2-1af8-4ade-a8d9-f438def7ee29)

  - **Application Layer (`Store.Application`)** - The application layer manages application logic and implements use cases that define how users can interact with the system. It acts as a mediator between the presentation and domain layers. We can find here specific implementations of application services and contracts, such as the KeyVaultOptions class, which defines the data that should be provided from Azure Key Vault without specifying exactly how these data should be retrieved. This layer also contains interfaces for infrastructure services, which describe how the application layer wants to communicate with the infrastructure layer. In other words, they define how it intends to use services that interact with external providers, which, according to Clean Architecture principles, belong to the infrastructure layer. In this layer, business validation also takes place, for example, to handle conflicts or missing objects, by calling functions defined in repository interfaces that are located in Domain layer, with their specific implementations in the infrastructure layer. <br></br>
  ![image](https://github.com/user-attachments/assets/c1d4f0c5-5e7a-4807-95e0-33a5aba5c5fe)

  - **Domain Layer (`Store.Domain`)** - The domain layer is the core of the application, containing business logic and domain rules. We can find here entities along with functions that are closely associated with them and implement business logic. These entities are mapped to database tables. The layer also includes repository interfaces and objects used for returning operation results using the result pattern, such as the `Result` class, which wraps an Error object providing additional information about the operation's outcome. The `Error` class represents an error by providing details about its code, error specifics, and type (`Failure`, `Validation`, `NotFound`, `Conflict`, etc.). Additionally, there are `Errors` classes that provide static methods mapped to the Error object, which are used to create specific errors with the previously mentioned fields. <br></br>
![image](https://github.com/user-attachments/assets/b983aa8e-35f1-4d2d-9aca-8ba9512a7b27)

  - **Infrastructure Layer (`Store.Infrastructure`)** - The Infrastructure Layer is responsible for implementing technical details and accessing external resources and systems that support the application's operation but are not part of its business logic. In this layer, we can find configurations including scripts for initializing the database with seed data, classes for configuring JWT settings and `Azure Key Vault`, which allow for secure management of keys, credentials, and application secrets. It also contains middlewares that handle global error management and token refreshing, migrations, repositories, the database context, and services that implement application logic not related to business logic. <br></br>
![image](https://github.com/user-attachments/assets/a1b7967a-9c98-4257-9d49-4b3ba8897bb6)

### Design patterns
 - **Dependency Injection (DI)** - Used to manage dependencies and promote loose coupling between components by injecting required services at runtime.
 - **Repository Pattern** - Used to abstract data access logic and provide a cleaner way to interact with data sources through defined interfaces.
 - **Result Pattern** - Used to standardize the handling of operation outcomes, encapsulating both success and failure states with relevant information.
 - **Enum-based Strategy Pattern** -  Used in the ResultExtensions class to determine HTTP status codes, titles, and types based on the ErrorType enumeration, allowing for streamlined and consistent error response handling across the application.
 - **Middleware Pattern** - Applied in the request pipeline of ASP.NET Core to handle cross-cutting concerns such as authentication, logging, and error handling
 - **Options Pattern** - Used to manage configuration settings in a structured and type-safe manner via strongly typed classes.
 - **Factory Pattern** -  Static methods in UserResult, like Success() and Failure(Error error), act as factories to create instances of UserResult in a controlled way.
 - **Singleton Pattern** - Ensures that a particular service or class instance is created only once and reused across the application.
 - **Strategy Pattern** -  Allows dynamic selection or change of execution strategy without altering the class implementation, used in JWT validation by configuring TokenValidationParameters and `ValidationService` to dynamically inject and execute the appropriate validator instance based on the type being validated..
 - **Chain of Responsibility Pattern** - Used in middleware configurations where each middleware processes the request and decides whether to pass it along the chain.
 - **Unit of Work Pattern** - Automatically implemented by EF Core to ensure atomicity and consistency in database operations.
 - **Aspect-Oriented Programming (AOP)** - implemented through middleware to handle cross-cutting concerns like token validation in a centralized manner, improving modularity and maintainability.
   
### DTO 
Each layer of the application has its own DTOs, which promotes complete separation between them. This ensures that data returned from, for example, the domain layer, will not include sensitive information that could be exposed to the user. DTOs are organized in DTO folders and are divided into two types: request and response. Additionally, each DTO has a suffix clearly indicating its purpose and the layer it is intended for, such as <DtoName>DtoApp for the application layer, <DtoName>DtoDom for the domain layer, and so on. Objects used for transferring data (Models) that are simply part of other DTOs but are not DTOs themselves — they do not have the `DTO` suffix.

### Communication between layers 
Communication follows the principles of Clean Architecture, with dependencies inverted and directed towards the inner domain, using interfaces to ensure that each layer adheres to the proper separation of concerns.
### Authentication, Authorization, and Configuration:
In the Api configuration values such as connection strings, secrets, and keys are securely stored in `Azure Key Vault`, and accessed using the `Options Pattern` to ensure type-safe and manageable configuration handling. Authentication is implemented using `JWT Bearer` tokens, providing secure access control based on roles defined within the application, such as `Visitor` and `Administrator`. Authorization is enforced at the controller and method level, using attributes like `[Authorize(Roles = UserRoles.Administrator)]` for administrators, `[Authorize(Roles = UserRoles.Visitor)]` for standard users, and `[AllowAnonymous]` for actions that are accessible to unauthenticated guests.
### Error Handling
Errors that occur throughout the application's operation are returned to the controllers using the result pattern, while exceptions are thrown only in unforeseen situations due to the system overhead associated with exception handling. The error handling approach in the API can be summarized as: exceptions are thrown where errors are not expected, and where errors are anticipated, the result pattern is used. The application utilizes various types of result classes, which slightly differ from each other but always include the fields `IsSuccess`, `IsFailure`, and `Error` to describe the outcome of an operation. The result classes have static methods that, when called, create a result object with appropriate values assigned to its fields, such as `public static EntityResult<TEntity> Success(TEntity entity) => new(true, Error.None, entity)`; or `public static EntityResult<TEntity> Failure(Error error) => new(false, error, default);`.

The `Error` class, which is a field of the Result class, includes properties like `Code` — indicating the error code that makes it easy to understand why the error was returned, `Description` — providing a full error description, and `Type` — representing the type of error, such as `Failure`, `Validation`, `NotFound`, `Conflict`, etc. The `Error` class also has its own static methods that set the appropriate error type assigned to the Type field, for example, `public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);` and public `static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);`. To standardize errors and facilitate their quick creation, static methods stored in static Errors classes are used, which return instances of the `Error` class e.g. `public static Error NotFoundByProductId(int productId) => Error.NotFound(
    $"{typeof(T).Name}.NotFoundByProductId", $"Entity with product ID: {productId} is missing");`, `public static readonly Error NotRequestedForAccountDeletion = Error.Validation("User.NotRequestedForAccountDeletion", "the user has not requested to delete the account");` .
### Security
application adheres to strict security standards, ensuring that all cookies and tokens are issued in a secure manner. Cookies are set with `HttpOnly` and `Secure` flags, and tokens are managed via secure practices, including the use of `CORS` policies and `HTTPS`. The application leverages `Azure Key Vault` to securely store sensitive configuration data, and `JWT Bearer tokens` are used for authentication and authorization.
### Refresh Token Middleware
In the infrastructure layer, there is a middleware called `JwtRefreshMiddleware`, which handles token refresh operations. With each request to the Api, it retrieves cookies containing the `refresh token` and `authentication token` from the request context. It then validates the authentication token's validity, if the authentication token has expired but the refresh token is still valid, a new authentication token is issued and returned to the user. If a new token is issued, the request is processed based on the new token without requiring the client to make another request to the API after the new authentication token is returned to the browser.
### Global Error Handling Middleware
The API also implements `ExceptionHandlingMiddleware`, which captures all unhandled exceptions thrown throughout the application's execution. This middleware automatically wraps the thrown exception in a `ProblemDetails` object (`Microsoft.AspNetCore.Http.Extensions`), which is then returned to the user’s browser in the standard error format used by the API.

![image](https://github.com/user-attachments/assets/0d45d769-21e3-4ac8-ade4-b68eb29d2c10)


### Extensions
The Api provides extension classes `ValidationResultExtensions` and `ResultExtensions`, which are used to wrap form validation errors and business logic errors in a `ProblemDetails` object, offering a more detailed description of the issue. Additionally, two other extension classes, `ServiceCollectionExtensions` and `ApplicationBuilderExtensions`, help organize the `startup configuration`. These classes are responsible for registering services, configurations, and dependencies such as JWT authentication, database, CORS, and Azure Key Vault, as well as configuring the middleware pipeline, including custom middleware, authentication, and API endpoints setup.
<div style="display: flex; gap: 10px; justify-content: flex-start;">
  <img src="https://github.com/user-attachments/assets/e88465f3-411e-4fe1-9148-da87e877c7ff" alt="ResultExtensions 1" width="33%" align="top">
  <img src="https://github.com/user-attachments/assets/32097767-7221-4512-a67a-e7cc63e56617" alt="ResultExtensions 2" width="33%" align="top">
  <img src="https://github.com/user-attachments/assets/ac80c3ab-d933-49ba-bba6-6728a47453c7" alt="ResultExtensions 3" width="33%" align="top">
</div>
<br></br>

*`ValidationResultExtensions` and `ResultExtensions`*

<br></br>
<div style="display: flex; gap: 10px; justify-content: flex-start;">
  <img src="https://github.com/user-attachments/assets/a799bbbb-db64-41cd-911e-87d77e4ab753" alt="Startup 1" width="33%" align="top">
  <img src="https://github.com/user-attachments/assets/2eb63e16-a21a-4e61-a182-fded7dc44ea9" alt="Startup 2" width="33%" align="top">
  <img src="https://github.com/user-attachments/assets/8e529cc1-385b-4aea-87e3-d102e30d925c" alt="Startup 3" width="33%" align="top">
</div>

*`ServiceCollectionExtensions` and `ApplicationBuilderExtensions`*

### Form validation and business logic validation
Following the Separation of Concerns principle, form validation takes place in the presentation layer at the controller level using FluentValidation, while business logic validation is handled in the application layer, ensuring a clear separation between input data validation and business rules. Additionally, the validation approach uses the Strategy Pattern with Dependency Injection, allowing for dynamic selection of the appropriate validator, making the method generic.

<div style="display: flex; gap: 10px; justify-content: flex-start;">
  <img src="https://github.com/user-attachments/assets/78b2b5a8-53bb-46d8-959a-9ca20d6cdf21" alt="Form validation 1" width="33%" align="top">
  <img src="https://github.com/user-attachments/assets/26c255a5-de2e-4cbd-a2b7-a08b7970d05b" alt="Form validation 2" width="33%" align="top">
  <img src="https://github.com/user-attachments/assets/a9ab11c2-d8c6-4722-b8a4-c34c62c7d9d2" alt="Form validation 3 " width="33%" align="top">
</div>

<br></br>

<div style="display: flex; gap: 10px; justify-content: flex-start;">
  <img src="https://github.com/user-attachments/assets/be1c8daf-3066-45e7-b982-993306e8db99" alt=Business validation 1" width="50%" align="top">
  <img src="https://github.com/user-attachments/assets/3df12b63-0aa3-495c-80ba-38050003e887" alt="Business validation 2" width="50%" align="top">
</div>

### Mapper
AutoMapper library was used for mapping, and the mapping configurations were defined in classes with the `MappingProfile` prefix.
<br></br>
<div style="display: flex; gap: 10px; justify-content: flex-start;">
  <img src="https://github.com/user-attachments/assets/45f5ad77-d347-4914-9f1f-0f5f0ff518f2" alt="Mapper" width="33%" align="top">
  <img src="https://github.com/user-attachments/assets/f7f7b49a-a197-44ed-bf5c-79c2246775db" alt="Mapper" width="33%" align="top">
    <img src="https://github.com/user-attachments/assets/b4f6d92f-d309-46fa-acf6-eb434d85cef9" alt="Mapper" width="33%" align="top">
</div>

### Tests
Test preparation is the next stage of development (in preparation...)

# Endpoints
## Carts Controller
### AddProductsToCartAsync
  - HTTP Path: `POST /api/carts/products`
  - Request Type: `POST`
  - Authorization: `Administrator `
  - Description: This endpoint is used to add products to the cart
  - Input Type: `FromBody`
  - Input: `NewProductsDtoApi`
```json
{
  "products": [
    {
      "productId": "int",
      "quantity": "decimal"
    }
  ]
}
```
  - Output: `Code 200 Ok(CartResponseDto)`
```json
{
  "CartId": "int?",
  "CartLineResponse": [
    {
      "productId": "int?",
      "ProductName": "string",
      "Quantity": "decimal",
      "UnitPrice": "decimal",
      "TotalPrice": "decimal"
    }
  ]
}
```
<br></br> 
### DeleteProductFromCartAsync
  - HTTP Path: `DELETE /api/carts/products/{productId}`
  - Request Type: `DELETE`
  - Authorization: `Administrator`
  - Description: This endpoint is used to delete single product from the cart
  - Input Type: `Parameter`
  - Argumets: `"productId": "int"`
  - Output: `Code 200 Ok(CartResponseDto)`
```json
{
  "CartId": "int?",
  "CartLineResponse": [
    {
      "productId": "int?",
      "ProductName": "string",
      "Quantity": "decimal",
      "UnitPrice": "decimal",
      "TotalPrice": "decimal"
    }
  ]
}
```
<br></br>
### ModifyProductAsync
  - HTTP Path: `PATCH /api/carts/products`
  - Request Type: `PATCH`
  - Authorization: `Administrator `
  - Description: This endpoint is used to modify the quantity of a product in the cart
  - Input Type: `FromBody`
  - Input model: `ModifyProductDtoApi`
```json
{
  "Product": {
    "ProductId": "int",
    "Quantity": "decimal"
  }
}

```
  - Output: `Code 200 Ok(CartResponseDto)`
```json
{
  "CartId": "int?",
  "CartLineResponse": [
    {
      "productId": "int?",
      "ProductName": "string",
      "Quantity": "decimal",
      "UnitPrice": "decimal",
      "TotalPrice": "decimal"
    }
  ]
}
```
<br></br>
### CheckCartAsync
  - HTTP Path: `GET /api/carts`
  - Request Type: `GET`
  - Authorization: `AllowAnonymous`
  - Description: This endpoint is used to retrieve information about the cart's contents, including its total value and creation date
  - Input Type: `-`
  - Output: `Code 204 NoContent`/`Code 200 OK(AboutCartDomResDto)`
```json
{
  "TotalCartValue": "decimal",
  "AboutProductsInCart": [
    {
      "ProductId": "int",
      "ProductName": "string",
      "ProductUnitPrice": "decimal",
      "ProductTotalPrice": "decimal",
      "Description": "string",
      "Manufacturer": "string",
      "Quantity": "decimal"
    }
  ],
  "CreatedAt": "DateTime"
}

```
<br></br>
### ClearCartAsync
  - HTTP Path: `DELETE /api/carts`
  - Request Type: `DELETE`
  - Authorization: `AllowAnonymous`
  - Input Type: `-`
  - Description: This endpoint is used to delete cart content
  - Output: `Code 204 NoContent`

<br></br>    
### GetCartByIdAsync
  - HTTP Path: `GET /api/carts/{cartId}`
  - Request Type: `GET`
  - Authorization: `Administrator`
  - Description: This endpoint is used to get cart content by `Id`
  - Input Type: `Parameter`
  - Argumets: `"cartId": "int"`
  - Output: `Code 200 Ok(CartResponseDto)`
```json
{
  "CartId": "int?",
  "CartLineResponse": [
    {
      "productId": "int?",
      "ProductName": "string",
      "Quantity": "decimal",
      "UnitPrice": "decimal",
      "TotalPrice": "decimal"
    }
  ]
}
```
<br></br>
### ReplaceCartContentAsync
  - HTTP Path: `PUT /api/carts`
  - Request Type: `PUT`
  - Authorization: `AllowAnonymous `
  - Description: This endpoint is used to replace cart content
  - Input Type: `FromBody`
  - Input model: `NewProductsDtoApi`
```json
{
  "Products": [
    {
      "ProductId": "int",
      "Quantity": "decimal"
    }
  ]
}

```
 - Output: `Code 204 NoContent`/`Code 200 Ok(AboutCartDomResDto)`
```json
{
  "TotalCartValue": "decimal",
  "AboutProductsInCart": [
    {
      "ProductId": "int",
      "ProductName": "string",
      "ProductUnitPrice": "decimal",
      "ProductTotalPrice": "decimal",
      "Description": "string",
      "Manufacturer": "string",
      "Quantity": "decimal"
    }
  ],
  "CreatedAt": "DateTime"
}

```
<br></br>
### CheckCurrentCartAsync
  - HTTP Path: `POST /api/carts/check-current-cart`
  - Request Type: `POST`
  - Authorization: `AllowAnonymous `
  - Input Type: `FromBody`
  - Description: This endpoint is used to check by date whether the current cart is up to date. If it is, the function returns nothing, if not, it returns the most up-to-date cart
  - Input model: `CheckCurrentCartDtoApi`
```json
{
  "CreatedAt": "DateTime"
}
```
- Output: `Code 204 NoContent`/`Code 200 Ok(AboutCartDomResDto)`
```json
{
  "TotalCartValue": "decimal",
  "AboutProductsInCart": [
    {
      "ProductId": "int",
      "ProductName": "string",
      "ProductUnitPrice": "decimal",
      "ProductTotalPrice": "decimal",
      "Description": "string",
      "Manufacturer": "string",
      "Quantity": "decimal"
    }
  ],
  "CreatedAt": "DateTime"
}
```
<br></br>
## Orders Controller
### SubmitOrderAsync
  - HTTP Path: `POST /api/orders/{method?}`
  - Request Type: `POST`
  - Authorization: `AllowAnonymous `
  - Description: This endpoint is used to place an order, paying by card or cash on delivery
  - Input Type: `FromBody` + `Parameter`
  - Argumets: `"method": "string?"`
  - Input model: `CrateOrderDtoApi`
```json
{
  "FirstName": "string",
  "LastName": "string",
  "Email": "string",
  "Street": "string",
  "StreetNumber": "string",
  "HomeNumber": "string",
  "PostCode": "string",
  "City": "string",
  "PhoneNumber": "string"
}

```
- Output: `Code 200 Ok(OrderResponseDto)`
```json
{
  "Id": "int",
  "TotalCartValue": "decimal",
  "AboutProductsInCart": [
    {
      "ProductId": "int",
      "ProductName": "string",
      "ProductUnitPrice": "decimal",
      "ProductTotalPrice": "decimal",
      "Description": "string",
      "Manufacturer": "string",
      "Quantity": "decimal"
    }
  ],
  "ShippingDetil": {
    "FirstName": "string",
    "LastName": "string",
    "Email": "string",
    "Street": "string",
    "StreetNumber": "string",
    "HomeNumber": "string",
    "PostCode": "string",
    "City": "string",
    "PhoneNumber": "string"
  }
}
```
<br></br>
### GetOrderByIdAsync
  - HTTP Path: `GET /api/orders/{orderId}`
  - Request Type: `GET`
  - Authorization: `Administrator `
  - Description: This endpoint is used to get order by `Id`
  - Input Type: `Parameter`
  - Argumets: `"orderId": "int"`
  - Output: `Code 200 Ok(OrderResponseDto)`
```json
{
  "Id": "int",
  "TotalCartValue": "decimal",
  "AboutProductsInCart": [
    {
      "ProductId": "int",
      "ProductName": "string",
      "ProductUnitPrice": "decimal",
      "ProductTotalPrice": "decimal",
      "Description": "string",
      "Manufacturer": "string",
      "Quantity": "decimal"
    }
  ],
  "ShippingDetil": {
    "FirstName": "string",
    "LastName": "string",
    "Email": "string",
    "Street": "string",
    "StreetNumber": "string",
    "HomeNumber": "string",
    "PostCode": "string",
    "City": "string",
    "PhoneNumber": "string"
  }
}
```
<br></br>
### DeleteOrderAsync
  - HTTP Path: `DELETE /api/orders/{orderId}`
  - Request Type: `DELETE`
  - Authorization: `Administrator `
  - Description: This endpoint is used to submit order deletion by administrator
  - Input Type: `Parameter`
  - Argumets: `"orderId": "int"`
  - Output: `Code 204 NoContent`
<br></br>
### MarkOrderAsDeletedAsync
  - HTTP Path: `PATCH /api/orders/{orderId}//mark-as-deleted`
  - Request Type: `PATCH`
  - Authorization: `AllowAnonymous `
  - Description: This endpoint is used to mark order as - to deletion by administrator, for user order becomes deleted
  - Input Type: `Parameter`
  - Argumets: `"orderId": "int"`
  - Output: `Code 204 NoContent`
<br></br>
### GetOrdersAsync
  - HTTP Path: `GET /api/orders`
  - Request Type: `GET`
  - Authorization: `Administrator `
  - Description: This endpoint is used to get all orders
  - Argumets: `-`
  - Output: `Code 200 OK(IEnumerable<OrdersResDto>)`
```JSON
[
  {
    "Status": "string",
    "CartLineResponse": [
      {
        "productId": "int?",
        "ProductName": "string",
        "Quantity": "decimal",
        "UnitPrice": "decimal",
        "TotalPrice": "decimal"
      }
    ],
    "ShippingInfo": {
      "FirstName": "string",
      "LastName": "string",
      "Email": "string",
      "Street": "string",
      "StreetNumber": "string",
      "HomeNumber": "string",
      "PostCode": "string",
      "City": "string",
      "PhoneNumber": "string"
    }
  }
]

```
<br></br>
## Payments Controller
### GetClientSecretAsync
  - HTTP Path: `POST /api/payments`
  - Request Type: `POST`
  - Authorization: `AllowAnonymous `
  - Description: This endpoint is used to get stripe secret for user
  - Input Type: `FromBody`
  - Input model: `GetClientSecretDtoApi`
```json
{
  "Amount": "int",
  "Currency": "string"
}
```
- Output: `Code 200 Ok(string)`
<br></br>
### UpdatePaymentIntentAsync
  - HTTP Path: `POST /api/payments/update-payment-intent`
  - Request Type: `POST`
  - Authorization: `AllowAnonymous `
  - Description: This endpoint is used to update payment intent metadata with order id
  - Input Type: `FromBody`
  - Input model: `UpdatePaymentIntentDtoApi`
```json
{
  "clientSecret": "string"
}

```
- Output: `Code 204 NoContent`
<br></br>
### ConfirmPaymentAsync
  - HTTP Path: `POST /api/payments/confirm-payment`
  - Request Type: `POST`
  - Authorization: `AllowAnonymous `
  - Description: This endpoint is used to confirm stripe payment on the backend side
  - Input Type: `FromBody`
  - Input model: `ConfirmPaymentDtoApi`
```json
{
  "PaymentIntentId": "string",
  "PaymentMethodId": "string"
}
```
- Output: `Code 204 NoContent`
<br></br>
### ConfirmPaymentAsync
  - HTTP Path: `POST /api/payments/webhook`
  - Request Type: `POST`
  - Authorization: `AllowAnonymous `
  - Description: This is a webhook triggered by the Stripe API to retrieve the payment status, which is returned to the frontend using SignalR
<br></br>
## Products Controller
### CreateProductAsync
  - HTTP Path: `POST /api/products`
  - Request Type: `POST`
  - Authorization: `Administrator `
  - Description: This endpoint is used to create product
  - Input Type: `FromBody`
  - Input model: `CreateProductDtoApi`
```json
{
  "ProductName": "string",
  "ProductPrice": "decimal",
  "ProductDescription": "string",
  "ProductCategoriesIds": [
    "int"
  ],
  "ProductManufacturer": "string?"
}
```
- Output: `Code 200 Ok(ProductResponseDto)`
```json
{
  "Id": "int",
  "Name": "string",
  "Price": "decimal",
  "Description": "string",
  "Manufacturer": "string?",
  "DateAdded": "DateTime"
}

```
<br></br>
### DeleteProductAsync
  - HTTP Path: `DELETE /api/products/{productId}`
  - Request Type: `DELETE`
  - Authorization: `Administrator `
  - Description: This endpoint is used to delete product
  - Input Type: `Parameter`
  - Argumets: `"productId": "int"`
  - Output: `Code 204 NoContent`
<br></br>
### GetProductByIdAsync
  - HTTP Path: `GET /api/products/{productId}`
  - Request Type: `GET`
  - Authorization: `AllowAnonymous `
  - Description: This endpoint is used to get product by `Id`
  - Input Type: `Parameter`
  - Argumets: `"productId": "int"`
  - Output: `Code 200 Ok(ProductResponseDto)`
```json
{
  "Id": "int",
  "Name": "string",
  "Price": "decimal",
  "Description": "string",
  "Manufacturer": "string?",
  "DateAdded": "DateTime"
}

```
<br></br>
### GetAllProductsAsync
  - HTTP Path: `GET /api/products`
  - Request Type: `GET`
  - Authorization: `AllowAnonymous `
  - Description: This endpoint is used to get all products
  - Input Type: `-`
  - Output: `Code 200 Ok(ProductResponseDto)`
```json
{
  "Id": "int",
  "Name": "string",
  "Price": "decimal",
  "Description": "string",
  "Manufacturer": "string?",
  "DateAdded": "DateTime"
}

```
<br></br>
### UpdateProductAsync
  - HTTP Path: `PUT /api/products`
  - Request Type: `PUT`
  - Authorization: `Administrator `
  - Description: This endpoint is used to update product 
  - Input Type: `FromBody`
  - Input model: `UpdateProductDtoApi`
```JSON
{
  "productId": "int",
  "ProductName": "string?",
  "ProductPrice": "decimal?",
  "ProductDescription": "string?",
  "ProductManufacturer": "string?",
  "ProductCategoriesIds": [
    "int"
  ]?
}
```
  - Output: `Code 200 Ok(ProductResponseDto)`
```json
{
  "Id": "int",
  "Name": "string",
  "Price": "decimal",
  "Description": "string",
  "Manufacturer": "string?",
  "DateAdded": "DateTime"
}

```
<br></br>
## Users Controller
### LoginAsync
  - HTTP Path: `POST /api/users/login`
  - Request Type: `POST`
  - Authorization: `AllowAnonymous `
  - Description: This endpoint is used to login user and **get cookie with JWT token**
  - Input Type: `FromBody`
  - Input model: `AuthenticateUserDtoApi`
```json
{
  "Email": "string",
  "Password": "string"
}

```
- Output: `Code 204 NoContent`
<br></br>
### RegisterUserAsync
  - HTTP Path: `POST /api/users`
  - Request Type: `POST`
  - Authorization: `AllowAnonymous `
  - Description: This endpoint is used to register new account
  - Input Type: `FromBody`
  - Input model: `RegisterUserDtoApi`
```json
{
  "Login": "string",
  "Email": "string",
  "Password": "string",
  "ConfirmPassword": "string"
}
```
- Output: `Code 204 NoContent`
<br></br>
### ConfirmEmailAsync
  - HTTP Path: `GET /api/users/confirm/{userId}/{token}`
  - Request Type: `GET`
  - Authorization: `AllowAnonymous `
  - Description: This endpoint is used to confirm account registration via activation link from email
  - Input Type: `Parameter`
  - Input arguments: `"userId":"string"`, `"token":"string"`
  - Output: `Code 200 Ok(string)`
<br></br>
### RequestDeletionAsync
  - HTTP Path: `PATCH /api/users/request-deletion`
  - Request Type: `PATCH`
  - Authorization: `Visitor`, `Administrator`
  - Description: This endpoint is used to request account deletion, account becomes not visible for users 
  - Input Type: `-`
  - Output: `Code 204 NoContent`
<br></br>
### SubmitDeletionAsync
  - HTTP Path: `GET /api/users/{id}`
  - Request Type: `GET`
  - Authorization: `Administrator`
  - Description: This endpoint is used to submit account deletion by user `Id` 
  - Input Type: `parameter`
  - Input arguments: `"id":"string"`
  - Output: `Code 204 NoContent`
<br></br>
### RemoveRefreshToken
  - HTTP Path: `GET /api/users/logout`
  - Request Type: `GET`
  - Authorization: `Visitor`, `Administrator`
  - Description: This endpoint is used to remove cookie with authentication token from user's browser  
  - Input Type: `-`
  - Output: `Code 204 NoContent`
<br></br>
### RemoveGuestSessionId
  - HTTP Path: `GET /api/users`
  - Request Type: `GET`
  - Authorization: `AllowAnonymous`
  - Description: This endpoint is used to remove cookie guest session Id from user's browser  
  - Input Type: `-`
  - Output: `Code 204 NoContent`
    <br></br>
# Database 
## ERD Diagram
## Entities
### `ASP.NET Core Identity tables`
- `StoreUser` - class extends the default IdentityUser provided by ASP.NET Core Identity. This table represents the user in the system and stores references to their orders(`StoreUser` `(1)->(N)` `Order`) and cart(`StoreUser` `(1) -> (1)` `Cart`).
- `AspNetRoleClaims` - Stores claims associated with roles, enabling role-based authorization by attaching specific claims to a role
- `AspNetRoles` - Stores roles that can be assigned to users, such as "Administrator" or "Visitor."
- `AspNetUserClaims` - Stores claims associated with users, allowing specific claims-based authorization on a per-user basis
- `AspNetUserLogins` - Tracks external login information (e.g., Google, Facebook) associated with users, allowing them to log in using third-party services
- `AspNetUserRoles` - A join table between users and roles, associating a specific user with one or more roles.
- `AspNetUsers` - `Stores user data, including information such as usernames, email addresses, and password hashes.
- `AspNetUserTokens` - Stores security tokens associated with users, which can be used for actions like password resets or multi-factor authentication.`
  
### `Order`
The `Order` table represents a placed order and stores information about its status in the `Status` column. The table is linked to the `Cart` (`Order` `(1) -> (1)` `Cart`) table, allowing the ordered products to be tracked and to the `StoreUser`(`Order` `(N) -> (1)` `StoreUser`) table, indicating who placed the order. `Order` has also a reference to the `ShippingDetail` (`Order` `(1) -> (1)` `ShippingDetail`) table, which stores shipping-related information.

### `ShippingDetail`
The `ShippingDetail` table represents the delivery details of the user placing the order and it's only connected to `Order` (`ShippingDetail` `(1) -> (1)` `Order`) table. 

# CI/CD
The backend API is connected to Azure Web App and the CI/CD process starts automatically after each push to the main branch. The API is built and deployed using GitHub Actions, with Entity Framework Core migrations applied during the process. Secrets, such as Azure credentials and the SQL connection string, are used for secure configuration. After every push, the API is deployed to Azure Web App.
<br></br>
# [thanks for reading! Now you are ready to check the Front-end :) ](https://github.com/Ksztof/store-front)
