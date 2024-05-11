using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ProjectTasks.DTO;
using ProjectTasks.Interfaces;

namespace ProjectTasks.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectsService;
        private readonly IFilesService _filesService;
        static readonly string[] scopeRequiredByApi = new string[] { "ReadWriteAccess" };
        public ProjectsController(IProjectsService projectsService, IFilesService filesService)
        {
            _projectsService = projectsService;
            _filesService = filesService;
        }

        [HttpGet("getProjects")]

        public async Task<IActionResult> GetAllProjects()
        {

            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            return Ok(await _projectsService.GetProjectsAsync());
        }
        [HttpGet("getProject/{id}")]
        public async Task<IActionResult> GetAllProjects(long id)
        {

            return Ok(await _projectsService.GetProjectAsync(id));

        }
        [HttpPut("updateProject")]
        public async Task<IActionResult> UpdateProject(EditProjectDTO editProjectDTO)
        {

            return Ok(await _projectsService.UpdateProjectAsync(editProjectDTO));


        }
        [HttpDelete("deleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(long id)
        {

            return Ok(await _projectsService.DeleteProjectAsync(id));

        }
        [HttpPost("addProject")]
        public async Task<IActionResult> AddProject(AddProjectDTO addProjectDTO)
        {

            return Ok(await _projectsService.AddProjectAsync(addProjectDTO));

        }
        [HttpPost("addFileToProject/{projectId}")]
        public async Task<IActionResult> AddFileToProject(long projectId,[FromForm]IFormFile file)
        {
            return Ok(await _filesService.AddFileToProjectAsync(file,projectId));
        }
        [HttpGet("getFilesFromProject/{projectId}")]
        public async Task<IActionResult> GetFilesFromProject(long projectId)
        {
            var memoryStream = await _filesService.DownloadFilesFromProjectAsync(projectId);
            return File(memoryStream, "application/zip", $"project_{projectId}_files.zip");
        }
        [HttpDelete("deleteFileFromProject")]
        public async Task<IActionResult> DeleteFileFromTask(long fileId, long projectId)
        {
            return Ok(await _filesService.DeleteFileFromProjectAsync(fileId, projectId));
        }
    }
}
