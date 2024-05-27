using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ProjectTaskCosmos.Interfaces;

namespace ProjectTaskCosmos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _tasksService;
        static readonly string[] scopeRequiredByApi = new string[] { "ReadAccess" };

        public TasksController(ITasksService tasksService)
        {
               _tasksService = tasksService;
        }
        [Authorize(Roles = "Reader.Read")]

        [HttpGet("getTask/{id}")]
        public async Task<IActionResult> GetTaskAsync(long id)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            try
            {
                return Ok(await _tasksService.GetTaskAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize(Roles = "Reader.Read")]

        [HttpGet("getTasksFromProject/{projectId}")]
        public async Task<IActionResult> GetTasksFromProject(long projectId)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            try
            {
                return Ok(await _tasksService.GetAllTasksFromProjectAsync(projectId));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
