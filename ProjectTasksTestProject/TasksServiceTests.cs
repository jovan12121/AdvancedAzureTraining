using Moq;
using ProjectTasks.DTO;
using ProjectTasks.Enums;
using ProjectTasks.Interfaces;
using ProjectTasks.Model;
using ProjectTasks.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTasksTestProject
{
    [TestClass]
    public class TasksServiceTests
    {
        [TestMethod]
        public async Task AddTaskAsync_ValidProjectId_ReturnsTask()
        {
            var repositoryMock = new Mock<IProjectTaskRepository>();
            repositoryMock.Setup(repo => repo.GetProjectAsync(It.IsAny<long>())).ReturnsAsync(new Project());
            repositoryMock.Setup(repo => repo.AddTaskAsync(It.IsAny<Task_>())).ReturnsAsync(new Task_ // Ensure AddTaskAsync returns the added task
            {
                Id = 1,
                TaskDescription = "Test Description",
                TaskName = "Test Task",
                Status = TaskStatus_.IN_PROGRESS
            });
            var rabbitmqMessagingServiceMock = new Mock<IRabbitMQMessagingService>();
            var service = new TasksService(repositoryMock.Object, rabbitmqMessagingServiceMock.Object);
            var addTaskDTO = new AddTaskDTO
            {
                ProjectId = 1,
                TaskDescription = "Test Description",
                TaskName = "Test Task"
            };
            var result = await service.AddTaskAsync(addTaskDTO);
            Assert.IsNotNull(result);
            Assert.AreEqual(addTaskDTO.TaskDescription, result.TaskDescription);
            Assert.AreEqual(addTaskDTO.TaskName, result.TaskName);
            Assert.AreEqual(TaskStatus_.IN_PROGRESS, result.Status);
            repositoryMock.Verify(repo => repo.AddTaskAsync(It.IsAny<Task_>()), Times.Once);
        }

        [TestMethod]
        public async Task AddTaskAsync_InvalidProjectId_ThrowsException()
        {

            var repositoryMock = new Mock<IProjectTaskRepository>();
            repositoryMock.Setup(repo => repo.GetProjectAsync(It.IsAny<long>())).ReturnsAsync((Project)null); 
            var rabbitmqMessagingServiceMock = new Mock<IRabbitMQMessagingService>();
            var service = new TasksService(repositoryMock.Object, rabbitmqMessagingServiceMock.Object);
            var addTaskDTO = new AddTaskDTO
            {
                ProjectId = 1,
                TaskDescription = "Test Description",
                TaskName = "Test Task"
            };
            await Assert.ThrowsExceptionAsync<ApplicationException>(() => service.AddTaskAsync(addTaskDTO));
            rabbitmqMessagingServiceMock.Verify(rabbitmq => rabbitmq.PublishMessage("Error occured: Project with that ID doesn't exist!"), Times.Once);
        }

        [TestMethod]
        public async Task DeleteTaskAsync_ValidTaskId_DeletesTask()
        {
            var repositoryMock = new Mock<IProjectTaskRepository>();
            repositoryMock.Setup(repo => repo.DeleteTaskAsync(It.IsAny<long>())).Returns(Task.CompletedTask);
            var rabbitmqMessagingServiceMock = new Mock<IRabbitMQMessagingService>();
            var service = new TasksService(repositoryMock.Object, rabbitmqMessagingServiceMock.Object);
            var result = await service.DeleteTaskAsync(1);
            Assert.IsTrue(result);
            repositoryMock.Verify(repo => repo.DeleteTaskAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task GetAllTasksFromProjectAsync_ValidProjectId_ReturnsListOfTasks()
        {
            var repositoryMock = new Mock<IProjectTaskRepository>();
            var expectedTasks = new List<Task_> { new Task_(), new Task_() };
            repositoryMock.Setup(repo => repo.GetAllTasksFromProjectAsync(It.IsAny<long>())).ReturnsAsync(expectedTasks);
            var rabbitmqMessagingServiceMock = new Mock<IRabbitMQMessagingService>();
            var service = new TasksService(repositoryMock.Object, rabbitmqMessagingServiceMock.Object);
            var result = await service.GetAllTasksFromProjectAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTasks.Count, result.Count);
            repositoryMock.Verify(repo => repo.GetAllTasksFromProjectAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task GetTaskAsync_ExistingTaskId_ReturnsTask()
        {
            var repositoryMock = new Mock<IProjectTaskRepository>();
            var expectedTask = new Task_();
            repositoryMock.Setup(repo => repo.GetTaskAsync(It.IsAny<long>())).ReturnsAsync(expectedTask);
            var rabbitmqMessagingServiceMock = new Mock<IRabbitMQMessagingService>();
            var service = new TasksService(repositoryMock.Object, rabbitmqMessagingServiceMock.Object);
            var result = await service.GetTaskAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTask, result);
            repositoryMock.Verify(repo => repo.GetTaskAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task UpdateTaskAsync_ExistingTaskId_UpdatesTask()
        {
            var repositoryMock = new Mock<IProjectTaskRepository>();
            var taskToUpdate = new Task_ { Id = 1, TaskDescription = "Old Description", TaskName = "Old Task Name" };
            var editTaskDTO = new EditTaskDTO { Id = 1, TaskDescription = "New Description", TaskName = "New Task Name" };
            repositoryMock.Setup(repo => repo.GetTaskAsync(It.IsAny<long>())).ReturnsAsync(taskToUpdate);
            repositoryMock.Setup(repo => repo.UpdateTaskAsync(It.IsAny<Task_>())).ReturnsAsync(taskToUpdate); 
            var rabbitmqMessagingServiceMock = new Mock<IRabbitMQMessagingService>();
            var service = new TasksService(repositoryMock.Object, rabbitmqMessagingServiceMock.Object);
            var result = await service.UpdateTaskAsync(editTaskDTO);
            Assert.IsNotNull(result);
            Assert.AreEqual(editTaskDTO.TaskDescription, result.TaskDescription);
            Assert.AreEqual(editTaskDTO.TaskName, result.TaskName);
            repositoryMock.Verify(repo => repo.UpdateTaskAsync(It.Is<Task_>(t => t.Id == taskToUpdate.Id && t.TaskDescription == editTaskDTO.TaskDescription && t.TaskName == editTaskDTO.TaskName)), Times.Once);
        }

        [TestMethod]
        public async Task UpdateTaskAsync_NonExistingTaskId_ThrowsException()
        {
            var repositoryMock = new Mock<IProjectTaskRepository>();
            repositoryMock.Setup(repo => repo.GetTaskAsync(It.IsAny<long>())).ReturnsAsync((Task_)null);
            var rabbitmqMessagingServiceMock = new Mock<IRabbitMQMessagingService>();
            var service = new TasksService(repositoryMock.Object, rabbitmqMessagingServiceMock.Object);
            var editTaskDTO = new EditTaskDTO { Id = 1, TaskDescription = "New Description", TaskName = "New Task Name" };
            await Assert.ThrowsExceptionAsync<ApplicationException>(() => service.UpdateTaskAsync(editTaskDTO));
            rabbitmqMessagingServiceMock.Verify(rabbitmq => rabbitmq.PublishMessage("Error occured: Task doesn't exist."), Times.Once);
        }

        [TestMethod]
        public async Task UpdateTaskStatusAsync_ExistingTaskId_UpdatesTaskStatus()
        {
            var repositoryMock = new Mock<IProjectTaskRepository>();
            var taskToUpdateStatus = new Task_ { Id = 1, Status = TaskStatus_.IN_PROGRESS };
            repositoryMock.Setup(repo => repo.GetTaskAsync(It.IsAny<long>())).ReturnsAsync(taskToUpdateStatus);
            var rabbitmqMessagingServiceMock = new Mock<IRabbitMQMessagingService>();
            var service = new TasksService(repositoryMock.Object, rabbitmqMessagingServiceMock.Object);
            var result = await service.UpdateTaskStatusAsync(1, TaskStatus_.COMPLETE);
            Assert.IsNotNull(result);
            Assert.AreEqual(TaskStatus_.COMPLETE, result.Status);
            Assert.IsNotNull(result.DateFinished);
            repositoryMock.Verify(repo => repo.UpdateTaskAsync(taskToUpdateStatus), Times.Once);
        }

    }
}
