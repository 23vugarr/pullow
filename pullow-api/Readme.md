# Pullow API - Portfolio Management System for Endowment Insurance (Backend API)

## Overview

This repository contains the backend API for a Portfolio Management System tailored for Endowment Insurance. The system aims to facilitate the management of portfolios related to endowment insurance policies. It provides functionalities for tracking policies, managing portfolios, generating reports, and more.

## Features

- **Portfolio Tracking**: Generate strategy based on goal and pasha life data
- **Goals**: Generate goals assign users
- **User Management**: Manage user accounts, roles, and permissions.
- **Authentication and Authorization**: Secure endpoints with authentication and authorization mechanisms.

## Technologies Used

- **ASP.NET Core 6**: Backend framework for building APIs.
- **C# 10**: Primary programming language.
- **Entity Framework Core**: ORM for database access.
- **Swagger**: API documentation and testing tool.
- **JWT Authentication**: JSON Web Tokens for secure authentication.
- **Postgresql**: Relational database management system for data storage.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/download) (Optional)

### Installation

1. Clone this repository: `git clone https://github.com/your/repository.git`
2. Navigate to the project directory: `cd portfolio-management-api`
3. Restore dependencies: `dotnet restore`
4. Update database schema: `dotnet ef database update`
5. Run the application: `dotnet run`

### Usage

- Access the API documentation and test endpoints using Swagger UI: `https://localhost:5001/swagger/index.html`

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvement, please feel free to open an issue or submit a pull request.

## License

This project is licensed under the [MIT License](LICENSE).

## Contact

For any inquiries or support, please contact [Your Name](mailto:your.email@example.com).
