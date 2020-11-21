using Microsoft.Extensions.Configuration;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_manage_system_backend.Services
{
    public class BaseService
    {
        protected PMSContext _dbContext;
        protected IConfiguration _configuration;

        public BaseService(PMSContext dbContext)
        {
            _dbContext = dbContext;
        }

        public BaseService(PMSContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
    }
}
