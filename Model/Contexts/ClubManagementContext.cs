using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model.Models;

namespace Model.Contexts;

public partial class ClubManagementContext : DbContext
{
    public ClubManagementContext()
    {
    }

    public ClubManagementContext(DbContextOptions<ClubManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Club> Clubs { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventParticipant> EventParticipants { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnectionString"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Club>(entity =>
        {
            entity.HasKey(e => e.ClubId).HasName("PK__Clubs__D35058C7E853BEA3");

            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.ClubName).HasMaxLength(100);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C870CB8107B0");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.EventName).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Upcoming");

            entity.HasOne(d => d.Club).WithMany(p => p.Events)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Events__ClubID__4222D4EF");
        });

        modelBuilder.Entity<EventParticipant>(entity =>
        {
            entity.HasKey(e => e.EventParticipantId).HasName("PK__EventPar__09F32B725B6C3B10");

            entity.HasIndex(e => new { e.EventId, e.UserId }, "UQ_Event_User").IsUnique();

            entity.Property(e => e.EventParticipantId).HasColumnName("EventParticipantID");
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Event).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventPart__Event__47DBAE45");

            entity.HasOne(d => d.User).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventPart__UserI__48CFD27E");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Reports__D5BD48E5F55D2052");

            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReportStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");

            entity.HasOne(d => d.Club).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reports__ClubID__5165187F");

            entity.HasOne(d => d.Semester).WithMany(p => p.Reports)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reports__Semeste__52593CB8");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("PK__Semester__043301BDB1210062");

            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.SemesterName).HasMaxLength(20);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Teams__123AE7B96765B33B");

            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.TeamName).HasMaxLength(50);

            entity.HasOne(d => d.Club).WithMany(p => p.Teams)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Teams__ClubID__5535A963");
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.TeamMemberId).HasName("PK__TeamMemb__C7C09285F95AFDAC");

            entity.Property(e => e.TeamMemberId).HasColumnName("TeamMemberID");
            entity.Property(e => e.JoinDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeamMembe__TeamI__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeamMembe__UserI__59FA5E80");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACD7D6211F");

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F284567F666422").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.JoinDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .HasColumnName("StudentID");
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Club).WithMany(p => p.Users)
                .HasForeignKey(d => d.ClubId)
                .HasConstraintName("FK__Users__ClubID__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
