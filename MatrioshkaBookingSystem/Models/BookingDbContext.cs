using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace MatrioshkaBookingSystem.Models;

public partial class BookingDbContext : DbContext
{
    public BookingDbContext()
    {
    }

    public BookingDbContext(DbContextOptions<BookingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Billinginfo> Billinginfos { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Bookingextraasset> Bookingextraassets { get; set; }

    public virtual DbSet<Extraasset> Extraassets { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Roomasset> Roomassets { get; set; }

    public virtual DbSet<Roomtype> Roomtypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=bookingdb;uid=root;pwd=160207AnnA", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.4.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Billinginfo>(entity =>
        {
            entity.HasKey(e => e.BillingId).HasName("PRIMARY");

            entity.ToTable("billinginfo");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.Property(e => e.BillingId).HasColumnName("BillingID");
            entity.Property(e => e.CardNumber).HasMaxLength(250);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Billinginfos)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("billinginfo_ibfk_1");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PRIMARY");

            entity.ToTable("bookings");

            entity.HasIndex(e => e.BillingId, "BillingID");

            entity.HasIndex(e => e.RoomId, "RoomID");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.BillingId).HasColumnName("BillingID");
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Billing).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BillingId)
                .HasConstraintName("bookings_ibfk_3");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("bookings_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("bookings_ibfk_1");
        });

        modelBuilder.Entity<Bookingextraasset>(entity =>
        {
            entity.HasKey(e => new { e.BookingId, e.ExtraAssetId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("bookingextraassets");

            entity.HasIndex(e => e.ExtraAssetId, "ExtraAssetID");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.ExtraAssetId).HasColumnName("ExtraAssetID");
            entity.Property(e => e.ExtraAssetPrice).HasPrecision(10);

            entity.HasOne(d => d.Booking).WithMany(p => p.Bookingextraassets)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingextraassets_ibfk_1");

            entity.HasOne(d => d.ExtraAsset).WithMany(p => p.Bookingextraassets)
                .HasForeignKey(d => d.ExtraAssetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingextraassets_ibfk_2");
        });

        modelBuilder.Entity<Extraasset>(entity =>
        {
            entity.HasKey(e => e.ExtraAssetId).HasName("PRIMARY");

            entity.ToTable("extraassets");

            entity.Property(e => e.ExtraAssetId).HasColumnName("ExtraAssetID");
            entity.Property(e => e.ExtraAssetName).HasMaxLength(250);
            entity.Property(e => e.ExtraAssetStatus)
                .HasDefaultValueSql("'Working'")
                .HasColumnType("enum('Working','Under Maintenance','Damaged')");
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.FloorId).HasName("PRIMARY");

            entity.ToTable("floors");

            entity.HasIndex(e => e.HotelId, "HotelID");

            entity.Property(e => e.FloorId).HasColumnName("FloorID");
            entity.Property(e => e.FloorStatus)
                .HasDefaultValueSql("'Available'")
                .HasColumnType("enum('Available','Non-Available')");
            entity.Property(e => e.HotelId).HasColumnName("HotelID");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Floors)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("floors_ibfk_1");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.HotelId).HasName("PRIMARY");

            entity.ToTable("hotels");

            entity.Property(e => e.HotelId).HasColumnName("HotelID");
            entity.Property(e => e.HotelLocation).HasMaxLength(250);
            entity.Property(e => e.HotelName).HasMaxLength(250);
            entity.Property(e => e.HotelStatus)
                .HasDefaultValueSql("'Open'")
                .HasColumnType("enum('Open','Closed','Under Maintenance')");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.HasIndex(e => e.RoleName, "RoleName").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PRIMARY");

            entity.ToTable("rooms");

            entity.HasIndex(e => e.FloorId, "FloorID");

            entity.HasIndex(e => e.TypeId, "TypeID");

            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.FloorId).HasColumnName("FloorID");
            entity.Property(e => e.RoomStatus).HasMaxLength(250);
            entity.Property(e => e.TypeId).HasColumnName("TypeID");

            entity.HasOne(d => d.Floor).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.FloorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rooms_ibfk_1");

            entity.HasOne(d => d.Type).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rooms_ibfk_2");

            entity.HasMany(d => d.Assets).WithMany(p => p.Rooms)
                .UsingEntity<Dictionary<string, object>>(
                    "Assetsinroom",
                    r => r.HasOne<Roomasset>().WithMany()
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("assetsinrooms_ibfk_2"),
                    l => l.HasOne<Room>().WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("assetsinrooms_ibfk_1"),
                    j =>
                    {
                        j.HasKey("RoomId", "AssetId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("assetsinrooms");
                        j.HasIndex(new[] { "AssetId" }, "AssetID");
                        j.IndexerProperty<int>("RoomId").HasColumnName("RoomID");
                        j.IndexerProperty<int>("AssetId").HasColumnName("AssetID");
                    });
        });

        modelBuilder.Entity<Roomasset>(entity =>
        {
            entity.HasKey(e => e.AssetId).HasName("PRIMARY");

            entity.ToTable("roomassets");

            entity.Property(e => e.AssetId).HasColumnName("AssetID");
            entity.Property(e => e.AssetName).HasMaxLength(250);
            entity.Property(e => e.AssetStatus)
                .HasDefaultValueSql("'Working'")
                .HasColumnType("enum('Working','Under Maintenance','Damaged')");
        });

        modelBuilder.Entity<Roomtype>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PRIMARY");

            entity.ToTable("roomtype");

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.TypeName).HasMaxLength(250);
            entity.Property(e => e.TypePrice).HasPrecision(10);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "Username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.FirstName).HasMaxLength(250);
            entity.Property(e => e.LastName).HasMaxLength(250);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.UserPassword).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_roles_ibfk_2"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_roles_ibfk_1"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("user_roles");
                        j.HasIndex(new[] { "RoleId" }, "RoleID");
                        j.IndexerProperty<int>("UserId").HasColumnName("User_ID");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
