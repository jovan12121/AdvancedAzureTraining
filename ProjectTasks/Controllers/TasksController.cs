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
        private readonly IFilesService _filesService;
        public TasksController(ITasksService tasksService, IFilesService filesService)
        {
            _tasksService = tasksService;
            _filesService = filesService;
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
        [HttpPost("addFileToTask/{taskId}")]
        public async Task<IActionResult> AddFileToTask(long taskId, [FromForm] IFormFile file)
        {
            return Ok(await _filesService.AddFileToTaskAsync(file, taskId));
        }
        [HttpGet("downloadFilesFromTask/{taskId}")]
        public async Task<IActionResult> GetFilesFromTask(long taskId)
        {
            var memoryStream = await _filesService.DownloadFilesFromProjectAsync(taskId);
            return File(memoryStream, "application/zip", $"task_{taskId}_files.zip");
        }
        [HttpDelete("deleteFileFromTask")]
        public async Task<IActionResult> DeleteFileFromTask(long fileId, long taskId)
        {
            return Ok(await _filesService.DeleteFileFromTaskAsync(fileId,taskId));
        }


    }
}
