Project Task Management System
Project Overview
The Project Task Management System is a full-stack web application developed to manage projects and tasks efficiently. The system supports user authentication and role-based authorization using JWT (JSON Web Token), allowing different permissions for Admin and User roles.
The application enables project management, task assignment, task tracking, and status updates through a modern Angular frontend and an ASP.NET Core Web API backend connected to a SQL Server database.

Technologies Used
Frontend
•	Angular
•	TypeScript
•	HTML
•	CSS
•	Angular Router
•	HttpClient
•	JWT Interceptor
Backend
•	ASP.NET Core Web API
•	C#
•	Entity Framework Core
•	LINQ
•	Dependency Injection
•	JWT Authentication & Authorization
Database
•	Microsoft SQL Server
I used Entity Framework Core Migrations to create and manage the database schema. I created the initial migration using Add-Migration InitialCreate and applied it to SQL Server using Update-Database. This automatically created the database tables based on my entity models.




System Features
Authentication & Authorization
•	User Registration
•	User Login
•	Password Hashing using SHA256
•	JWT Token Generation
•	JWT Token Validation
•	Role-Based Authorization (Admin / User)
Project Management
•	Create Projects
•	View Projects
•	Update Projects
•	Delete Projects
Task Management
•	Create Tasks
•	View Tasks
•	Update Tasks
•	Delete Tasks
•	Assign Tasks to Users
•	Track Task Status

User Roles
Admin
The Admin can:
•	Create Projects
•	Update Projects
•	Delete Projects
•	Create Tasks
•	Update Task Details
•	Delete Tasks
•	Assign Tasks to Users
The Admin cannot modify task progress status.

User
The User can:
•	View Assigned Tasks
•	Update Task Status
o	To Do
o	In Progress
o	Done
The User cannot:
•	Create Tasks
•	Delete Tasks
•	Modify Task Details

Database Design
Users Table
Stores user information:
•	Id
•	FullName
•	Email
•	PasswordHash
•	Role
Projects Table
Stores project information:
•	Id
•	Name
•	Description
•	CreatedDate



Tasks Table
Stores task information:
•	Id
•	Title
•	Description
•	Status
•	Priority
•	CreatedDate
•	ProjectId
•	AssignedUserId
Backend Architecture
Controllers
Controllers receive HTTP requests from Angular and return responses.
Examples:
•	AuthController
•	ProjectController
•	TaskController
Services
Services contain business logic and database operations.
Examples:
•	AuthService
•	ProjectService
•	TaskService
Interfaces
Interfaces define service contracts and are used with Dependency Injection.
Examples:
•	IAuthService
•	IProjectService
•	ITaskService
Program.cs
Program.cs configures:
•	SQL Server Connection
•	Dependency Injection
•	JWT Authentication
•	Authorization
•	CORS
•	Swagger

Dependency Injection
Dependency Injection is used to inject services into controllers.
Example:
builder.Services.AddScoped<IProjectService, ProjectService>();
This improves maintainability, scalability, and testability.

JWT Authentication Flow
1.	User enters email and password.
2.	Backend validates credentials.
3.	AuthService generates a JWT Token.
4.	Token is returned to Angular.
5.	Angular stores the token in Local Storage.
6.	Angular Interceptor automatically attaches the token to every API request.
7.	ASP.NET Core validates the token before allowing access to protected endpoints.

Database Connection Flow
Frontend → Backend
Angular sends HTTP requests using HttpClient.
Example:
this.http.get('/api/Task');
Backend → Database
Controllers call Services.
Services use Entity Framework Core through AppDbContext.
Example:
_context.TaskItems.ToListAsync();
Database → Backend → Frontend
SQL Server returns data → Entity Framework Core maps it to C# objects → API returns JSON → Angular displays the data.

Project Architecture
Angular UI
↓
HttpClient
↓
ASP.NET Core Controllers
↓
Interfaces
↓
Services
↓
Entity Framework Core
↓
AppDbContext
↓
SQL Server Database

Packages Used
Backend Packages
•	Microsoft.EntityFrameworkCore
•	Microsoft.EntityFrameworkCore.SqlServer
•	Microsoft.EntityFrameworkCore.Tools
•	Microsoft.AspNetCore.Authentication.JwtBearer
•	Microsoft.IdentityModel.Tokens
•	System.IdentityModel.Tokens.Jwt
Frontend Packages
•	@angular/router
•	@angular/forms
•	@angular/common/http
•	jwt-decode

Key Concepts Implemented
•	RESTful APIs
•	CRUD Operations
•	Entity Framework Core
•	Dependency Injection
•	JWT Authentication
•	Role-Based Authorization
•	SQL Server Integration
•	Angular Routing
•	Angular Services
•	Http Interceptors
•	Object-Oriented Programming (OOP)
•	Separation of Concerns
•	Client-Server Architecture

Serilog Logging Implementation Steps
I added file-based logging to the ASP.NET Core Web API project using Serilog. The purpose of this implementation is to track application activities, API requests, authentication events, and errors in daily log files.
1. Installed Required NuGet Packages
I installed the following packages:
Install-Package Serilog.AspNetCore
Install-Package Serilog.Sinks.File
2. Added Serilog Namespace
In Program.cs, I added:
using Serilog;
3. Configured Serilog Logger
Before creating the application builder, I configured Serilog to write logs into a text file inside a Logs folder:
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();
This creates a daily log file such as:
Logs/log-20260624.txt
4. Registered Serilog in the Application Host
After creating the builder, I connected Serilog with the ASP.NET Core logging system:
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

5. Used ILogger in Services and Controllers
The project already uses ILogger in different services and controllers.
Example:
private readonly ILogger<TaskService> _logger;
The logger is injected using Dependency Injection through the constructor:
public TaskService(
    AppDbContext context,
    ILogger<TaskService> logger)
{
    _context = context;
    _logger = logger;
}
6. Added Log Messages in Business Logic
I used logging methods to track important operations.
Examples:
_logger.LogInformation("Task created successfully");
_logger.LogInformation("Login successful");
_logger.LogWarning("Task not found");
_logger.LogError(ex, "Error while creating task");
7. Log File Location
After running the application, a Logs folder is created inside the project directory.
Example path:
ProjectTaskManagementAPI
Logs
log-20260624.txt
The log file can be opened using Notepad.
8. Example Log Output
Example logs generated by the application:
[INF] Now listening on: https://localhost:7167
[INF] Application started
[INF] Login successful
[INF] Task created successfully
[ERR] Error while updating task
9. Purpose of Logging
The logging system helps with:
•	Monitoring application activity
•	Tracking API requests
•	Debugging errors
•	Troubleshooting issues
•	Auditing important system operations
Summary
I implemented Serilog file logging in the ASP.NET Core Web API project to record system events and errors into daily text files. This improves debugging, monitoring, and maintainability of the application.






