# E-Commerce Web API 🛒

**Scalable E-Commerce Backend API**

A scalable e-commerce Web API built with **ASP.NET Core**, providing the core backend functionality required for an online shopping platform.

The API handles product management, shopping cart operations, order processing, user authentication, and secure payment integration using Stripe.

## 🎯 Overview

The project focuses on building a maintainable and scalable backend for an e-commerce platform using modern ASP.NET Core development practices.

It provides RESTful APIs for managing products, customers, shopping carts, orders, and payments.

## ✨ Features

### Product Management

* Product management
* Product categorization
* Product-related API operations

### Shopping Cart

* Shopping cart management
* Add and remove products
* Cart item management

### Orders

* Order processing
* Order management
* Order-related operations

### Authentication

* User authentication
* Secure API access
* JWT-based authentication

### Payments

* Secure payment integration
* Stripe payment processing

## 🏗️ Architecture

The API is designed using a layered architecture with a focus on separation of concerns and maintainability.

Key concepts include:

* RESTful API design
* Dependency Injection
* Repository Pattern
* Unit of Work
* Specification Pattern
* Entity Framework Core
* Service Layer
* DTOs
* AutoMapper
* JWT Authentication

## 🛠️ Technologies

| Technology            | Purpose              |
| --------------------- | -------------------- |
| C#                    | Programming Language |
| ASP.NET Core          | Web API Framework    |
| Entity Framework Core | ORM                  |
| SQL Server            | Database             |
| JWT                   | Authentication       |
| Stripe                | Payment Processing   |
| AutoMapper            | Object Mapping       |
| Swagger / OpenAPI     | API Documentation    |
| Postman               | API Testing          |

## 💳 Payment Integration

The project integrates with **Stripe** to handle secure payment processing.

Payment-related operations are handled through the backend API while following a separation between application logic and external payment services.

## 🗄️ Database

The application uses **SQL Server** with **Entity Framework Core** for database management and persistence.

The database contains the core entities required for products, users, shopping carts, orders, and payment-related operations.

## 📖 API Documentation

Swagger / OpenAPI is used to document and explore the available API endpoints.

## 🚀 Getting Started

### Prerequisites

* .NET SDK
* SQL Server
* Visual Studio or another compatible IDE

### Installation

Clone the repository:

```bash
git clone https://github.com/SeifSherif84/Ecommerce-Web-API.git
```

Navigate to the project:

```bash
cd Ecommerce-Web-API
```

Restore dependencies:

```bash
dotnet restore
```

Configure the database connection and required application settings.

Apply database migrations:

```bash
dotnet ef database update
```

Run the application:

```bash
dotnet run
```

## 📌 Project Status

This project was built to practice and demonstrate real-world ASP.NET Core Web API development, backend architecture, authentication, database management, and payment integration.

## 👨‍💻 Author

**Seif Sherif**

.NET Backend Developer | C# | ASP.NET Core

GitHub: [@SeifSherif84](https://github.com/SeifSherif84)
