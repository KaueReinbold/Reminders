namespace Reminders.Infrastructure.Data.EntityFramework.Configurations;

public class RemindersConfiguration
    : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder
            .ToTable("Reminders");

        builder
            .HasKey(reminder => reminder.Id);

        builder
            .Property(reminder => reminder.Title)
            .IsRequired();

        builder
            .Property(reminder => reminder.Description)
            .IsRequired();

        builder
            .Property(reminder => reminder.LimitDate)
            .IsRequired();

        builder
            .Property(reminder => reminder.IsDone)
            .IsRequired();

        builder
            .Property(reminder => reminder.IsDeleted)
            .IsRequired();
    }
}