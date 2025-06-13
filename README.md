# 🌿 Plantify – eCommerce for Plants

An ASP.NET Core MVC web application for buying indoor plants. Users can browse, add to cart, and place orders. Admins can manage product inventory and view orders.

## 🔧 Features

- User registration, login, and claims-based greeting
- Role-based authorization with admin access to inventory and orders
- Add to cart and checkout functionality
- Product catalog with 10+ items
- Entity Framework Core with SQL Server
- Azure Blob Storage integration for image handling
- Clean MVC architecture using Razor Pages
- Bootstrap 5 for responsive UI

## 🗂 Tech Stack

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (Code First)
- Microsoft Identity
- Azure Blob Storage
- Bootstrap 5
- Razor Pages

## 🧠 Entity Relationships

- One-to-many: Cart ↔ CartItems
- One-to-many: Orders ↔ OrderItems
- One-to-many: Products ↔ CartItems & OrderItems

## 👤 Author

**Amith Acharya**  
Bengaluru, India  
Passionate about .NET MVC, clean architecture, and scalable web apps.
