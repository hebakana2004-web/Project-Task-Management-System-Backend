using Microsoft.EntityFrameworkCore;
using ProjectTaskManagementAPI.Data;
using ProjectTaskManagementAPI.DTOs;
using ProjectTaskManagementAPI.Interfaces;
using ProjectTaskManagementAPI.Models;

namespace ProjectTaskManagementAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(
            AppDbContext context,
            ILogger<ProjectService> logger)
        {
            _context = context;
            _logger = logger;
        }

  
        public async Task<List<Project>> GetAllProjectsAsync()
        {
            try
            {
                var projects = await _context.Projects.ToListAsync();

                _logger.LogInformation("Projects retrieved successfully");

                return projects;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving projects");

                throw;
            }
        }
        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null)
                {
                    _logger.LogWarning("Project not found");
                    return null;
                }

                _logger.LogInformation("Project retrieved successfully");

                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving project by id");
                throw;
            }
        }

        public async Task<string> CreateProjectAsync(ProjectDto dto)
        {
            try
            {
                var project = new Project
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    CreatedDate = DateTime.Now
                };

                await _context.Projects.AddAsync(project);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Project created successfully");

                return "Project created successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating project");

                throw;
            }
        }

        public async Task<string> UpdateProjectAsync(int id, ProjectDto dto)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null)
                {
                    _logger.LogWarning("Project not found for update");
                    return "Project not found";
                }

                project.Name = dto.Name;
                project.Description = dto.Description;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Project updated successfully");

                return "Project updated successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating project");
                throw;
            }
        }

        public async Task<string> DeleteProjectAsync(int id)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null)
                {
                    _logger.LogWarning("Project not found for delete");
                    return "Project not found";
                }

                _context.Projects.Remove(project);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Project deleted successfully");

                return "Project deleted successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting project");
                throw;
            }
        }
    }
}