namespace ProjectTaskManagementAPI.DTOs
{
    public class TaskDto
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "To Do";

        public string Priority { get; set; } = "Medium";

        public int ProjectId { get; set; }

        public int AssignedUserId { get; set; }
    }
}