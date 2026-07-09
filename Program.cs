using Microsoft.EntityFrameworkCore;
using ProjectTaskManagementAPI.Data;
using ProjectTaskManagementAPI.Interfaces;
using ProjectTaskManagementAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;// We use it to log information, warnings, and errors,We use it to record events.

using Microsoft.OpenApi.Models;




// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();


// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);// Creates a new instance of the WebApplicationBuilder class, which is used to configure and build the web application. The args parameter represents command-line arguments passed to the application.

// Configure Serilog
builder.Host.UseSerilog();// Configures the application to use Serilog for logging. It replaces the default logging provider with Serilog, which allows for more advanced logging capabilities, such as structured logging and writing logs to various sinks (e.g., files, databases, etc.).


// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// Add services to the container
builder.Services.AddControllers();


// Configure Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "ProjectTaskManagement";
});


//It enables API endpoint discovery so that Swagger can automatically generate API documentation and list all available endpoints.
//Because Swagger needs to know all the endpoints in the project in order to display and test them.
builder.Services.AddEndpointsApiExplorer();



// Swagger Configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LoginPage API",
        Version = "v1"
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    // Add a security requirement to indicate that the API requires authentication
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));// Configures the application to use SQL Server with the connection string specified in the configuration (e.g., appsettings.json) under "DefaultConnection". This allows the application to connect to the database and perform CRUD operations on the defined models (TaskItem, Comment, Project, User) through Entity Framework Core.

// JWT Authentication Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters 
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Dependency Injection for Services
builder.Services.AddScoped<IAuthService, AuthService>();// Registers the AuthService class as the implementation of the IAuthService interface in the dependency injection container. This allows the application to inject an instance of AuthService wherever IAuthService is required, promoting loose coupling and easier testing.
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();


var app = builder.Build();// Builds the application, which will be used to configure the HTTP request pipeline and run the application.

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AngularPolicy");// Enables CORS for the specified policy

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();// Maps the controller routes to the application, allowing it to handle incoming HTTP requests and route them to the appropriate controller actions.


// Apply database migrations on startup
//This code automatically applies Entity Framework Core migrations when the application starts. 
//Since the API and SQL Server start together in Docker, it also retries the database connection until SQL Server is ready, preventing startup failures.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retries = 10;

    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch
        {
            retries--;
            Thread.Sleep(5000);
        }
    }
}

app.Run();
