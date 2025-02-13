# Revenue Recognition System

## Overview
The **Revenue Recognition System** is a comprehensive web application built with ASP.NET Core that manages software sales contracts and payments. It enables both individual and business clients to create agreements and register payments. The system also implements robust authentication and authorization mechanisms using JWT.

## Architecture & Objectives
- **Primary Goal**:  
  Manage software sales contracts and payments by:
  - Creating agreements with both individual and business clients.
  - Registering payments and tracking contract statuses.
  - Calculating actual and forecasted revenue.
  - Applying discounts on software purchases.
- **Security**:  
  Implements JWT-based authentication and authorization for secure access.

## Technologies & Tools
- **.NET 8.0 Web API** Utilizes minimal APIs and controllers for endpoint creation.
- **Entity Framework Core** Provides ORM capabilities for managing a SQL Server database.
- **JWT** Handles user authentication and token management (access token and refresh token).
- **FluentValidation** Validates incoming request models to ensure data integrity.
- **xUnit** Used for unit testing business logic and database operations.

## Data Access Layer - DatabaseContext
- **File**: `DatabaseContext.cs`
- **Key Features:**
  - **DbSet Definitions** Defines DbSets for entities such as Clients, Individuals, Businesses, Softwares, Agreements, Payments, Discounts, Subscriptions, AppUserModels, and RefreshTokens.
  - **Model Configuration** Maps entities to specific tables (e.g., `ToTable("Clients")`) and configures relationships between them (e.g., between Payment and Client).
  - **Data Seeding** Seeds initial data for certain entities (e.g., Software and Discount) to facilitate testing and provide default records upon migration.

## Business Logic Layer - Models, Services, Exceptions

### Models
The project includes domain entities representing the main business objects:
- **Client, Individual, Business**:  
  - Represent clients, with `Individual` and `Business` inheriting from `Client`.
  - Include properties such as address, email, phone, and specific identifiers (e.g., PESEL for individuals, KRS for businesses).
- **Software**:  
  - Represents software products with details like one-time pricing, subscription pricing, and purchase type.
- **Agreement**:  
  - Manages contract details including price, payment dates, software version, and signing status.
- **Payment**:  
  - Records client payments, including amount, date, and links to the corresponding agreement.
- **Discount**:  
  - Contains discount details such as percentage value and validity period.
- **Subscription**:  
  - Handles subscription-based software purchase models.
- **AppUserModel & RefreshToken**:  
  - Represent system users and tokens for session extension.

### Exceptions
Custom exceptions handle specific business logic errors:
- **AgreementAlreadySigned**:  
  Thrown when an agreement has already been signed.
- **AmountHasBeenExceeded**:  
  Indicates that the payment amount exceeds the agreed contract value.
- **NotFoundException**:  
  Used when a required resource (e.g., client, software, or agreement) is not found.
- **WrongSoftwareType**:  
  Thrown when the software type does not match the expected purchase type (e.g., expecting a one-time purchase but received a subscription).

### Services
Services implement business logic using the `DatabaseContext`:
- **AgreementService**:  
  - Creates agreements by fetching client and software data, applying discounts (including additional discounts for returning customers), calculating the final price (including technical support fees), and saving new agreements.
- **PaymentService**:  
  - Manages payment registrations and revenue calculations.
  - Verifies that payments do not exceed the agreed amount.
  - Updates agreement statuses and uses an external API (via HttpClient) to fetch currency exchange rates.
- **ClientService**:  
  - Handles create, read, update and delete operations for both individual and business clients.
- **AuthService**:  
  - Implements login, registration, and token refresh functionalities.
  - Utilizes `SecurityHelpers` for JWT generation and password validation.

### Helpers – SecurityHelpers.cs
Contains static methods for:
- Generating JWT and refresh tokens.
- Hashing passwords using salting and the PBKDF2 algorithm.
- Validating passwords against stored hashes.

![Hashing](https://carlpaton.github.io/d/salted-hash/password-hash-salt-1.png)

### Validation – Validators
FluentValidation is used to ensure incoming data meets specified criteria:
- **AuthValidator**:  
  Validates login and registration data.
- **CreateAgreementValidator**:  
  Checks that payment dates are within a valid range (e.g., 3 to 30 days) and that the support period is appropriate.
- **CreateIndividualClientValidator & CreateBusinessClientValidator**:  
  Validate client data such as PESEL length, email format, phone number length, and KRS for businesses.
- **CreatePaymentValidator**:  
  Ensures that the payment amount is greater than zero.

## Testing – Project.Tests
- **Unit Testing with xUnit**:  
  Uses an in-memory database to simulate database operations without the need for a real SQL Server.
- **Key Tests**:
  - **AgreementServiceTest**:  
    Validates agreement creation and handles error scenarios (e.g., missing client/software or mismatched software type).
  - **ClientServiceTests**:  
    Tests the creation, update, and deletion of clients.
  - **PaymentServiceTests**:  
    Verifies payment registration, checks for exceeded payment amounts, ensures proper exception handling, and calculates revenue.
- **Dummy HttpClient**:  
  `HttpClientDummy.cs` is used in PaymentService tests to simulate external API responses (e.g., for fetching currency exchange rates).

## Configuration & Deployment

### Program.cs
- Configures the ASP.NET Core application.
- Registers all necessary services in the DI container (e.g., DbContext, business services, validators).
- Sets up JWT authentication (configuring issuer, audience, and secret key).
- Adds authorization policies (e.g., "UserPolicy" and "AdminPolicy").
- Maps endpoints using minimal APIs for login, registration, client management, agreements, and payments.
- Integrates Swagger for API documentation and testing.

### Configuration Files
- **appsettings.json & appsettings.Development.json**:  
  Contain configurations for logging, the SQL Server connection string, and JWT settings.
