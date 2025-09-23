using Identity;
using ManagementWorkOrdersAPI.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DatabaseContext;

public class ApplicationDbContext:IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
    }

    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<Extract> Extracts { get; set; }
    public DbSet<Approval> Approvals { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkOrder>()
                .HasIndex(o => new { o.WorkOrderNumber, o.Type })
                .IsUnique();
    }

}
