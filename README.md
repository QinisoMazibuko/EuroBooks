# EuroBooks

# Tech Stack
* ASP.Net Core 3.1 (I did not use .Net 5 as it does not yet have long term support)
* ASP Identity (JWT authorization)
* Microsoft SQL Server with Entity Framework Core

## Packages
* FluentValidation
* MediatR
* Swagger
* Automapper

## Testing Frameworks
* NUnit with Shouldly

## Solution Breakdown:

* The Backend solution is an implementation of the command, Query, Responsibility Segragation Pattern/Architecture (CQRS) 
* API
	* EuroBooks.API
		* This layer depends on both the Application and Infrastructure layers, however, the dependency on Infrastructure is only to support dependency injection. Therefore only *Startup.cs* should reference Infrastructure.
		* In appsettings.json 
		* To be able to run migrations, set this as Startup project
		* Run migrations using following command
		> Add-Migration 'Migration text' -Context ApplicationDbContext -OutputDir Migrations\SQLServer
		> Update-Database -Context ApplicationDbContext
	* EuroBooks.API.Integration.Tests
		* Test full API functionality
* Infrastructure
	* EuroBooks.Infrastructure	
> This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.
* Application
	* EuroBooks.Application
		* Command and Queries objects
		* Validation
	* EuroBooks.Application.Integration.Tests
		* Test for full Command or Query execution
> This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer or project. 
> This layer defines interfaces that are implemented by outside layers. 
> For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.
* Domain
	* EuroBooks.Domain
> This will contain all entities, enums, exceptions, interfaces, types and logic specific to the domain layer