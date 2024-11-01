# 🛒 E-Commerce Web API with .NET 8 & SQL Server

Welcome to the **E-Commerce API** repository, built with **.NET 8** and **SQL Server** for seamless, high-performance e-commerce operations! This API is crafted to deliver robust e-commerce features, supports multiple languages, and is optimized for great performance.

---

## 📋 Table of Contents

- [✨ Features](#-features)
- [🚀 Getting Started](#-getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [📁 Project Structure](#-project-structure)
- [⚙️ Configuration](#️-configuration)
- [📖 API Documentation](#-api-documentation)
- [🌐 Localization Support](#-localization-support)
- [📊 Performance Optimization](#-performance-optimization)
- [🛠 Contributing](#-contributing)
- [📜 License](#-license)

---

## ✨ Features

- **.NET 8** - Utilizes the latest .NET framework for performance and reliability.
- **SQL Server** - Leverages SQL Server for secure and efficient data management.
- **Multi-Language Support** - Offers multiple language options for a global reach.
- **High Performance** - Optimized for high traffic and large e-commerce datasets.
- **Extensible and Modular** - Easily customizable for new features and updates.
- **Secure** - Built-in authentication and authorization following best practices.

---

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) or SQL Server Docker container
- [Git](https://git-scm.com/)

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/your-ecommerce-api.git
   cd your-ecommerce-api
   ```

2. **Set up SQL Server**: Ensure SQL Server is running and reachable. Configure connection strings as needed in `appsettings.json`.

3. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

4. **Build the project**:
   ```bash
   dotnet build
   ```

5. **Run database migrations** (if applicable):
   ```bash
   dotnet ef database update
   ```

6. **Launch the API**:
   ```bash
   dotnet run
   ```

The API should now be running at `http://localhost:5000`.

---

## 📁 Project Structure
not yet organized

```
/src
├── Controllers       # API endpoints
├── Data              # Data models and database context
├── Services          # Business logic layer
├── Localization      # Resource files for multi-language support
├── Middleware        # Custom middleware for handling requests
└── Program.cs        # Application entry point
```

---

## ⚙️ Configuration

### Database

In `appsettings.json`, configure the database connection string to point to your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;"
}
```

### Localization

Add supported languages and resource files to `/Localization`. You can specify default languages and cultures in `Program.cs` for global support.

---

## 📖 API Documentation

The API documentation is generated via **Swagger** and available at:

- `http://localhost:5000/swagger`

This includes detailed information about each endpoint, parameters, responses, and example requests.

---

## 🌐 Localization Support

This API supports multiple languages to cater to a global user base. Localization is enabled for:

- **Error Messages**
- **UI Texts**
- **Notification Templates**

To add more languages, simply create new `.resx` files in the `/Localization` folder and configure them in `Program.cs`.

---

## 📊 Performance Optimization

.NET 8 and SQL Server enable **high-performance features** for this API:

- **Entity Framework Core** optimizations
- **Connection Pooling** for SQL Server
- **Asynchronous Processing** for scalable request handling
- **Caching** and **Response Compression** to reduce load times
- **Minimal APIs** for faster endpoint routing

---

## 🛠 Contributing

We welcome contributions! To contribute:

1. **Fork the repository**
2. **Create a branch** for your feature or bug fix:
   ```bash
   git checkout -b feature/your-feature
   ```
3. **Commit your changes**:
   ```bash
   git commit -m "Add your feature description"
   ```
4. **Push to your branch**:
   ```bash
   git push origin feature/your-feature
   ```
5. **Open a Pull Request**

Please ensure all new code follows the existing coding style and is well-documented.

---

## 📜 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

Happy Coding! 🎉