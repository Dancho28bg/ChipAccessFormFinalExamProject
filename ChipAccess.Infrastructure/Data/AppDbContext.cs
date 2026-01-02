using Microsoft.EntityFrameworkCore;
using ChipAccess.Domain.Entities;

namespace ChipAccess.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<AccessApprovalForm> AccessApprovalForms { get; set; }
        public DbSet<ReassignmentQueue> ReassignmentQueues { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AccessApprovalArchive> AccessApprovalArchives { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<AccessApprovalForm>().ToTable("AccessApprovalForms");
            modelBuilder.Entity<ReassignmentQueue>().ToTable("ReassignmentQueue");
            modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");
            
            modelBuilder.Entity<Employee>()
                .Property(e => e.IsActive)
                .HasDefaultValue(true);
            modelBuilder.Entity<AccessApprovalArchive>()
                .ToTable("AccessApprovalArchive");
        }
    }
}