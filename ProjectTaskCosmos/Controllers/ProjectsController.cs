using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ProjectTaskCosmos.Interfaces;

namespace ProjectTaskCosmos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectsService;
        private readonly IFilesService _filesService;
        static readonly string[] scopeRequiredByApi = new string[] { "ReadAccess" };

        public ProjectsController(IProjectsService projectsService, IFilesService filesService)
        {
            _projectsService = projectsService;
            _filesService = filesService;
        }
        [Authorize(Roles = "Reader.Read")]
        [HttpGet("getProjects")]
        public async Task<IActionResult> GetAllProjects()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            try
            {
                return Ok(await _projectsService.GetProjectsAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize(Roles = "Reader.Read")]

        [HttpGet("getProject/{id}")]
        public async Task<IActionResult> GetAllProjects(long id)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            try
            {
                return Ok(await _projectsService.GetProjectAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize(Roles = "Reader.Read")]

        [HttpGet("getFilesFromProject/{id}")]
        public async Task<IActionResult> GetFilesFromProject(long id)
        {
            try
            {
                return Ok(await _filesService.DownloadFilesFromProjectAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize(Roles = "Reader.Read")]

        [HttpGet("getFilesFromTask/{id}")]
        public async Task<IActionResult> GetFilesFromTask(long id)
        {
            try
            {
                return Ok(await _filesService.DownloadFilesFromTaskAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
