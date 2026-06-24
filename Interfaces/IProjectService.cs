using ProjectTaskManagementAPI.DTOs;
using ProjectTaskManagementAPI.Models;

namespace ProjectTaskManagementAPI.Interfaces
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllProjectsAsync();

        Task<Project?> GetProjectByIdAsync(int id);

        Task<string> CreateProjectAsync(ProjectDto dto);

        Task<string> UpdateProjectAsync(int id, ProjectDto dto);

        Task<string> DeleteProjectAsync(int id);
    }
}
// The IProjectService interface defines methods for managing projects, including retrieving all projects, getting a project by ID, creating a new project, updating an existing project, and deleting a project. Each method returns a Task, allowing for asynchronous execution.
//This is the CRUD contract for projects.