using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Evaluation_3.Models.Entity
{
    public partial class mada_immoContext : DbContext
    {
        public mada_immoContext()
        {
        }

        public mada_immoContext(DbContextOptions<mada_immoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<Bien> Biens { get; set; } = null!;
        public virtual DbSet<Client> Clients { get; set; } = null!;
        public virtual DbSet<Csvbien> Csvbiens { get; set; } = null!;
        public virtual DbSet<Csvcommission> Csvcommissions { get; set; } = null!;
        public virtual DbSet<Csvlocation> Csvlocations { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<Photo> Photos { get; set; } = null!;
        public virtual DbSet<Photobien> Photobiens { get; set; } = null!;
        public virtual DbSet<Proprietaire> Proprietaires { get; set; } = null!;
        public virtual DbSet<Region> Regions { get; set; } = null!;
        public virtual DbSet<Typebien> Typebiens { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Name=pgsqlString");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admin");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Login)
                    .HasMaxLength(255)
                    .HasColumnName("login");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");
            });

            modelBuilder.Entity<Bien>(entity =>
            {
                entity.ToTable("bien");

                entity.HasIndex(e => e.Reference, "bien_reference_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Idproprietaire).HasColumnName("idproprietaire");

                entity.Property(e => e.Idregion).HasColumnName("idregion");

                entity.Property(e => e.Idtype).HasColumnName("idtype");

                entity.Property(e => e.Loyermensuel).HasColumnName("loyermensuel");

                entity.Property(e => e.Nom)
                    .HasMaxLength(255)
                    .HasColumnName("nom");

                entity.Property(e => e.Reference)
                    .HasMaxLength(255)
                    .HasColumnName("reference");

                entity.HasOne(d => d.IdproprietaireNavigation)
                    .WithMany(p => p.Biens)
                    .HasForeignKey(d => d.Idproprietaire)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bien_idproprietaire_fkey");

                entity.HasOne(d => d.IdregionNavigation)
                    .WithMany(p => p.Biens)
                    .HasForeignKey(d => d.Idregion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bien_idregion_fkey");

                entity.HasOne(d => d.IdtypeNavigation)
                    .WithMany(p => p.Biens)
                    .HasForeignKey(d => d.Idtype)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bien_idtype_fkey");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("client");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Nom)
                    .HasMaxLength(255)
                    .HasColumnName("nom");
            });

            modelBuilder.Entity<Csvbien>(entity =>
            {
                entity.ToTable("csvbiens");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.LoyerMensuel).HasColumnName("loyer_mensuel");

                entity.Property(e => e.Nom)
                    .HasMaxLength(255)
                    .HasColumnName("nom");

                entity.Property(e => e.Proprietaire)
                    .HasMaxLength(255)
                    .HasColumnName("proprietaire");

                entity.Property(e => e.Reference)
                    .HasMaxLength(255)
                    .HasColumnName("reference");

                entity.Property(e => e.Region)
                    .HasMaxLength(255)
                    .HasColumnName("region");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<Csvcommission>(entity =>
            {
                entity.ToTable("csvcommission");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Commission).HasColumnName("commission");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<Csvlocation>(entity =>
            {
                entity.ToTable("csvlocation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Client)
                    .HasMaxLength(255)
                    .HasColumnName("client");

                entity.Property(e => e.DateDebut)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("date_debut");

                entity.Property(e => e.DureeMois).HasColumnName("duree_mois");

                entity.Property(e => e.Reference)
                    .HasMaxLength(255)
                    .HasColumnName("reference");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("location");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Datedebut)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("datedebut");

                entity.Property(e => e.Duree).HasColumnName("duree");

                entity.Property(e => e.Idbien).HasColumnName("idbien");

                entity.Property(e => e.Idclient).HasColumnName("idclient");

                entity.Property(e => e.Ispayed).HasColumnName("ispayed");

                entity.Property(e => e.Referencebien)
                    .HasMaxLength(255)
                    .HasColumnName("referencebien");

                entity.HasOne(d => d.IdbienNavigation)
                    .WithMany(p => p.LocationIdbienNavigations)
                    .HasForeignKey(d => d.Idbien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("location_idbien_fkey");

                entity.HasOne(d => d.IdclientNavigation)
                    .WithMany(p => p.Locations)
                    .HasForeignKey(d => d.Idclient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("location_idclient_fkey");

                entity.HasOne(d => d.ReferencebienNavigation)
                    .WithMany(p => p.LocationReferencebienNavigations)
                    .HasPrincipalKey(p => p.Reference)
                    .HasForeignKey(d => d.Referencebien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("location_referencebien_fkey");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.ToTable("photo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Path).HasColumnName("path");
            });

            modelBuilder.Entity<Photobien>(entity =>
            {
                entity.ToTable("photobien");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Idbien).HasColumnName("idbien");

                entity.Property(e => e.Idphoto).HasColumnName("idphoto");

                entity.HasOne(d => d.IdbienNavigation)
                    .WithMany(p => p.Photobiens)
                    .HasForeignKey(d => d.Idbien)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("photobien_idbien_fkey");

                entity.HasOne(d => d.IdphotoNavigation)
                    .WithMany(p => p.Photobiens)
                    .HasForeignKey(d => d.Idphoto)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("photobien_idphoto_fkey");
            });

            modelBuilder.Entity<Proprietaire>(entity =>
            {
                entity.ToTable("proprietaire");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Nom)
                    .HasMaxLength(255)
                    .HasColumnName("nom")
                    .HasDefaultValueSql("'Proprietaire'::character varying");

                entity.Property(e => e.Numerotel)
                    .HasMaxLength(15)
                    .HasColumnName("numerotel");
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.ToTable("region");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Nom)
                    .HasMaxLength(255)
                    .HasColumnName("nom");
            });

            modelBuilder.Entity<Typebien>(entity =>
            {
                entity.ToTable("typebien");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Commission).HasColumnName("commission");

                entity.Property(e => e.Nom)
                    .HasMaxLength(100)
                    .HasColumnName("nom");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
