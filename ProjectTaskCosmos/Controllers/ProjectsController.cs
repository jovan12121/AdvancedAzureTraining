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
        public ProjectsController(IProjectsService projectsService) 
        {
            _projectsService = projectsService;
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
    }
}
