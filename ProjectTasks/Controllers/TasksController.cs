using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
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
        static readonly string[] scopeRequiredByApi = new string[] { "ReadWriteAccess" };

        public TasksController(ITasksService tasksService, IFilesService filesService, IRabbitMQMessagingService rabbitMQMessagingService)
        {
            _tasksService = tasksService;
            _filesService = filesService;
            _rabbitMQMessagingService = rabbitMQMessagingService;
        }
        [Authorize(Policy = "AdminPolicy")]

        [HttpGet("getTask/{id}")]
        public async Task<IActionResult> GetTaskAsync(long id)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            _rabbitMQMessagingService.PublishMessage($"Called get task with taskId:{id}.");
            return Ok(await _tasksService.GetTaskAsync(id));

        }
        [Authorize(Policy = "AdminPolicy")]

        [HttpGet("getTasksFromProject/{projectId}")]
        public async Task<IActionResult> GetTasksFromProject(long projectId)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            _rabbitMQMessagingService.PublishMessage($"Called get tasks from project projectId:{projectId}.");
            return Ok(await _tasksService.GetAllTasksFromProjectAsync(projectId));

        }
        [Authorize(Policy = "AdminPolicy")]

        [HttpDelete("deleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            _rabbitMQMessagingService.PublishMessage($"Called delete task with taskId:{id}.");
            return Ok(await _tasksService.DeleteTaskAsync(id));

        }
        [Authorize(Policy = "AdminPolicy")]

        [HttpPut("updateTask")]
        public async Task<IActionResult> UpdateTask(EditTaskDTO editTaskDTO)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            _rabbitMQMessagingService.PublishMessage($"Called update task with taskId:{editTaskDTO.Id}.");
            return Ok(await _tasksService.UpdateTaskAsync(editTaskDTO));

        }
                    
        [HttpPost("addTask")]
        public async Task<IActionResult> AddTask(AddTaskDTO addTaskDTO)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            _rabbitMQMessagingService.PublishMessage($"Called add task.");
            return Ok(await _tasksService.AddTaskAsync(addTaskDTO));

        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("addFileToTask/{taskId}")]
        public async Task<IActionResult> AddFileToTask(long taskId,IFormFile file)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            _rabbitMQMessagingService.PublishMessage($"Called add file to task with taskId:{taskId}.");
            return Ok(await _filesService.AddFileToTaskAsync(file, taskId));
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("downloadFilesFromTask/{taskId}")]
        public async Task<IActionResult> GetFilesFromTask(long taskId)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            _rabbitMQMessagingService.PublishMessage($"Called get files from task with taskId:{taskId}.");
            var memoryStream = await _filesService.DownloadFilesFromProjectAsync(taskId);
            return File(memoryStream, "application/zip", $"task_{taskId}_files.zip");
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("deleteFileFromTask")]
        public async Task<IActionResult> DeleteFileFromTask(long fileId, long taskId)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            _rabbitMQMessagingService.PublishMessage($"Called delete file from task with taskId:{taskId}, fileId: {fileId}.");
            return Ok(await _filesService.DeleteFileFromTaskAsync(fileId,taskId));
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpPatch("{taskId}/changeStatus")]
        public async Task<IActionResult> UpdateTaskStatus(long taskId, [FromBody]TaskStatus_ status)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            _rabbitMQMessagingService.PublishMessage($"Changing status of task {taskId} to {status}.");
            return Ok(await _tasksService.UpdateTaskStatusAsync(taskId, status));
        }


    }
}
