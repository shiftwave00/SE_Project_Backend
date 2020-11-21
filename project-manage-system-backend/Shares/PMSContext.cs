using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using project_manage_system_backend.Models;

namespace project_manage_system_backend.Shares
{
    public class PMSContext: DbContext
    {
        public PMSContext(DbContextOptions<PMSContext> options): base(options) {}

        public PMSContext() { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<RepoModel> Repositories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite("Data Source=PMS_Database.db");
            }
        }
    }
}
