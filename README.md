TixGen - Technical Support Management System-ASP.NET Web API

TixGen is a real-time technical support management system built using ASP.NET Web API, Entity Framework Core, JWT Authentication, and Role-Based Authorization. This project provides a structured way to manage support tickets with role-based access control for Admin and Agent users.

Features
	• User Authentication & Authorization 
		○ Registration with role assignment (Admin, Agent)
		○ JWT-based authentication
		○ ASP.NET Core Identity for role management
	• Ticket Management 
		○ Create, assign, and resolve tickets
		○ Role-based access control for operations
	• Logging Middleware 
		○ Custom middleware for request logging
	• Entity Framework Core Integration 
		○ MS SQL Server as the database
		○ Migrations for database schema management
	• RESTful API Endpoints 
		○ Secure API routes with authorization policies
		
Technologies Used
	• Backend: ASP.NET Core 6+, Entity Framework Core
	• Authentication: JWT Tokens, ASP.NET Core Identity
	• Database: Microsoft SQL Server
	• Middleware: Custom Logging Middleware
	

Prerequisites
	• .NET 6+ SDK
	• SQL Server
	
Setup Instructions
	1. Clone the Repository
git clone https://github.com/your-username/TixGen.git
cd TixGen
	2. Update Connection String in appsettings.json
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=TixGenDB;Trusted_Connection=True;"
}
	3. Run Database Migrations
dotnet ef database update
	4. Run the Application
dotnet run
API Endpoints
Authentication
Method	Endpoint	Description
POST	/api/auth/register	Register a user
POST	/api/auth/login	Login a user
		
Ticket Management
Method	Endpoint	Description
POST	/api/tickets/create	Create a new ticket
PUT	/api/tickets/assign	Assign a ticket
PUT	/api/tickets/resolve	Resolve a ticket
		
Role-Based Access
	• Admin: Can create, assign, and manage tickets
	• Agent: Can only resolve assigned tickets
	
Project Structure
TixGen/
│-- Controllers/
│   ├── AuthController.cs
│   ├── TicketsController.cs
│-- Data/
│   ├── AppDbContext.cs
│-- Middleware/
│   ├── LoggingMiddleware.cs
│-- Models/
│   ├── User.cs
│   ├── Ticket.cs
│-- Program.cs
│-- appsettings.json
│-- TixGen.csproj

Future Enhancements
	• Implement real-time notifications using SignalR
	• Add a React frontend for a complete full-stack application
	
Contributing
Contributions are welcome! Feel free to fork the repo and submit pull requests.

License
This project is licensed under the MIT License.
![image](https://github.com/user-attachments/assets/71695cbb-0489-47a3-a60f-e07494328467)
