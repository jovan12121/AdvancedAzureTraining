using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectTasks.Model;

namespace ProjectTasks.Infrastracture
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task_> Tasks { get; set; }
        public DbSet<FileAttachment> Files { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasKey(p => p.Id);
            modelBuilder.Entity<Project>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Task_>().HasKey(p => p.Id);
            modelBuilder.Entity<Task_>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<FileAttachment>().HasKey(f => f.Id);
            modelBuilder.Entity<FileAttachment>().Property(f => f.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Project>().HasMany(p => p.Tasks).WithOne(t => t.Project).HasForeignKey(t => t.ProjectId);
            modelBuilder.Entity<Project>().HasMany(p => p.Files).WithOne(f => f.Project).HasForeignKey(f => f.ProjectId);
            modelBuilder.Entity<Task_>().HasMany(t => t.Files).WithOne(f => f.Task).HasForeignKey(f => f.TaskId);
            modelBuilder.Entity<Task_>()
                            .Property(e => e.MetaData)
                            .HasConversion(
                                        v => JsonConvert.SerializeObject(v),
                                        v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
            base.OnModelCreating(modelBuilder);
        }

    }
}
