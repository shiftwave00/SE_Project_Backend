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
        public void CreateUser(UserModel model)
        {
            using(var dbContext = new PMSContext())
            {
                dbContext.Add(model);
                if(dbContext.SaveChanges() == 0)
                {
                    throw new Exception("create user fail");
                }

            }
        }

        public bool CheckUserExist(string account)
        {
            using (var dbContext = new PMSContext())
            {
                var Users = dbContext.Users.Where(u => u.Account.Equals(account));
                if (Users.Count() > 0) return true;

            }
            return false;
        }
    }
}
