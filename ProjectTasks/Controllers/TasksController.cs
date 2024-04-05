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

            return Ok(await _tasksService.GetTaskAsync(id));

        }
        [HttpGet("getTasksFromProject/{projectId}")]
        public async Task<IActionResult> GetTasksFromProject(long projectId)
        {

            return Ok(await _tasksService.GetAllTasksFromProjectAsync(projectId));

        }
        [HttpDelete("deleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {

            return Ok(await _tasksService.DeleteTaskAsync(id));

        }
        [HttpPut("updateTask")]
        public async Task<IActionResult> UpdateTask(EditTaskDTO editTaskDTO)
        {

            return Ok(await _tasksService.UpdateTaskAsync(editTaskDTO));

        }
        [HttpPost("addTask")]
        public async Task<IActionResult> AddTask(AddTaskDTO addTaskDTO)
        {

            return Ok(await _tasksService.AddTaskAsync(addTaskDTO));

        }


    }
}
