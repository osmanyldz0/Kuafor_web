using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Kuafor_web.Models;

public partial class BerberDbContext : DbContext
{
    public BerberDbContext()
    {
    }

    public BerberDbContext(DbContextOptions<BerberDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Berber> Berbers { get; set; }

    public virtual DbSet<Hizmet> Hizmets { get; set; }

    public virtual DbSet<Kullanici> Kullanicis { get; set; }

    public virtual DbSet<Menuler> Menulers { get; set; }

    public virtual DbSet<Randevu> Randevus { get; set; }

    public virtual DbSet<Salon> Salons { get; set; }

    public virtual DbSet<Sayfalar> Sayfalars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=OSMAN\\SQLEXPRESS;Database=BerberDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Berber>(entity =>
        {
            entity.ToTable("Berber");

            entity.Property(e => e.BerberId).HasColumnName("Berber_Id");
            entity.Property(e => e.BerberIsim)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Berber_Isim");
            entity.Property(e => e.CalisilmayanGun)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.SalonAd)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VerilenHizmetler)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Salon).WithMany(p => p.Berbers)
                .HasForeignKey(d => d.SalonId)
                .HasConstraintName("FK_Berber_Salon");
        });

        modelBuilder.Entity<Hizmet>(entity =>
        {
            entity.ToTable("Hizmet");

            entity.Property(e => e.HizmetId).HasColumnName("Hizmet_Id");
            entity.Property(e => e.BerberId).HasColumnName("Berber_Id");
            entity.Property(e => e.HizmetAdi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Hizmet_adi");
            entity.Property(e => e.HizmetUcreti).HasColumnName("Hizmet_Ucreti");
            entity.Property(e => e.KullaniciId).HasColumnName("Kullanici_Id");

            entity.HasOne(d => d.Berber).WithMany(p => p.Hizmets)
                .HasForeignKey(d => d.BerberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hizmet_Berber");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.Hizmets)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hizmet_Kullanıcı");
        });

        modelBuilder.Entity<Kullanici>(entity =>
        {
            entity.HasKey(e => e.KullaniciId).HasName("PK_Kullanıcı");

            entity.ToTable("Kullanici");

            entity.Property(e => e.KullaniciId).HasColumnName("Kullanici_Id");
            entity.Property(e => e.Eposta).HasMaxLength(100);
            entity.Property(e => e.KullaniciIsim)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Kullanici_Isim");
            entity.Property(e => e.Parola).HasMaxLength(45);
            entity.Property(e => e.Telefon).HasMaxLength(15);
        });

        modelBuilder.Entity<Menuler>(entity =>
        {
            entity.HasKey(e => e.MenuId);

            entity.ToTable("Menuler");

            entity.Property(e => e.MenuId).ValueGeneratedNever();
            entity.Property(e => e.Baslık)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Icerik)
                .HasColumnType("ntext")
                .HasColumnName("icerik");
        });

        modelBuilder.Entity<Randevu>(entity =>
        {
            entity.ToTable("Randevu");

            entity.Property(e => e.RandevuId).HasColumnName("Randevu_Id");
            entity.Property(e => e.Aktif).HasColumnName("aktif");
            entity.Property(e => e.BerberAd)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.BerberId).HasColumnName("Berber_Id");
            entity.Property(e => e.Hizmetler)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.KullaniciId).HasColumnName("Kullanici_Id");
            entity.Property(e => e.Pasif).HasColumnName("pasif");
            entity.Property(e => e.RandevuSaat).HasColumnName("Randevu_Saat");

            entity.HasOne(d => d.Berber).WithMany(p => p.Randevus)
                .HasForeignKey(d => d.BerberId)
                .HasConstraintName("FK_Randevu_Berber");
        });

        modelBuilder.Entity<Salon>(entity =>
        {
            entity.ToTable("Salon");

            entity.Property(e => e.SalonAd)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Sayfalar>(entity =>
        {
            entity.HasKey(e => e.SayfaId).HasName("PK_Sayfalar_1");

            entity.ToTable("Sayfalar");

            entity.Property(e => e.Aktif).HasColumnName("aktif");
            entity.Property(e => e.Baslık)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("baslık");
            entity.Property(e => e.Silindi).HasColumnName("silindi");
            entity.Property(e => e.İcerik).HasColumnType("ntext");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
