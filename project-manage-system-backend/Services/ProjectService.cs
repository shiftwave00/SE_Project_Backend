using Microsoft.CodeAnalysis;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class ProjectService
    {
        public void Create(ProjectDto projectDto)
        {
            using (var dbContext = new PMSContext())
            {
                var user = dbContext.Users.Find(projectDto.UserId);
                var project = new ProjectModel
                {
                    Name = projectDto.ProjectName,
                    Owner = user,
                    Repositories = new List<RepositoryModel>()
                };

                user.Projects.Add(project);
                if(dbContext.SaveChanges() == 0)
                {
                    throw new Exception("create project fail");
                }
            }
        }

        public List<UserModel> GetProjectByUserAccount(string account)
        {
            using (var dbContext = new PMSContext())
            {
                var user = dbContext.Users;
                return user.ToList();
            }
        }
    }
}
