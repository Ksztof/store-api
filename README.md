# Overview
store-api is a REST API for an online store that serves as the backend for the frontend application store-front. The API handles operations related to data management, business logic, authentication, authorization, and communication with the database. It is the central component of the system, which, together with the store-front application, forms a comprehensive e-commerce solution. By clearly separating the frontend and backend, the API enables scalability, security, and easier maintenance of the application, allowing for independent development and updates of both parts of the system.

# Tech Stack
### [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)
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
The project utilizes the Clean Architecture design pattern, which divides the project into Presentation, Application, Domain, and Infrastructure layers, thereby separating business logic from infrastructure. This architecture adheres to SOLID principles, improving the structure of the code and reducing its complexity. By implementing this pattern, the code has become scalable and easily extendable with new functionalities.

![image](https://github.com/user-attachments/assets/0f8ace32-f8bc-45fe-9401-2156c135f0cb)

### Domain-driven design (DDD)
Implementing DDD in the project allowed us to focus on business logic by encapsulating it within entity classes, where each entity has its own methods that perform data operations according to business rules. This separation ensures that the business logic is distinct from the rest of the system, making it easier to manage and maintain.
### Layers
By implementing a layered architecture in accordance with Clean Architecture, including the use of interfaces, we have the ability to swap out individual layers of the application. The layers are not dependent on specific implementations but rely on abstractions provided by interfaces. This allows us, for example, to replace the presentation layer with Blazor components or another frontend technology, or to change the infrastructure layer to switch to MongoDB, or simply to replace specific repository implementations.
  - **Presentation Layer (`Store.Api`)**: This layer is responsible for handling requests from front-end application. We can find here controllers, extension functions used for clear presentation of API responses, application startup configuration (Program.cs) and validation for inputs and form data submitted in requests.
  - **Application Layer (`Store.Application`)**: The application layer manages application logic and implements use cases that define how users can interact with the system. It acts as a mediator between the presentation and domain layers. We can find here specific implementations of application services and contracts, such as the KeyVaultOptions class, which defines the data that should be provided from Azure Key Vault without specifying exactly how these data should be retrieved. This layer also contains interfaces for infrastructure services, which describe how the application layer wants to communicate with the infrastructure layer. In other words, they define how it intends to use services that interact with external providers, which, according to Clean Architecture principles, belong to the infrastructure layer. In this layer, business validation also takes place, for example, to handle conflicts or missing objects, by calling functions defined in repository interfaces that are located in Domain layer, with their specific implementations in the infrastructure layer.
  - **Domain Layer (`Store.Domain`)**: The domain layer is the core of the application, containing business logic and domain rules. We can find here entities along with functions that are closely associated with them and implement business logic. These entities are mapped to database tables. The layer also includes repository interfaces and objects used for returning operation results using the result pattern, such as the `Result` class, which wraps an Error object providing additional information about the operation's outcome. The `Error` class represents an error by providing details about its code, error specifics, and type (`Failure`, `Validation`, `NotFound`, `Conflict`, etc.). Additionally, there are `Errors` classes that provide static methods mapped to the Error object, which are used to create specific errors with the previously mentioned fields.
### Design patterns
### DTO 
### Communication between layers 
(komunikacja zgodna z zasadami Clean architect,  zależności odwrotne, intefejsy, ależności idą w kierunku wewnętrznej domeny.)
### Authentication, Authorization, and Configuration:
konfiguracja: Łańcuchy połączeń do baz danych (np. SQL Server)., Tajności i klucze, key vault, options pattern 
### Error Handling
Result pattern + throwing exceptions
### Security
CORRS cookies https only itd
### Refresh Token Middleware
### Global Error Handling Middleware 
### Extensions
### Form Validation + Domain layer validatiron 
### Mapper
### Organization of Program.cs


# Database 
### ERD Diagram
### Entities
