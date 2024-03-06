using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskCosmos.Interfaces;

namespace ProjectTaskCosmos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _tasksService;
        public TasksController(ITasksService tasksService)
        {
               _tasksService = tasksService;
        }
        [HttpGet("getTask/{id}")]
        public async Task<IActionResult> GetTaskAsync(long id)
        {
            try
            {
                return Ok(await _tasksService.GetTaskAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("getTasksFromProject/{projectId}")]
        public async Task<IActionResult> GetTasksFromProject(long projectId)
        {
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
