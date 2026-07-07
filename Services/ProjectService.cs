using Microsoft.EntityFrameworkCore;
using ProjectTaskManagementAPI.Data;
using ProjectTaskManagementAPI.DTOs;
using ProjectTaskManagementAPI.Interfaces;
using ProjectTaskManagementAPI.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ProjectTaskManagementAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProjectService> _logger;
        private readonly IDistributedCache _cache;

        public ProjectService(
            AppDbContext context,
            ILogger<ProjectService> logger,
            IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }


        // Get all projects
        public async Task<List<Project>> GetAllProjectsAsync()
        {
            const string cacheKey = "all_projects";

            try
            {
                // Check if data exists in Redis
                var cachedProjects = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedProjects))
                {
                    _logger.LogInformation("Projects retrieved from Redis cache");

                    return JsonSerializer.Deserialize<List<Project>>(cachedProjects)!;
                }

                // Retrieve from SQL Server
                var projects = await _context.Projects.ToListAsync();

                // Store in Redis for 5 minutes
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(projects),
                    options);

                _logger.LogInformation("Projects retrieved from database and stored in Redis");

                return projects;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving projects");

                throw;
            }
        }

        // Get project by id
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

        // Create a new project
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
                await _cache.RemoveAsync("all_projects");

                _logger.LogInformation("Project created successfully");

                return "Project created successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating project");

                throw;
            }
        }

        // Update an existing project
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
                await _cache.RemoveAsync("all_projects");

                _logger.LogInformation("Project updated successfully");

                return "Project updated successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating project");
                throw;
            }
        }

        // Delete a project
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

                
                await _cache.RemoveAsync("all_projects");

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