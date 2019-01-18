using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BackgroundServices.Models
{
    public partial class BackgroundTasksContext : DbContext
    {
        public BackgroundTasksContext()
        {
        }

        public BackgroundTasksContext(DbContextOptions<BackgroundTasksContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TestData> TestData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=BackgroundTasks;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestData>(entity =>
            {
                entity.Property(e => e.Value).HasColumnName("value");
            });
        }
    }
}
