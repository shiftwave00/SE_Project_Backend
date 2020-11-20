using Microsoft.EntityFrameworkCore;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class UserService
    {
        private readonly PMSContext _dbContext;
        public UserService() 
        {
            _dbContext = new PMSContext();
        }
        public UserService(PMSContext context )
        {
            _dbContext = context;
        }

        public void CreateUser(UserModel model)
        {
            _dbContext.Users.Add(model);
            if (_dbContext.SaveChanges() == 0)
            {
                throw new Exception("create user fail");
            }
        }

        public bool CheckUserExist(string account)
        {
            var Users = _dbContext.Users.Where(u => u.Account.Equals(account));
            if (Users.Count() > 0) return true;
            return false;
        }
    }
}
