namespace ProjectTaskManagementAPI.Models
{
    public class TaskItem // Represents a task in the task management system
    {
        public int Id { get; set; } // Primary key      

        public string Title { get; set; } = string.Empty; // Task title

        public string Description { get; set; } = string.Empty; // Task description     

        public string Status { get; set; } = "To Do"; // Task status

        public string Priority { get; set; } = "Medium"; // Task priority

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Task creation date     

        public int ProjectId { get; set; } // Foreign key to the associated project

        public Project? Project { get; set; } // Navigation property to the associated project

        public int AssignedUserId { get; set; } // Foreign key to the assigned user

        public User? AssignedUser { get; set; } // Navigation property to the assigned user
    }
}
