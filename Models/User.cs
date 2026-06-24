namespace ProjectTaskManagementAPI.Models
{
    public class User // Represents a user in the task management system
    {
        public int Id { get; set; } // Primary key

        public string FullName { get; set; } = string.Empty; // User's full name

        public string Email { get; set; } = string.Empty; // User's email address

        public string PasswordHash { get; set; } = string.Empty; // Hashed password

        public string Role { get; set; } = "User"; // User's role

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<TaskItem>? Tasks { get; set; } // User's tasks
    }
}
