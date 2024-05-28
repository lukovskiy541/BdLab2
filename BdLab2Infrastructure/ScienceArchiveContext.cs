using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Bd2Domain.Model;

namespace BdLab2Infrastructure;
public partial class ScienceArchiveContext : DbContext
{
    public ScienceArchiveContext()
    {
    }

    public ScienceArchiveContext(DbContextOptions<ScienceArchiveContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Journal> Journals { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Publication> Publications { get; set; }

    public virtual DbSet<PublicationType> PublicationTypes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Reviewer> Reviewers { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=Max\\SQLEXPRESS; Database=ScienceArchive; Trusted_Connection=True; TrustServerCertificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasOne(d => d.Department).WithMany(p => p.Authors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Authors_Departments");

            entity.HasMany(d => d.Publications).WithMany(p => p.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "AuthorsPublication",
                    r => r.HasOne<Publication>().WithMany()
                        .HasForeignKey("PublicationsId")
                        .HasConstraintName("FK_AuthorsPublications_Publications"),
                    l => l.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .HasConstraintName("FK_AuthorsPublications_Authors"),
                    j =>
                    {
                        j.HasKey("AuthorId", "PublicationsId");
                        j.ToTable("AuthorsPublications");
                    });
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasOne(d => d.Organization).WithMany(p => p.Departments)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Departments_Organizations");
        });

        modelBuilder.Entity<Journal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Journal");

            entity.Property(e => e.Issn).HasColumnName("ISSN");
            entity.Property(e => e.Url).HasColumnName("URL");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.Property(e => e.Url).HasColumnName("URL");
        });

        modelBuilder.Entity<Publication>(entity =>
        {
            entity.Property(e => e.JournalId).HasColumnName("JournalID");
            entity.Property(e => e.Name).HasDefaultValue("");
            entity.Property(e => e.PdfUrl).HasColumnName("pdfURL");

            entity.HasOne(d => d.Journal).WithMany(p => p.Publications)
                .HasForeignKey(d => d.JournalId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Publications_Journals");

            entity.HasOne(d => d.Type).WithMany(p => p.Publications)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Publications_PublicationTypes");

            entity.HasMany(d => d.Subjects).WithMany(p => p.Publications)
                .UsingEntity<Dictionary<string, object>>(
                    "PublicationSubject",
                    r => r.HasOne<Subject>().WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade),
                    l => l.HasOne<Publication>().WithMany()
                        .HasForeignKey("PublicationId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("PublicationId", "SubjectId");
                        j.ToTable("PublicationSubject");
                    });
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Review");

            entity.HasOne(d => d.Publication).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.PublicationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Review_Publications");

            entity.HasOne(d => d.Reviewer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ReviewerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Review_Reviewers");
        });

        modelBuilder.Entity<Reviewer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Reviewer");

            entity.HasOne(d => d.Department).WithMany(p => p.Reviewers)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reviewers_Departments");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
