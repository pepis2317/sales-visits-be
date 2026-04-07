using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using entities.Entities;

namespace entities;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CustomerLocation> CustomerLocations { get; set; }

    public virtual DbSet<RepositionRequest> RepositionRequests { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SalesVisit> SalesVisits { get; set; }

    public virtual DbSet<VisitType> VisitTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("pg_trgm")
            .HasPostgresExtension("postgis");

        modelBuilder.Entity<CustomerLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_locations_pkey");

            entity.ToTable("customer_locations");

            entity.HasIndex(e => e.Name, "idx_customer_locations_name_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.LastVisitedAt).HasColumnName("last_visited_at");
            entity.Property(e => e.Location)
                .HasColumnType("geometry(Point,4326)")
                .HasColumnName("location");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<RepositionRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reposition_requests_pkey");

            entity.ToTable("reposition_requests");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AcceptedAt).HasColumnName("accepted_at");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerLocationId).HasColumnName("customer_location_id");
            entity.Property(e => e.DeclinedAt).HasColumnName("declined_at");
            entity.Property(e => e.NewPosition)
                .HasColumnType("geography(Point,4326)")
                .HasColumnName("new_position");
            entity.Property(e => e.OldPosition)
                .HasColumnType("geography(Point,4326)")
                .HasColumnName("old_position");
            entity.Property(e => e.SalesId).HasColumnName("sales_id");

            entity.HasOne(d => d.CustomerLocation).WithMany(p => p.RepositionRequests)
                .HasForeignKey(d => d.CustomerLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reposition_requests_customer_location_id_fkey");

            entity.HasOne(d => d.Sales).WithMany(p => p.RepositionRequests)
                .HasForeignKey(d => d.SalesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reposition_requests_sales_id_fkey");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sales_pkey");

            entity.ToTable("sales");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<SalesVisit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sales_visits_pkey");

            entity.ToTable("sales_visits");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CustomerLocationId).HasColumnName("customer_location_id");
            entity.Property(e => e.SalesId).HasColumnName("sales_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.VisitTypeId)
                .HasDefaultValueSql("'9b0396b3-d5f5-45fd-8043-0ed61317c4fb'::uuid")
                .HasColumnName("visit_type_id");

            entity.HasOne(d => d.CustomerLocation).WithMany(p => p.SalesVisits)
                .HasForeignKey(d => d.CustomerLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sales_visits_customer_location_id_fkey");

            entity.HasOne(d => d.Sales).WithMany(p => p.SalesVisits)
                .HasForeignKey(d => d.SalesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sales_visits_sales_id_fkey");

            entity.HasOne(d => d.VisitType).WithMany(p => p.SalesVisits)
                .HasForeignKey(d => d.VisitTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sales_visits_visit_type_id_fkey");
        });

        modelBuilder.Entity<VisitType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("visit_types_pkey");

            entity.ToTable("visit_types");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
