using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.DTO;
using ProjectTasks.Interfaces;

namespace ProjectTasks.Controllers
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
        [HttpDelete("deleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            try
            {
                return Ok(await _tasksService.DeleteTaskAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("updateTask")]
        public async Task<IActionResult> UpdateTask(EditTaskDTO editTaskDTO)
        {
            try
            {
                return Ok(await _tasksService.UpdateTaskAsync(editTaskDTO));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("addTask")]
        public async Task<IActionResult> AddTask(AddTaskDTO addTaskDTO)
        {
            try
            {
                return Ok(await _tasksService.AddTaskAsync(addTaskDTO));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }
}
