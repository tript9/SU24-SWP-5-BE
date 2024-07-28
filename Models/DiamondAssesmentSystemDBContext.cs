using Microsoft.EntityFrameworkCore;
using SWPApp.Models;

public class DiamondAssesmentSystemDBContext : DbContext
{
    public DiamondAssesmentSystemDBContext(DbContextOptions<DiamondAssesmentSystemDBContext> options)
        : base(options)
    { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServiceDetail> ServiceDetails { get; set; }
    public DbSet<Result> Results { get; set; }
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<CommitmentRecord> CommitmentRecords { get; set; }
    public DbSet<SealingRecord> SealingRecords { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CommitmentRecord>().HasKey(c => c.RecordId);

        modelBuilder.Entity<Request>()
            .HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId);

        modelBuilder.Entity<Request>()
            .HasOne(r => r.Employee)
            .WithMany()
            .HasForeignKey(r => r.EmployeeId);

        modelBuilder.Entity<ServiceDetail>()
            .HasKey(sd => new { sd.ServiceId, sd.AssessmentStep });

        modelBuilder.Entity<ServiceDetail>()
            .HasOne(sd => sd.Service)
            .WithMany()
            .HasForeignKey(sd => sd.ServiceId);

        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Customer)
            .WithMany()
            .HasForeignKey(f => f.CustomerId);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Service)
            .WithMany(s => s.Employees)
            .HasForeignKey(e => e.ServiceId)
            .OnDelete(DeleteBehavior.SetNull); // Configure the delete behavior to set null
    }
}
