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
    public DbSet<RequestDetail> RequestDetails { get; set; }
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

        // Xóa cấu hình liên quan đến EmployeeId
        // modelBuilder.Entity<Request>()
        //     .HasOne(r => r.Employee)
        //     .WithMany()
        //     .HasForeignKey(r => r.EmployeeId);

        modelBuilder.Entity<RequestDetail>()
            .HasKey(rd => new { rd.RequestId, rd.ServiceId });

        modelBuilder.Entity<RequestDetail>()
            .HasOne(rd => rd.Request)
            .WithMany()
            .HasForeignKey(rd => rd.RequestId);

        modelBuilder.Entity<RequestDetail>()
            .HasOne(rd => rd.Service)
            .WithMany()
            .HasForeignKey(rd => rd.ServiceId);

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
    }
}
