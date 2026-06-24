using Microsoft.EntityFrameworkCore;
using ProjectTaskManagementAPI.Models;

namespace ProjectTaskManagementAPI.Data
{
    public class AppDbContext : DbContext // Represents the database context for the task management system
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) // Constructor that accepts DbContextOptions and passes it to the base class constructor
        {
        }


        public DbSet<TaskItem> TaskItems { get; set; } // Represents the collection of task items in the database
        public DbSet<Comment> Comments { get; set; } // Represents the collection of comments in the database
        public DbSet<Project> Projects { get; set; } // Represents the collection of projects in the database
        public DbSet<User> Users { get; set; } // Represents the collection of users in the database

        protected override void OnModelCreating(ModelBuilder modelBuilder) // Configures the model relationships and constraints
        {
            base.OnModelCreating(modelBuilder);     // Calls the base method to ensure any default configurations are applied
        }
    }
}
//This means Entity Framework will convert the models into tables within SQL Server. 

//The DbSet properties represent the collections of entities that will be stored in the database.
//The OnModelCreating method can be used to configure relationships, constraints, and other aspects of the model, but in this case, it simply calls the base implementation.
