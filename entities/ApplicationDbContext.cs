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

    public virtual DbSet<Blob> Blobs { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<CustomerLocation> CustomerLocations { get; set; }

    public virtual DbSet<CustomerTarget> CustomerTargets { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemType> ItemTypes { get; set; }

    public virtual DbSet<ItemUnit> ItemUnits { get; set; }

    public virtual DbSet<RepositionRequest> RepositionRequests { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SalesVisit> SalesVisits { get; set; }

    public virtual DbSet<VisitPlan> VisitPlans { get; set; }

    public virtual DbSet<VisitType> VisitTypes { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("pg_trgm")
            .HasPostgresExtension("postgis");

        modelBuilder.Entity<Blob>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("blobs_pkey");

            entity.ToTable("blobs");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.BlobName)
                .HasColumnType("character varying")
                .HasColumnName("blob_name");
            entity.Property(e => e.ContentType)
                .HasColumnType("character varying")
                .HasColumnName("content_type");
            entity.Property(e => e.Filename)
                .HasColumnType("character varying")
                .HasColumnName("filename");
            entity.Property(e => e.SizeBytes).HasColumnName("size_bytes");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("uploaded_at");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("brands_pkey");

            entity.ToTable("brands");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

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
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.Potential).HasColumnName("potential");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<CustomerTarget>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_targets_pkey");

            entity.ToTable("customer_targets");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CustomerLocationId).HasColumnName("customer_location_id");
            entity.Property(e => e.TargetName).HasColumnName("target_name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");
            entity.Property(e => e.ValidTo).HasColumnName("valid_to");

            entity.HasOne(d => d.CustomerLocation).WithMany(p => p.CustomerTargets)
                .HasForeignKey(d => d.CustomerLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_targets_customer_location_id_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("items_pkey");

            entity.ToTable("items");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ShortName)
                .HasMaxLength(100)
                .HasColumnName("short_name");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

            entity.HasOne(d => d.Brand).WithMany(p => p.Items)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("items_brand_id_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Items)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("items_type_id_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.Items)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("items_unit_id_fkey");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Items)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("items_warehouse_id_fkey");
        });

        modelBuilder.Entity<ItemType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_types_pkey");

            entity.ToTable("item_types");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<ItemUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_units_pkey");

            entity.ToTable("item_units");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
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
            entity.Property(e => e.Note).HasColumnName("note");
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

        modelBuilder.Entity<VisitPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("visit_plans_pkey");

            entity.ToTable("visit_plans");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CustomerLocationId).HasColumnName("customer_location_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.SalesId).HasColumnName("sales_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.VisitOrder)
                .HasDefaultValue(1)
                .HasColumnName("visit_order");

            entity.HasOne(d => d.CustomerLocation).WithMany(p => p.VisitPlans)
                .HasForeignKey(d => d.CustomerLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("visit_plans_customer_location_id_fkey");

            entity.HasOne(d => d.Sales).WithMany(p => p.VisitPlans)
                .HasForeignKey(d => d.SalesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("visit_plans_sales_id_fkey");
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

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("warehouses_pkey");

            entity.ToTable("warehouses");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
