using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskCosmos.Interfaces;

namespace ProjectTaskCosmos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectsService;
        private readonly IFilesService _filesService;
        public ProjectsController(IProjectsService projectsService, IFilesService filesService)
        {
            _projectsService = projectsService;
            _filesService = filesService;
        }
        [HttpGet("getProjects")]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                return Ok(await _projectsService.GetProjectsAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("getProject/{id}")]
        public async Task<IActionResult> GetAllProjects(long id)
        {
            try
            {
                return Ok(await _projectsService.GetProjectAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
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
