using System;
using System.Collections.Generic;
using ConsoleApp6.ExistingData.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp6.ExistingData;

public partial class FitnessExistingContext : DbContext
{
    public FitnessExistingContext(DbContextOptions<FitnessExistingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("clients_pkey");

            entity.ToTable("clients");

            entity.HasIndex(e => e.Email, "clients_email_key").IsUnique();

            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RegistrationDate).HasColumnName("registration_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
