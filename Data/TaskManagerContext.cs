using Microsoft.EntityFrameworkCore;
using Task_Manager.Models;

using Microsoft.EntityFrameworkCore;
using Task_Manager.Models;

namespace Task_Manager.Data
{
    public class TaskManagerContext : DbContext
    {
        public TaskManagerContext(DbContextOptions<TaskManagerContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TaskTag> TaskTags { get; set; }

        // Configuring relationships using Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User to TaskCategory relationship (one-to-many)
            modelBuilder.Entity<TaskCategory>()
                .HasOne(tc => tc.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(tc => tc.UserID)
                .OnDelete(DeleteBehavior.Cascade);  // Optional: Cascade delete if TaskCategory is deleted

            // User to TaskItem relationship (one-to-many)
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.User)
                .WithMany(u => u.TaskItems)
                .HasForeignKey(t => t.UserID)
                .OnDelete(DeleteBehavior.Cascade);  // Optional: Cascade delete if TaskItem is deleted

            // TaskCategory to TaskItem relationship (one-to-many, optional)
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.TaskItems)
                .HasForeignKey(t => t.CategoryID)
                .IsRequired(false)  // Category is optional for a TaskItem
                .OnDelete(DeleteBehavior.SetNull);  // Optional: Set CategoryID to null on delete of TaskCategory

            // TaskTag to TaskItem relationship (one-to-many)
            modelBuilder.Entity<TaskTag>()
                .HasOne(tt => tt.TaskItem)
                .WithMany(t => t.TaskTags)  // Assuming TaskItem has a collection of TaskTags
                .HasForeignKey(tt => tt.TaskID)
                .OnDelete(DeleteBehavior.Cascade);  // Optional: Cascade delete if TaskItem is deleted

            // TaskTag to User relationship (one-to-many)
            modelBuilder.Entity<TaskTag>()
                .HasOne(tt => tt.User)
                .WithMany(u => u.TaskTags)
                .HasForeignKey(tt => tt.UserID)
                .OnDelete(DeleteBehavior.Cascade);  // Optional: Cascade delete if User is deleted
        }
    }
}
