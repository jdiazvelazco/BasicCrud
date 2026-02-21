using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WsApiexamen.Models;

public partial class BdiExamenContext : DbContext
{
    public BdiExamenContext()
    {
    }

    public BdiExamenContext(DbContextOptions<BdiExamenContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblExamen> TblExamen { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblExamen>(entity =>
        {
            entity.HasKey(e => e.IdExamen).HasName("PK_tblExamen");

            entity.ToTable("tblExamen");

            entity.Property(e => e.IdExamen)
                .ValueGeneratedNever()
                .HasColumnName("idExamen");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
