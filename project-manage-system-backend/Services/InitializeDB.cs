using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class InitializeDB
    {
        public InitializeDB()
        {
            using (var dbContext = new PMSContext())
            {

                RepositoryModel repository = new RepositoryModel
                {
                    Name = "test123",
                    Url = "https::/somewhere",
                };

                ProjectModel project = new ProjectModel
                {
                    Name = "project2",
                    Repositories = new List<RepositoryModel>
                    {
                        repository
                    }
                };
                UserModel user = new UserModel
                {
                    Name = "blakctea123",
                    Account = "test123",
                    AvatarUrl = "https://avatars0.githubusercontent.com/u/31059035?s=400&u=d34edd3e9d38ae89fa98b782c644425d711516c5&v=4",
                    Authority = "",
                };
                dbContext.Add(user);
                dbContext.Add(repository);
                dbContext.Add(project);
                
                dbContext.SaveChanges();
            }
        }
    }
}
