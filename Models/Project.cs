namespace ProjectTaskManagementAPI.Models
{
    public class Project // Represents a project in the task management system
    {
        public int Id { get; set; } // Primary key

        public string Name { get; set; } = string.Empty; // Project name

        public string Description { get; set; } = string.Empty; // Project description

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Project creation date
        
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<TaskItem>? Tasks { get; set; } // Project tasks
    }
}
