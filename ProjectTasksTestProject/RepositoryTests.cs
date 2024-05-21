using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectTasks.Enums;
using ProjectTasks.Infrastracture;
using ProjectTasks.Interfaces;
using ProjectTasks.Model;
using ProjectTasks.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTasksTestProject
{
    [TestClass]
    public class RepositoryTests
    {
        [TestClass]
        public class ProjectTaskRepositoryTests
        {
            private DbContextOptions<DatabaseContext> _dbContextOptions;
            private Mock<IRabbitMQMessagingService> _mockRabbitMQMessagingService;
            private DatabaseContext _databaseContext;
            private ProjectTaskRepository _repository;

            [TestInitialize]
            public void Setup()
            {
                _dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                    .UseInMemoryDatabase(databaseName: "TestDatabase")
                    .Options;

                _mockRabbitMQMessagingService = new Mock<IRabbitMQMessagingService>();

                _databaseContext = new DatabaseContext(_dbContextOptions);
                _repository = new ProjectTaskRepository(_databaseContext, _mockRabbitMQMessagingService.Object);
            }

            [TestCleanup]
            public void Cleanup()
            {
                _databaseContext.Database.EnsureDeleted();
                _databaseContext.Dispose();
            }

            [TestMethod]
            public async Task AddTaskAsync_ShouldAddTask()
            {
                var task = new Task_ {Id = 1, TaskName = "Test Task", ProjectId = 1, TaskDescription = "Description", DateStarted = DateTime.Now };

                var result = await _repository.AddTaskAsync(task);

                Assert.AreEqual(task, result);
                Assert.AreEqual(1, _databaseContext.Tasks.Count());
            }

            [TestMethod]
            public async Task DeleteTaskAsync_ShouldDeleteTask()
            {
                var task = new Task_ {Id = 1, TaskName = "Test Task", ProjectId = 1, TaskDescription = "Description", DateStarted = DateTime.Now };
                _databaseContext.Tasks.Add(task);
                await _databaseContext.SaveChangesAsync();

                await _repository.DeleteTaskAsync(1);

                Assert.AreEqual(0, _databaseContext.Tasks.Count());
            }

            [TestMethod]
            public async Task UpdateTaskAsync_ShouldUpdateTask()
            {
                var task = new Task_ { Id = 1, TaskName = "Test Task", ProjectId = 1 , TaskDescription = "Description", DateStarted = DateTime.Now };
                _databaseContext.Tasks.Add(task);
                await _databaseContext.SaveChangesAsync();

                task.TaskName = "Updated Task";
                var result = await _repository.UpdateTaskAsync(task);

                Assert.AreEqual("Updated Task", result.TaskName);
            }

            [TestMethod]
            public async Task GetTaskAsync_ShouldReturnTask()
            {
                var task = new Task_ { Id = 1, TaskName = "Test Task", ProjectId = 1, TaskDescription = "Description" , DateStarted = DateTime.Now};
                _databaseContext.Tasks.Add(task);
                await _databaseContext.SaveChangesAsync();

                var result = await _repository.GetTaskAsync(1);

                Assert.AreEqual(task, result);
            }

            [TestMethod]
            public async Task AddProjectAsync_ShouldAddProject()
            {
                var project = new Project
                {
                    Id = 1,
                    ProjectName = "Test Project",
                    Code = "ABC123",
                    DateStarted = DateTime.Now,
                    Status = ProjectStatus.IN_PROGRESS 
                };

                var result = await _repository.AddProjectAsync(project);

                Assert.AreEqual(project, result);
                Assert.AreEqual(1, _databaseContext.Projects.Count());
            }

            [TestMethod]
            public async Task DeleteProjectAsync_ShouldDeleteProject()
            {
                var project = new Project { Id = 1, ProjectName = "Test Project" , Code = "Code" , DateStarted = DateTime.Now};
                _databaseContext.Projects.Add(project);
                await _databaseContext.SaveChangesAsync();

                await _repository.DeleteProjectAsync(1);

                Assert.AreEqual(0, _databaseContext.Projects.Count());
            }

            [TestMethod]
            public async Task UpdateProjectAsync_ShouldUpdateProject()
            {
                var project = new Project { Id = 1, ProjectName = "Test Project", Code = "Code", DateStarted = DateTime.Now };
                _databaseContext.Projects.Add(project);
                await _databaseContext.SaveChangesAsync();

                project.ProjectName = "Updated Project";
                var result = await _repository.UpdateProjectAsync(project);

                Assert.AreEqual("Updated Project", result.ProjectName);
            }

            [TestMethod]
            public async Task GetProjectAsync_ShouldReturnProject()
            {
                var project = new Project {Id = 1, ProjectName = "Test Project", Code = "Code", DateStarted = DateTime.Now };
                _databaseContext.Projects.Add(project);
                await _databaseContext.SaveChangesAsync();

                var result = await _repository.GetProjectAsync(1);

                Assert.AreEqual(project, result);
            }

            [TestMethod]
            public async Task GetProjectsAsync_ShouldReturnAllProjects()
            {
                var project1 = new Project { Id = 1, ProjectName = "Project 1", Code = "Code", DateStarted = DateTime.Now };
                var project2 = new Project { Id = 2, ProjectName = "Project 2", Code = "Code", DateStarted = DateTime.Now };
                _databaseContext.Projects.AddRange(project1, project2);
                await _databaseContext.SaveChangesAsync();

                var result = await _repository.GetProjectsAsync();

                Assert.AreEqual(2, result.Count);
            }

            [TestMethod]
            public async Task GetAllTasksFromProjectAsync_ShouldReturnTasks()
            {
                var project = new Project { Id = 1, ProjectName = "Test Project", Code = "Code", DateStarted = DateTime.Now };
                var task1 = new Task_ { Id = 1, TaskName = "Task 1", ProjectId = 1 , DateStarted = DateTime.Now , TaskDescription = "Description"};
                var task2 = new Task_ { Id = 2, TaskName = "Task 2", ProjectId = 1, DateStarted = DateTime.Now, TaskDescription = "Description" };
                _databaseContext.Projects.Add(project);
                _databaseContext.Tasks.AddRange(task1, task2);
                await _databaseContext.SaveChangesAsync();

                var result = await _repository.GetAllTasksFromProjectAsync(1);

                Assert.AreEqual(2, result.Count);
            }

            [TestMethod]
            public async Task AddFileToProjectAsync_ShouldAddFile()
            {
                var file = new FileAttachment { Id = 1, Name = "Test File", Path = "test/path", ProjectId = 1 };

                var result = await _repository.AddFileToProjectAsync(file);

                Assert.AreEqual(file, result);
                Assert.AreEqual(1, _databaseContext.Files.Count());
            }

            [TestMethod]
            public async Task AddFileToTasksAsync_ShouldAddFile()
            {
                var file = new FileAttachment { Id = 1, Name = "Test File", Path = "test/path", TaskId = 1 };

                var result = await _repository.AddFileToTasksAsync(file);

                Assert.AreEqual(file, result);
                Assert.AreEqual(1, _databaseContext.Files.Count());
            }

            [TestMethod]
            public async Task GetFilesFromProjectAsync_ShouldReturnFiles()
            {
                var file1 = new FileAttachment { Id = 1, Name = "File 1", Path = "path1", ProjectId = 1 };
                var file2 = new FileAttachment { Id = 2, Name = "File 2", Path = "path2", ProjectId = 1 };
                _databaseContext.Files.AddRange(file1, file2);
                await _databaseContext.SaveChangesAsync();

                var result = await _repository.GetFilesFromProjectAsync(1);

                Assert.AreEqual(2, result.Count);
            }

            [TestMethod]
            public async Task GetFilesFromTaskAsync_ShouldReturnFiles()
            {
                var file1 = new FileAttachment { Id = 1, Name = "File 1", Path = "path1", TaskId = 1 };
                var file2 = new FileAttachment
                {
                    Id = 2,
                    Name = "File 2",
                    Path = "path2",
                    TaskId = 1
                };
                _databaseContext.Files.AddRange(file1, file2);
                await _databaseContext.SaveChangesAsync();

                var result = await _repository.GetFilesFromTaskAsync(1);

                Assert.AreEqual(2, result.Count);
            }

            [TestMethod]
            public async Task GetFileAsync_ShouldReturnFile()
            {
                var file = new FileAttachment { Id = 1, Name = "Test File", Path = "test/path" };
                _databaseContext.Files.Add(file);
                await _databaseContext.SaveChangesAsync();

                var result = await _repository.GetFileAsync(1);

                Assert.AreEqual(file, result);
            }

        }
    }
}

