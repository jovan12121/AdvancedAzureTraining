﻿using ProjectTaskCosmos.Interfaces;
using ProjectTaskCosmos.Model;

namespace ProjectTaskCosmos.Services
{
    public class ProjectsService : IProjectsService
    {
        private CosmosService cosmosService;
        public ProjectsService()
        {
            cosmosService = new CosmosService();
        }
        public async Task<Project> GetProjectAsync(long projectId)
        {
            return await cosmosService.GetProjectAsync(projectId);        
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await cosmosService.GetProjectsAsync();
        }
    }
}
