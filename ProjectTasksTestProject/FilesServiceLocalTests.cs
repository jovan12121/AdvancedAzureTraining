using Microsoft.AspNetCore.Http;
using Moq;
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
    public class FilesServiceLocalTests
    {
        private Mock<IProjectTaskRepository> _repositoryMock;
        private Mock<IRabbitMQMessagingService> _messagingServiceMock;
        private FilesServiceLocal _filesService;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IProjectTaskRepository>();
            _messagingServiceMock = new Mock<IRabbitMQMessagingService>();
            _filesService = new FilesServiceLocal(_repositoryMock.Object, _messagingServiceMock.Object);
        }

        [TestMethod]
        public async Task AddFileToProjectAsync_ValidFile_ReturnsFileAttachment()
        {
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            var projectId = 1;
            var fileAttachment = new FileAttachment
            {
                Name = Path.GetRandomFileName() + Path.GetExtension(fileName),
                Path = Path.Combine("Files", "Projects", projectId.ToString(), fileName),
                ProjectId = projectId
            };
            _repositoryMock.Setup(repo => repo.AddFileToProjectAsync(It.IsAny<FileAttachment>())).ReturnsAsync(fileAttachment);
            var result = await _filesService.AddFileToProjectAsync(fileMock.Object, projectId);
            Assert.IsNotNull(result);
            Assert.AreEqual(fileAttachment.Name, result.Name);
            Assert.AreEqual(fileAttachment.Path, result.Path);
            Assert.AreEqual(fileAttachment.ProjectId, result.ProjectId);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public async Task AddFileToProjectAsync_NullFile_ThrowsApplicationException()
        {
            IFormFile file = null;
            var projectId = 1;
            await _filesService.AddFileToProjectAsync(file, projectId);
        }

        [TestMethod]
        public async Task AddFileToTaskAsync_ValidFile_ReturnsFileAttachment()
        {
            var fileMock = new Mock<IFormFile>();
            var content = "File";
            var fileName = "test.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var taskId = 1;
            var fileAttachment = new FileAttachment
            {
                Name = Path.GetRandomFileName() + Path.GetExtension(fileName),
                Path = Path.Combine("Files", "Tasks", taskId.ToString(), fileName),
                TaskId = taskId
            };
            _repositoryMock.Setup(repo => repo.AddFileToTasksAsync(It.IsAny<FileAttachment>())).ReturnsAsync(fileAttachment);
            var result = await _filesService.AddFileToTaskAsync(fileMock.Object, taskId);
            Assert.IsNotNull(result);
            Assert.AreEqual(fileAttachment.Name, result.Name);
            Assert.AreEqual(fileAttachment.Path, result.Path);
            Assert.AreEqual(fileAttachment.TaskId, result.TaskId);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public async Task AddFileToTaskAsync_NullFile_ThrowsApplicationException()
        {
            IFormFile file = null;
            var taskId = 1;
            await _filesService.AddFileToTaskAsync(file, taskId);
        }

        [TestMethod]
        public async Task DeleteFileFromProjectAsync_ValidFile_DeletesFile()
        {
            var fileId = 1;
            var projectId = 1;
            var fileAttachment = new FileAttachment
            {
                Id = fileId,
                Path = Path.Combine("Files", "Projects", projectId.ToString(), "test.txt"),
                ProjectId = projectId
            };
            _repositoryMock.Setup(repo => repo.GetFileAsync(fileId)).ReturnsAsync(fileAttachment);
            var result = await _filesService.DeleteFileFromProjectAsync(fileId, projectId);
            Assert.IsTrue(result);
            _repositoryMock.Verify(repo => repo.DeleteFileAsync(fileId), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public async Task DeleteFileFromProjectAsync_InvalidProjectId_ThrowsApplicationException()
        {
            var fileId = 1;
            var projectId = 2;
            var fileAttachment = new FileAttachment
            {
                Id = fileId,
                Path = Path.Combine("Files", "Projects", "1", "test.txt"),
                ProjectId = 1
            };
            _repositoryMock.Setup(repo => repo.GetFileAsync(fileId)).ReturnsAsync(fileAttachment);
            await _filesService.DeleteFileFromProjectAsync(fileId, projectId);
        }

        [TestMethod]
        public async Task DeleteFileFromTaskAsync_ValidFile_DeletesFile()
        {
            var fileId = 1;
            var taskId = 1;
            var fileAttachment = new FileAttachment
            {
                Id = fileId,
                Path = Path.Combine("Files", "Tasks", taskId.ToString(), "test.txt"),
                TaskId = taskId
            };
            _repositoryMock.Setup(repo => repo.GetFileAsync(fileId)).ReturnsAsync(fileAttachment);
            var result = await _filesService.DeleteFileFromTaskAsync(fileId, taskId);
            Assert.IsTrue(result);
            _repositoryMock.Verify(repo => repo.DeleteFileAsync(fileId), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public async Task DeleteFileFromTaskAsync_InvalidTaskId_ThrowsApplicationException()
        {
            var fileId = 1;
            var taskId = 2;
            var fileAttachment = new FileAttachment
            {
                Id = fileId,
                Path = Path.Combine("Files", "Tasks", "1", "test.txt"),
                TaskId = 1
            };
            _repositoryMock.Setup(repo => repo.GetFileAsync(fileId)).ReturnsAsync(fileAttachment);
            await _filesService.DeleteFileFromTaskAsync(fileId, taskId);
        }

    }
}
