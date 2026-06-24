using ProjectTaskManagementAPI.DTOs;
using ProjectTaskManagementAPI.Models;

namespace ProjectTaskManagementAPI.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllTasksAsync();

        Task<TaskItem?> GetTaskByIdAsync(int id);

        Task<string> CreateTaskAsync(TaskDto dto);

        Task<string> UpdateTaskAsync(int id, TaskDto dto);

        Task<string> DeleteTaskAsync(int id);
        Task<string> UpdateTaskStatusAsync(int id, string status);
    }
}