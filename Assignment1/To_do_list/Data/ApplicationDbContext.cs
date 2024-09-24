using Microsoft.EntityFrameworkCore;// File: Data/ApplicationDbContext.cs
using To_do_list.Model;

namespace To_do_list.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
         : base(options)
        {
        }

        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}

