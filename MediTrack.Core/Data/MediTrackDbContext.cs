using MediTrack.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MediTrack.Core.Data;

public class MediTrackDbContext : DbContext
{
    public MediTrackDbContext(DbContextOptions<MediTrackDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<IntakeLog> IntakeLogs => Set<IntakeLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.FullName).HasMaxLength(100);
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OfficialName).HasMaxLength(150);
            entity.Property(e => e.DisplayName).HasMaxLength(150);
            entity.Property(e => e.DosageUnit).HasMaxLength(20);
            entity.Property(e => e.IntakeInstructions).HasMaxLength(500);
            entity.Property(e => e.MedicineType).HasMaxLength(30);
            entity.Property(e => e.Frequency).HasMaxLength(50);
            entity.Property(e => e.ReminderTimes).HasMaxLength(200);
            entity.Property(e => e.MealTiming).HasMaxLength(30);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Medications)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IntakeLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.IntakeLogs)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Medication)
                  .WithMany(m => m.IntakeLogs)
                  .HasForeignKey(e => e.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
