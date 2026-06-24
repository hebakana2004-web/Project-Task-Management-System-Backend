using Microsoft.EntityFrameworkCore;
using ProjectTaskManagementAPI.Data;
using ProjectTaskManagementAPI.Interfaces;
using ProjectTaskManagementAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;// We use it to log information, warnings, and errors,We use it to record events.

Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);// Creates a new instance of the WebApplicationBuilder class, which is used to configure and build the web application. The args parameter represents command-line arguments passed to the application.
// Allow Angular frontend to access the API
builder.Host.UseSerilog();// Configures the application to use Serilog for logging. It replaces the default logging provider with Serilog, which allows for more advanced logging capabilities, such as structured logging and writing logs to various sinks (e.g., files, databases, etc.).

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

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));// Configures the application to use SQL Server with the connection string specified in the configuration (e.g., appsettings.json) under "DefaultConnection". This allows the application to connect to the database and perform CRUD operations on the defined models (TaskItem, Comment, Project, User) through Entity Framework Core.
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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();// Builds the application, which will be used to configure the HTTP request pipeline and run the application.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularPolicy");// Enables CORS for the specified policy
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
