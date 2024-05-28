using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.Enums;
using ProjectTasks.Interfaces;

namespace ProjectTasks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportGeneratorService _reportGeneratorService;
        //private readonly IRabbitMQMessagingService _rabbitMQMessagingService;

        public ReportsController(IReportGeneratorService reportGeneratorService/*, IRabbitMQMessagingService rabbitMQMessagingService*/)
        {
            _reportGeneratorService = reportGeneratorService;
            //_rabbitMQMessagingService = rabbitMQMessagingService;
        }
        [HttpGet("getReportForProjects")]
        public async Task<IActionResult> GetReportForProjects(DateTime? startDate, DateTime? endDate, ProjectStatus? projectStatus)
        {
            return Ok(await _reportGeneratorService.GenerateReportAboutProjects(startDate, endDate, projectStatus));
        }
        [HttpGet("getReportForTasks/{projectId}")]
        public async Task<IActionResult> GetReportForTasks(DateTime? startDate, DateTime? endDate, TaskStatus_? taskStatus, long projectId)
        {
            return Ok(await _reportGeneratorService.GenerateReportAboutTasks(startDate, endDate, taskStatus, projectId));
        }
    }
}
