using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ProjectTasks.DTO;
using ProjectTasks.Enums;
using ProjectTasks.Interfaces;
using RabbitMQ.Client;

namespace ProjectTasks.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectsService;
        private readonly IFilesService _filesService;
        private readonly IRabbitMQMessagingService _rabbitMQMessagingService;
        static readonly string[] scopeRequiredByApi = new string[] { "ReadWriteAccess" };
        public ProjectsController(IProjectsService projectsService, IFilesService filesService, IRabbitMQMessagingService rabbitMQMessagingService)
        {
            _projectsService = projectsService;
            _filesService = filesService;
            _rabbitMQMessagingService = rabbitMQMessagingService;
        }

        [HttpGet("getProjects")]

        public async Task<IActionResult> GetAllProjects()
        {

            //HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            _rabbitMQMessagingService.PublishMessage("Called get all projects.");
            return Ok(await _projectsService.GetProjectsAsync());
        }
        [HttpGet("getProject/{id}")]
        public async Task<IActionResult> GetAllProjects(long id)
        {
            _rabbitMQMessagingService.PublishMessage($"Called get project with id: {id}.");
            return Ok(await _projectsService.GetProjectAsync(id));

        }
        [HttpPut("updateProject")]
        public async Task<IActionResult> UpdateProject(EditProjectDTO editProjectDTO)
        {
            _rabbitMQMessagingService.PublishMessage($"Called update for project wit id: {editProjectDTO.Id}.");
            return Ok(await _projectsService.UpdateProjectAsync(editProjectDTO));
        }
        [HttpDelete("deleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(long id)
        {
            _rabbitMQMessagingService.PublishMessage($"Called delete with id: {id}.");
            return Ok(await _projectsService.DeleteProjectAsync(id));
        }
        [HttpPost("addProject")]
        public async Task<IActionResult> AddProject(AddProjectDTO addProjectDTO)
        {
            _rabbitMQMessagingService.PublishMessage($"Called add project.");
            return Ok(await _projectsService.AddProjectAsync(addProjectDTO));

        }
        [HttpPost("addFileToProject/{projectId}")]
        public async Task<IActionResult> AddFileToProject(long projectId,[FromForm]IFormFile file)
        {
            _rabbitMQMessagingService.PublishMessage($"Called add file to project with projectId: {projectId}.");
            return Ok(await _filesService.AddFileToProjectAsync(file,projectId));
        }
        [HttpGet("getFilesFromProject/{projectId}")]
        public async Task<IActionResult> GetFilesFromProject(long projectId)
        {
            _rabbitMQMessagingService.PublishMessage($"Called get files from project with projectId: {projectId}.");
            var memoryStream = await _filesService.DownloadFilesFromProjectAsync(projectId);
            return File(memoryStream, "application/zip", $"project_{projectId}_files.zip");
        }
        [HttpDelete("deleteFileFromProject")]
        public async Task<IActionResult> DeleteFileFromTask(long fileId, long projectId)
        {
            _rabbitMQMessagingService.PublishMessage($"Called delete file from project with projectId: {projectId}, fileId: {fileId}.");
            return Ok(await _filesService.DeleteFileFromProjectAsync(fileId, projectId));
        }
        [HttpPatch("{projectId}/updateStatus")]
        public async Task<IActionResult> UpdateStatus(int projectId, [FromBody] ProjectStatus status)
        {
            _rabbitMQMessagingService.PublishMessage($"Changing status of project {projectId} to {status}.");
            return Ok(await _projectsService.UpdateProjectStatusAsync(projectId, status));
        }
    }
}
