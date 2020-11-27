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

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Repo> Repositories { get; set; }

    }
}
