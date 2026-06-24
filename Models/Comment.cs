namespace ProjectTaskManagementAPI.Models
{
    public class Comment // Represents a comment on a task in the task management system
    {
        public int Id { get; set; } // Primary key  

        public string Text { get; set; } = string.Empty; // Comment text

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Comment creation date

        public int TaskItemId { get; set; } // Foreign key to the associated task

        public int UserId { get; set; } // Foreign key to the user who made the comment
    }
}
