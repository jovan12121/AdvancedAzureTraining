using Microsoft.EntityFrameworkCore;
using ProjectTasks.Model;

namespace ProjectTasks.Infrastracture
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options):base(options) { }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task_> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasKey(p => p.Id);
            modelBuilder.Entity<Project>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Task_>().HasKey(p => p.Id);
            modelBuilder.Entity<Task_>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Project>().HasMany(p => p.Tasks).WithOne(t => t.Project).HasForeignKey(t=>t.ProjectId);
            base.OnModelCreating(modelBuilder);
        }

    }
}
