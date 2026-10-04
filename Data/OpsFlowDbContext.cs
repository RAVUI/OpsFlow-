using Microsoft.EntityFrameworkCore;
using OpsFlow.Models.Entities;

namespace OpsFlow.Data;

public class OpsFlowDbContext : DbContext
{
    public OpsFlowDbContext(
        DbContextOptions<OpsFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<OpsFlow.Models.Entities.Claim> Claims { get; set; }

    public DbSet<ClaimAttachment> ClaimAttachments { get; set; }

    public DbSet<ClaimHistory> ClaimHistory { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);



        modelBuilder.Entity<OpsFlow.Models.Entities.Claim>(entity =>
        {
            entity.ToTable("Claims");

            entity.HasKey(x => x.Id);


            // Issue details

            entity.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Description)
                .IsRequired();


            // Raised by

            entity.Property(x => x.RaisedByUserId)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.RaisedByName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.RaisedByEmail)
                .HasMaxLength(255)
                .IsRequired();


            // Organization

            entity.Property(x => x.OrganizationName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.OrganizationLocation)
                .HasMaxLength(150)
                .IsRequired();


            // Manager

            entity.Property(x => x.ManagerUserId)
                .HasMaxLength(100);

            entity.Property(x => x.ManagerName)
                .HasMaxLength(150);

            entity.Property(x => x.ManagerEmail)
                .HasMaxLength(255);


            // Assigned employee

            entity.Property(x => x.AssignedEmployeeId)
                .HasMaxLength(100);

            entity.Property(x => x.AssignedEmployeeName)
                .HasMaxLength(150);

            entity.Property(x => x.AssignedEmployeeEmail)
                .HasMaxLength(255);


            // Status

            entity.Property(x => x.Status)
                .HasMaxLength(30)
                .IsRequired();


            // Resolution

            entity.Property(x => x.ResolutionComment);


            // Dates

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(x => x.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");


            // INDEXES
        

            entity.HasIndex(x => x.RaisedByUserId);

            entity.HasIndex(x => x.AssignedEmployeeId);

            entity.HasIndex(x => x.ManagerUserId);

            entity.HasIndex(x => x.Status);

            entity.HasIndex(x => x.CreatedAt);


            // RELATIONSHIPS

            entity.HasMany(x => x.Attachments)
                .WithOne(x => x.Claim)
                .HasForeignKey(x => x.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.History)
                .WithOne(x => x.Claim)
                .HasForeignKey(x => x.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // CLAIM ATTACHMENT

        modelBuilder.Entity<ClaimAttachment>(entity =>
        {
            entity.ToTable("ClaimAttachments");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FileName)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.ContentType)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.FileData)
                .HasColumnType("varbinary(max)")
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(x => x.ClaimId);
        });


        // CLAIM HISTORY

        modelBuilder.Entity<ClaimHistory>(entity =>
        {
            entity.ToTable("ClaimHistory");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Action)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.FromStatus)
                .HasMaxLength(30);

            entity.Property(x => x.ToStatus)
                .HasMaxLength(30);

            entity.Property(x => x.PerformedByUserId)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.PerformedByName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.PerformedByEmail)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.Comment);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(x => x.ClaimId);

            entity.HasIndex(x => x.CreatedAt);
        });
    }
}