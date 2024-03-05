﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTasks.DTO;
using ProjectTasks.Interfaces;

namespace ProjectTasks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectsService;
        public ProjectsController(IProjectsService projectsService)
        {
            _projectsService = projectsService;
        }
        [HttpGet("getProjects")]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                return Ok(await _projectsService.GetProjectsAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("getProject/{id}")]
        public async Task<IActionResult> GetAllProjects(long id)
        {
            try
            {
                return Ok(await _projectsService.GetProjectAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("updateProject")]
        public async Task<IActionResult> UpdateProject(EditProjectDTO editProjectDTO)
        {
            try
            {
                return Ok(await _projectsService.UpdateProjectAsync(editProjectDTO));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpDelete("deleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(long id)
        {
            try
            {
                return Ok(await _projectsService.DeleteProjectAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("addProject")]
        public async Task<IActionResult> AddProject(AddProjectDTO addProjectDTO)
        {
            try
            {
                return Ok(await _projectsService.AddProjectAsync(addProjectDTO));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
