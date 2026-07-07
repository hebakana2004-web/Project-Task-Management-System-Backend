using Microsoft.AspNetCore.Mvc;
using ProjectTaskManagementAPI.Interfaces;
using ProjectTaskManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ProjectTaskManagementAPI.Controllers
{
    [Route("api/[controller]")]//api/project
    [ApiController]
    [Authorize]//This means no one can access this Controller unless they have a valid token.
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(
            IProjectService projectService,
            ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        // GET: api/project
        [HttpGet]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();

                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting projects");

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                });
            }
        }

        // GET: api/project/{id}
        [HttpGet("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);

                if (project == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Project not found"
                    });
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting project by id");

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                });
            }
        }

        // POST: api/project
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProject(ProjectDto dto)
        {
            try
            {
                var result = await _projectService.CreateProjectAsync(dto);

                return Ok(new
                {
                    Success = true,
                    Message = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating project");

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                });
            }
        }

        // PUT: api/project/{id}
        [HttpPut("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProject(int id, ProjectDto dto)
        {
            try
            {
                var result = await _projectService.UpdateProjectAsync(id, dto);

                if (result == "Project not found")
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = result
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating project");

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                });
            }
        }

        // DELETE: api/project/{id}
        [HttpDelete("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var result = await _projectService.DeleteProjectAsync(id);

                if (result == "Project not found")
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = result
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting project");

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                });
            }
        }
    }


}