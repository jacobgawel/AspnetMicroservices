# AspnetMicroservices

### Breakdown of all the RESTful APIs
#### Catalog API (Products)

| Method | Request URI | Use case |
|:--------|:-------------|:----------|
|GET|api/v1/Catalog|Listing Products and Categories|
|GET|api/v1/Catalog/{id}|Get Products with product Id|
|GET|api/v1/Catalog/GetProductByCategory/{category}|Get products by category|
|POST|api/v1/Catalog|Create new product|
|PUT|api/v1/Catalog|Update product|
|DELETE|api/v1/Catalog|Delete product|

#### Catalog API Nuget Packages
| Package | Version | Use case |
|:--------|:-------------|:----------|
|Swashbuckle.AspNetCore|6.5.0|Swagger UI|
|MondoDB.Driver|2.21.0|MongoDB Driver|

#### General Structure of Catalog API

*Data/* - Contains the MongoDB Context (connection string, database name, collection name)

*Entities/* - Contains the Product entities (description of the product)

*Repositories/* - Contains the Product repository (CRUD operations) this is where the context is injected. This provides an abstraction layer between the controller and the database.

*Controller/* - Contains the Product controller (RESTful API endpoints)
