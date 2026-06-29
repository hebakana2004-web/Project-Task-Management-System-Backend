using Microsoft.EntityFrameworkCore;
using ProjectTaskManagementAPI.Data;
using ProjectTaskManagementAPI.DTOs;
using ProjectTaskManagementAPI.Interfaces;
using ProjectTaskManagementAPI.Models;

namespace ProjectTaskManagementAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            AppDbContext context,
            ILogger<TaskService> logger)
        {
            _context = context;
            _logger = logger;
        }


        // Get all tasks
        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            try
            {
                var tasks = await _context.TaskItems
                    .Include(t => t.Project)
                    .Include(t => t.AssignedUser)
                    .ToListAsync();

                _logger.LogInformation("Tasks retrieved successfully");

                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving tasks");
                throw;
            }
        }

        // Get task by id
        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            try
            {
                var task = await _context.TaskItems
                    .Include(t => t.Project)
                    .Include(t => t.AssignedUser)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (task == null)
                {
                    _logger.LogWarning("Task not found");
                    return null;
                }

                _logger.LogInformation("Task retrieved successfully");

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving task by id");
                throw;
            }
        }

        // Create a new task
        public async Task<string> CreateTaskAsync(TaskDto dto)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);

                if (project == null)
                {
                    return "Project not found";
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.AssignedUserId);

                if (user == null)
                {
                    return "User not found";
                }

                var task = new TaskItem
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Status = dto.Status,
                    Priority = dto.Priority,
                    CreatedDate = DateTime.Now,
                    ProjectId = dto.ProjectId,
                    AssignedUserId = dto.AssignedUserId
                };

                await _context.TaskItems.AddAsync(task);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Task created successfully");

                return "Task created successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating task");

                throw;
            }
        }

        // Update an existing task
        public async Task<string> UpdateTaskAsync(int id, TaskDto dto)
        {
            try
            {
                var task = await _context.TaskItems
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (task == null)
                {
                    return "Task not found";
                }

                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);

                if (project == null)
                {
                    return "Project not found";
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.AssignedUserId);

                if (user == null)
                {
                    return "User not found";
                }

                task.Title = dto.Title;
                task.Description = dto.Description;
                task.Status = dto.Status;
                task.Priority = dto.Priority;
                task.ProjectId = dto.ProjectId;
                task.AssignedUserId = dto.AssignedUserId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Task updated successfully");

                return "Task updated successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating task");
                throw;
            }
        }

        // Update task status
        public async Task<string> UpdateTaskStatusAsync(int id, string status)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
                return "Task not found";

            task.Status = status;

            await _context.SaveChangesAsync();

            return "Task status updated successfully";
        }

        // Delete a task
        public async Task<string> DeleteTaskAsync(int id)
        {
            try
            {
                var task = await _context.TaskItems
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (task == null)
                {
                    return "Task not found";
                }

                _context.TaskItems.Remove(task);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Task deleted successfully");

                return "Task deleted successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting task");
                throw;
            }
        }
    }
}