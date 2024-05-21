using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class ProjectServiceTests
    {
        private Mock<IProjectTaskRepository> _mockRepository;
        private Mock<IRabbitMQMessagingService> _mockMessagingService;
        private IProjectsService _projectsService;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepository = new Mock<IProjectTaskRepository>();
            _mockMessagingService = new Mock<IRabbitMQMessagingService>();
            _projectsService = new ProjectsService(_mockRepository.Object, _mockMessagingService.Object);
        }

        [TestMethod]
        public async Task AddProjectAsync_ShouldReturnAddedProject()
        {
            var addProjectDto = new AddProjectDTO
            {
                ProjectName = "Test Project",
                Code = "TP",
                DateStarted = DateTime.Now
            };
            var expectedProject = new Project { Id = 1, ProjectName = "Test Project", Code = "TP", DateStarted = DateTime.Now, Status = ProjectStatus.IN_PROGRESS };
            _mockRepository.Setup(repo => repo.AddProjectAsync(It.IsAny<Project>())).ReturnsAsync(expectedProject);
            var result = await _projectsService.AddProjectAsync(addProjectDto);
            Assert.AreEqual(expectedProject.Id, result.Id);
            Assert.AreEqual(expectedProject.ProjectName, result.ProjectName);
        }

        [TestMethod]
        public async Task DeleteProjectAsync_ShouldReturnTrueOnSuccessfulDeletion()
        {
            var projectId = 1;
            _mockRepository.Setup(repo => repo.DeleteProjectAsync(projectId)).Returns(Task.CompletedTask);
            var result = await _projectsService.DeleteProjectAsync(projectId);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public async Task GetProjectAsync_ReturnsProject()
        {
            var projectId = 1;
            var expectedProject = new Project { Id = projectId };
            _mockRepository.Setup(r => r.GetProjectAsync(projectId)).Returns(Task.FromResult(expectedProject));
            var project = await _projectsService.GetProjectAsync(projectId);
            Assert.IsNotNull(project);
            Assert.AreEqual(expectedProject, project);
            _mockRepository.Verify(r => r.GetProjectAsync(projectId), Times.Once);
        }
        [TestMethod]
        public async Task GetProjectsAsync_ReturnsListOfProjects()
        {
            var expectedProjects = new List<Project>() { new Project { Id = 1 }, new Project { Id = 2 } };
            _mockRepository.Setup(r => r.GetProjectsAsync()).Returns(Task.FromResult(expectedProjects));
            var projects = await _projectsService.GetProjectsAsync();
            Assert.IsNotNull(projects);
            Assert.AreEqual(expectedProjects.Count, projects.Count);
            _mockRepository.Verify(r => r.GetProjectsAsync(), Times.Once);
        }
    }
}
