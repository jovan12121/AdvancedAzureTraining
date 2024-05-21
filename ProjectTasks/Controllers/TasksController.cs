using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.DTO;
using ProjectTasks.Enums;
using ProjectTasks.Interfaces;

namespace ProjectTasks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _tasksService;
        private readonly IFilesService _filesService;
        private readonly IRabbitMQMessagingService _rabbitMQMessagingService;
        public TasksController(ITasksService tasksService, IFilesService filesService, IRabbitMQMessagingService rabbitMQMessagingService)
        {
            _tasksService = tasksService;
            _filesService = filesService;
            _rabbitMQMessagingService = rabbitMQMessagingService;
        }

        [HttpGet("getTask/{id}")]
        public async Task<IActionResult> GetTaskAsync(long id)
        {
            _rabbitMQMessagingService.PublishMessage($"Called get task with taskId:{id}.");
            return Ok(await _tasksService.GetTaskAsync(id));

        }
        [HttpGet("getTasksFromProject/{projectId}")]
        public async Task<IActionResult> GetTasksFromProject(long projectId)
        {
            _rabbitMQMessagingService.PublishMessage($"Called get tasks from project projectId:{projectId}.");
            return Ok(await _tasksService.GetAllTasksFromProjectAsync(projectId));

        }
        [HttpDelete("deleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            _rabbitMQMessagingService.PublishMessage($"Called delete task with taskId:{id}.");
            return Ok(await _tasksService.DeleteTaskAsync(id));

        }
        [HttpPut("updateTask")]
        public async Task<IActionResult> UpdateTask(EditTaskDTO editTaskDTO)
        {
            _rabbitMQMessagingService.PublishMessage($"Called update task with taskId:{editTaskDTO.Id}.");
            return Ok(await _tasksService.UpdateTaskAsync(editTaskDTO));

        }
        [HttpPost("addTask")]
        public async Task<IActionResult> AddTask(AddTaskDTO addTaskDTO)
        {
            _rabbitMQMessagingService.PublishMessage($"Called add task.");
            return Ok(await _tasksService.AddTaskAsync(addTaskDTO));

        }
        [HttpPost("addFileToTask/{taskId}")]
        public async Task<IActionResult> AddFileToTask(long taskId, [FromForm] IFormFile file)
        {
            _rabbitMQMessagingService.PublishMessage($"Called add file to task with taskId:{taskId}.");
            return Ok(await _filesService.AddFileToTaskAsync(file, taskId));
        }
        [HttpGet("downloadFilesFromTask/{taskId}")]
        public async Task<IActionResult> GetFilesFromTask(long taskId)
        {
            _rabbitMQMessagingService.PublishMessage($"Called get files from task with taskId:{taskId}.");
            var memoryStream = await _filesService.DownloadFilesFromProjectAsync(taskId);
            return File(memoryStream, "application/zip", $"task_{taskId}_files.zip");
        }
        [HttpDelete("deleteFileFromTask")]
        public async Task<IActionResult> DeleteFileFromTask(long fileId, long taskId)
        {
            _rabbitMQMessagingService.PublishMessage($"Called delete file from task with taskId:{taskId}, fileId: {fileId}.");
            return Ok(await _filesService.DeleteFileFromTaskAsync(fileId,taskId));
        }
        [HttpPatch("{taskId}/changeStatus")]
        public async Task<IActionResult> UpdateTaskStatus(long taskId, [FromBody]TaskStatus_ status)
        {
            _rabbitMQMessagingService.PublishMessage($"Changing status of task {taskId} to {status}.");
            return Ok(await _tasksService.UpdateTaskStatusAsync(taskId, status));
        }


    }
}
