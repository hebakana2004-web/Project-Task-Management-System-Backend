using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManagementAPI.DTOs;
using ProjectTaskManagementAPI.Interfaces;

namespace ProjectTaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(
            ITaskService taskService,
            ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting tasks");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);

                if (task == null)
                    return NotFound(new { Success = false, Message = "Task not found" });

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting task by id");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTask(TaskDto dto)
        {
            try
            {
                var result = await _taskService.CreateTaskAsync(dto);

                if (result == "Project not found" || result == "User not found")
                    return BadRequest(new { Success = false, Message = result });

                return Ok(new { Success = true, Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating task");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTask(int id, TaskDto dto)
        {
            try
            {
                var result = await _taskService.UpdateTaskAsync(id, dto);

                if (result == "Task not found")
                    return NotFound(new { Success = false, Message = result });

                if (result == "Project not found" || result == "User not found")
                    return BadRequest(new { Success = false, Message = result });

                return Ok(new { Success = true, Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating task");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] string status)
        {
            try
            {
                var result = await _taskService.UpdateTaskStatusAsync(id, status);

                if (result == "Task not found")
                    return NotFound(new { Success = false, Message = result });

                return Ok(new { Success = true, Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating task status");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(id);

                if (result == "Task not found")
                    return NotFound(new { Success = false, Message = result });

                return Ok(new { Success = true, Message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting task");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred." });
            }
        }
    }
}