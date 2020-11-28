using Microsoft.EntityFrameworkCore;
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
            using (var dbContext = new PMSContext(new DbContextOptionsBuilder<PMSContext>()
                                            .UseSqlite("Data Source=PMS_Database.db")
                                            .Options))
            {

                Repo repository = new Repo
                {
                    Name = "test123",
                    Url = "https://github.com/109-SETeam/project-manage-system-backend",
                };

                Project project = new Project
                {
                    Name = "project1"
                };
                User user = new User
                {
                    Name = "blakctea123",
                    Account = "test123",
                    AvatarUrl = "https://avatars0.githubusercontent.com/u/31059035?s=400&u=d34edd3e9d38ae89fa98b782c644425d711516c5&v=4",
                    Authority = "",
                };
                dbContext.Add(user);
                dbContext.Add(repository);
                dbContext.Add(project);

                repository = new Repo
                {
                    Name = "test234",
                    Url = "https::/somewhere",
                };

                project = new Project
                {
                    Name = "project2"
                };
                user = new User
                {
                    Name = "AAAAAAAA",
                    Account = "test246",
                    AvatarUrl = "https://avatars2.githubusercontent.com/u/37148109?s=460&u=eaae4f4b457f9bf49f6dea8451568308ca7aba38&v=4",
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
