using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LuvFinder.Models;

public partial class LuvFinderContext : DbContext
{
    public LuvFinderContext()
    {
    }

    public LuvFinderContext(DbContextOptions<LuvFinderContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionType> QuestionTypes { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLike> UserLikes { get; set; }

    public virtual DbSet<UserMessage> UserMessages { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<UserViewing> UserViewings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-HJJ87HE\\SQLEXPRESS;Database=LuvFinder;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.ToTable("Answer");

            entity.HasIndex(e => e.QuestionId, "IDX_Answer_QuestionID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.Text)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Answer_Question");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cities__3214EC074A6A2D20");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Region).WithMany(p => p.Cities)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cities_Regions");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Countrie__3214EC07E9E4D47E");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code)
                .HasMaxLength(2)
                .IsUnicode(false);
            entity.Property(e => e.Language)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("Profile");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

            entity.HasOne(d => d.Question).WithMany(p => p.Profiles)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Profile_Question");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Question");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ShortDesc)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Text)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.QuestionTypeNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuestionType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Question_QuestionType");
        });

        modelBuilder.Entity<QuestionType>(entity =>
        {
            entity.ToTable("QuestionType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Regions__3214EC0711BA03CA");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Country).WithMany(p => p.Regions)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Regions_Countries");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserLike>(entity =>
        {
            entity.HasIndex(e => e.FromId, "IDX_UserLikes_FromID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FromId).HasColumnName("FromID");
            entity.Property(e => e.ToId).HasColumnName("ToID");

            entity.HasOne(d => d.From).WithMany(p => p.UserLikeFroms)
                .HasForeignKey(d => d.FromId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserLikes_User");

            entity.HasOne(d => d.To).WithMany(p => p.UserLikeTos)
                .HasForeignKey(d => d.ToId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserLikes_User1");
        });

        modelBuilder.Entity<UserMessage>(entity =>
        {
            entity.HasIndex(e => e.FromId, "IDX_UserMessages_FromID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FromId).HasColumnName("FromID");
            entity.Property(e => e.Message)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.ToId).HasColumnName("ToID");

            entity.HasOne(d => d.From).WithMany(p => p.UserMessageFroms)
                .HasForeignKey(d => d.FromId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserMessages_User");

            entity.HasOne(d => d.To).WithMany(p => p.UserMessageTos)
                .HasForeignKey(d => d.ToId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserMessages_User1");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("UserProfile");

            entity.HasIndex(e => e.UserId, "IDX_UserProfile_UserID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AnswerId).HasColumnName("AnswerID");
            entity.Property(e => e.AnswerText).HasColumnType("text");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.Selected).HasDefaultValueSql("((0))");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Answer).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.AnswerId)
                .HasConstraintName("FK_UserProfile_Answer");

            entity.HasOne(d => d.Question).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_Question");

            entity.HasOne(d => d.User).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_User");
        });

        modelBuilder.Entity<UserViewing>(entity =>
        {
            entity.HasIndex(e => e.UserViewingId, "IDX_UserViewings_UserViewingID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserViewedId).HasColumnName("UserViewedID");
            entity.Property(e => e.UserViewingId).HasColumnName("UserViewingID");

            entity.HasOne(d => d.UserViewed).WithMany(p => p.UserViewingUserVieweds)
                .HasForeignKey(d => d.UserViewedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserViewings_User4");

            entity.HasOne(d => d.UserViewingNavigation).WithMany(p => p.UserViewingUserViewingNavigations)
                .HasForeignKey(d => d.UserViewingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserViewings_User3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
