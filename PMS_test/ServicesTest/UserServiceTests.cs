using Isopoh.Cryptography.Argon2;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Models;
using project_manage_system_backend.Services;
using project_manage_system_backend.Shares;
using System.Data.Common;
using Xunit;

namespace PMS_test
{
    [TestCaseOrderer("XUnit.Project.Orderers.AlphabeticalOrderer", "XUnit.Project")]
    public class UserServiceTests
    {
        private readonly PMSContext _dbContext;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _dbContext = new PMSContext(new DbContextOptionsBuilder<PMSContext>()
                                            .UseSqlite(CreateInMemoryDatabase())
                                            .Options);
            _dbContext.Database.EnsureCreated();
            _userService = new UserService(_dbContext);
            InitialDatabase();
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");

            connection.Open();

            return connection;
        }

        internal void InitialDatabase()
        {
            _dbContext.Users.Add(new User
            {
                Account = "admin",
                Password = Argon2.Hash("password"),
                Authority = "Admin",
                Name = "管理員"
            });
        }

        public void TestCreateAdminUser()
        {
            _userService.CreateUser(new LocalAccountDto
            {
                Account = "testAdmin",
                Password = "password"
            });

            var actualUser = _dbContext.Users.Find("testAdmin");
            Assert.Equal("testAdmin", actualUser.Account);
            Assert.Equal("Admin", actualUser.Authority);
        }
    }
}
