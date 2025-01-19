# E-Commerce Inventory and Order Management API

A scalable and secure **.NET 7 API** built to manage inventory, orders, and customers for an e-commerce platform. It features role-based authentication, CRUD operations for products, order management, inventory handling, and Docker containerization.

## Table of Contents
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Local Development Setup](#local-development-setup)
- [Docker Setup](#docker-setup)
- [Testing the API](#testing-the-api)
- [Security Considerations](#security-considerations)
- [Notes](#notes)

## Features

- **Authentication & Authorization**: JWT-based authentication with roles:
  - **Admin**: Manage products, inventory, orders, and customers.
  - **Customer**: Place orders and view order history.

- **Product Endpoints** (`/api/products`):
  - `GET`: Fetch paginated and filtered product data.
  - `POST` (Admin): Add a new product.
  - `PUT {id}` (Admin): Update product details.
  - `DELETE {id}` (Admin): Delete a product.

- **Order Endpoints** (`/api/orders`):
  - `POST`: Place an order, validate stock availability.
  - `GET` (Admin): View all orders with filters.
  - `GET {id}`: Retrieve order details.
  - `PUT {id}/status` (Admin): Update order status.

- **Inventory Management**:
  - Deduct stock on order placement.
  - Notify admins when stock falls below a threshold.

- **Order Tracking**:
  - Assign unique order tracking numbers.
  - Allow customers to track order status.

- **Sales Reports**:
  - Provide summary reports for admins (e.g., daily/weekly sales, top-selling products).

- **Database**: 
  - Uses Entity Framework Core with SQL Server. 
  - Tables: Products, Orders, OrderItems.

- **Documentation**:
  - Swagger UI for API exploration and testing.

- **Deployment**:
  - Dockerized for deployment.
  - Environment variables for secrets and configurations.

## Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or LocalDB
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for Docker setup)
- A tool like [Postman](https://www.postman.com/) or a web browser for testing endpoints

## Local Development Setup

### 1. Clone the Repository
```bash
git clone git@github.com:Halfhero212/QiECommerceAPI.git





