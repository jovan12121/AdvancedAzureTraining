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
        static readonly string[] scopeRequiredByApi = new string[] { "ReadWriteAccess" };
        public ProjectsController(IProjectsService projectsService)
        {
            _projectsService = projectsService;
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
    }
}
