using MediTrack.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MediTrack.Core.Data;

public class MediTrackDbContext : DbContext
{
    public MediTrackDbContext(DbContextOptions<MediTrackDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<MedicationSchedule> MedicationSchedules => Set<MedicationSchedule>();
    public DbSet<MedicationInventory> MedicationInventories => Set<MedicationInventory>();
    public DbSet<MedicationSnooze> MedicationSnoozes => Set<MedicationSnooze>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<IntakeLog> IntakeLogs => Set<IntakeLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map to explicit snake_case table names matching the SQL schema
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Medication>().ToTable("medications");
        modelBuilder.Entity<MedicationSchedule>().ToTable("medication_schedules");
        modelBuilder.Entity<MedicationInventory>().ToTable("medication_inventory");
        modelBuilder.Entity<MedicationSnooze>().ToTable("medication_snooze");
        modelBuilder.Entity<Reminder>().ToTable("reminders");
        modelBuilder.Entity<IntakeLog>().ToTable("intake_logs");

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
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Medications)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Schedules)
                  .WithOne(s => s.Medication)
                  .HasForeignKey(s => s.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Inventory)
                  .WithOne(i => i.Medication)
                  .HasForeignKey<MedicationInventory>(i => i.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Snooze)
                  .WithOne(s => s.Medication)
                  .HasForeignKey<MedicationSnooze>(s => s.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Reminders)
                  .WithOne(r => r.Medication)
                  .HasForeignKey(r => r.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.IntakeLogs)
                  .WithOne(l => l.Medication)
                  .HasForeignKey(l => l.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MedicationSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MealTiming).HasMaxLength(30);
            entity.Property(e => e.WeekdaySchedule)
                  .HasColumnType("json")
                  .HasConversion(
                      v => System.Text.Json.JsonSerializer.Serialize(v.Select(d => (int)d).ToArray(), (System.Text.Json.JsonSerializerOptions?)null),
                      v => System.Text.Json.JsonSerializer.Deserialize<int[]>(v, (System.Text.Json.JsonSerializerOptions?)null)!
                               .Select(i => (DayOfWeek)i).ToArray());
            entity.HasOne(e => e.Medication)
                  .WithMany(m => m.Schedules)
                  .HasForeignKey(e => e.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Reminders)
                  .WithOne(r => r.Schedule)
                  .HasForeignKey(r => r.ScheduleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MedicationInventory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.MedicationId).IsUnique();
            entity.HasOne(e => e.Medication)
                  .WithOne(m => m.Inventory)
                  .HasForeignKey<MedicationInventory>(e => e.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MedicationSnooze>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.MedicationId).IsUnique();
            entity.HasOne(e => e.Medication)
                  .WithOne(m => m.Snooze)
                  .HasForeignKey<MedicationSnooze>(e => e.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.MedicationId);
            entity.HasIndex(e => e.ScheduleId);
            entity.HasIndex(e => e.ScheduledDateTime);
            entity.HasIndex(e => e.Status);
            entity.HasOne(e => e.Medication)
                  .WithMany(m => m.Reminders)
                  .HasForeignKey(e => e.MedicationId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Schedule)
                  .WithMany(s => s.Reminders)
                  .HasForeignKey(e => e.ScheduleId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.IntakeLogs)
                  .WithOne(l => l.Reminder)
                  .HasForeignKey(l => l.ReminderId)
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
            entity.HasOne(e => e.Reminder)
                  .WithMany(r => r.IntakeLogs)
                  .HasForeignKey(e => e.ReminderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
