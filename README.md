# AspnetMicroservices

### Environment Setup
#### Install Docker
* [Docker Desktop](https://www.docker.com/products/docker-desktop)

#### Install Visual Studio
* [Visual Studio](https://visualstudio.microsoft.com/downloads/)

#### **Useful things to know in the solution settings when working with Docker containers**
-  In the source code of the solution there is a file called appsettings.json. This file is responsible for env variables that will be used in the program. For instance the DatabaseConnection. This is the connection string to the MongoDB datanase. This is set to localhost by default. However, when running the solution in a Docker container, the localhost will not work. Instead, we must use the service name of the mongo db. We do this by injecting the env variable in the docker-compose file. This is done by using the following syntax: `environmnent: - DatabaseSettings:ConnectionString=mongodb://catalogdb:27017`. This will set the env variable to the connection string of the mongo db.
- While trying to debug the program when working with containers. Especially when launching the program from Visual Studio. It is important to know that the program will use the default ConnectionString that's set in the appsettings.json file. This is because the env variable is not set when launching the program from Visual Studio. To get around this, we use the appsettings.Development.json file and define the `DatabaseSettings:ConnectionString` pointing to the container service connection in that file. This file is used when launching the program from Visual Studio in the Development environment.

## Breakdown of all the RESTful APIs
### Catalog API (Products)

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

#### The APIs are structured in a 3 layer architecture:
- Data Access Layer
- Business Logic Layer
- Presentation Layer

#### **Data Access Layer**
Only database operations are performed in this layer. This layer is responsible for the CRUD operations. This layer is also responsible for the connection to the database. This layer is defined in the `Data/` folder and is called `CatalogContext.cs`. This layer is injected into the Business Logic Layer.

#### **Business Logic Layer**
This layer is responsible for the business logic of the application. For instance, if we want to add a new product, we would do so in this layer. Using this layer. This layer is defined in the `Repositories/` folder and is called `ProductRepository.cs`. This layer is injected into the Presentation Layer or API Controller Endpoints.

![3 Layer Architecture](./Images/3layer.png)

#### **Here is a breakdown of the folder structure of the Catalog API**

*Data/* - Contains the MongoDB Context (connection string, database name, collection name)

*Entities/* - Contains the Product entities (description of the product)

*Repositories/* - Contains the Product repository (CRUD operations) this is where the context is injected. This provides an abstraction layer between the controller and the database. 

The benefit of this is that if we ever want to change the logic of how we interact with the database, we can do so without changing the controller and simply change the repository accordingly. When using this we simply use functions such as `GetAllProducts()` or `GetProductById(x)`. This is also particularly useful for unit testing as we can mock the repository and test the controller.

*Controller/* - Contains the Product controller (RESTful API endpoints)

### Basket API (Shopping Cart)
| Method | Request URI | Use case |
|:--------|:-------------|:----------|
|GET|api/v1/Basket|Get basket and items with username|
|POST|api/v1/Basket|Update Basket and Items (add-remove item on basket)|
|DELETE|api/v1/Basket|Delete basket|
|POST|api/v1/Basket/Checkout|Checkout basket|
