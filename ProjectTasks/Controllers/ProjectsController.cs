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
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("getProjects")]

        public async Task<IActionResult> GetAllProjects()
        {

            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            _rabbitMQMessagingService.PublishMessage("Called get all projects.");
            return Ok(await _projectsService.GetProjectsAsync());
        }
        [Authorize(Roles = "Admin.Admin")]
        [HttpGet("getProject/{id}")]
        public async Task<IActionResult> GetAllProjects(long id)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            //_rabbitMQMessagingService.PublishMessage($"Called get project with id: {id}.");
            return Ok(await _projectsService.GetProjectAsync(id));

        }
        [Authorize(Roles = "Admin.Admin")]

        [HttpPut("updateProject")]
        public async Task<IActionResult> UpdateProject(EditProjectDTO editProjectDTO)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            //_rabbitMQMessagingService.PublishMessage($"Called update for project wit id: {editProjectDTO.Id}.");
            return Ok(await _projectsService.UpdateProjectAsync(editProjectDTO));
        }
        [Authorize(Roles = "Admin.Admin")]

        [HttpDelete("deleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(long id)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            //_rabbitMQMessagingService.PublishMessage($"Called delete with id: {id}.");
            return Ok(await _projectsService.DeleteProjectAsync(id));
        }
        [Authorize(Roles = "Admin.Admin")]

        [HttpPost("addProject")]
        public async Task<IActionResult> AddProject(AddProjectDTO addProjectDTO)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            //_rabbitMQMessagingService.PublishMessage($"Called add project.");
            return Ok(await _projectsService.AddProjectAsync(addProjectDTO));

        }
        [Authorize(Roles = "Admin.Admin")]

        [HttpPost("addFileToProject/{projectId}")]
        public async Task<IActionResult> AddFileToProject(long projectId,IFormFile file)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            //_rabbitMQMessagingService.PublishMessage($"Called add file to project with projectId: {projectId}.");
            return Ok(await _filesService.AddFileToProjectAsync(file,projectId));
        }
        [Authorize(Roles = "Admin.Admin")]

        [HttpGet("getFilesFromProject/{projectId}")]
        public async Task<IActionResult> GetFilesFromProject(long projectId)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            //_rabbitMQMessagingService.PublishMessage($"Called get files from project with projectId: {projectId}.");
            var memoryStream = await _filesService.DownloadFilesFromProjectAsync(projectId);
            return File(memoryStream, "application/zip", $"project_{projectId}_files.zip");
        }
        [Authorize(Roles = "Admin.Admin")]

        [HttpDelete("deleteFileFromProject")]
        public async Task<IActionResult> DeleteFileFromTask(long fileId, long projectId)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            //_rabbitMQMessagingService.PublishMessage($"Called delete file from project with projectId: {projectId}, fileId: {fileId}.");
            return Ok(await _filesService.DeleteFileFromProjectAsync(fileId, projectId));
        }
        [Authorize(Roles = "Admin.Admin")]

        [HttpPatch("{projectId}/updateStatus")]
        public async Task<IActionResult> UpdateStatus(int projectId, [FromBody] ProjectStatus status)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            //_rabbitMQMessagingService.PublishMessage($"Changing status of project {projectId} to {status}.");
            return Ok(await _projectsService.UpdateProjectStatusAsync(projectId, status));
        }
    }
}
