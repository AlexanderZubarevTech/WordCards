using Microsoft.EntityFrameworkCore;
using Reminder.Business.NotificationTypes.CountdownTypes;
using Reminder.Business.NotificationTypes.Usual;

namespace Reminder.Core.Contexts
{
    public sealed class ApplicationContext : EntityContext
    {
        public ApplicationContext() : base() 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityConfiguration<NotificationType>(modelBuilder);
            EntityConfiguration<NotificationTypeActiveDay>(modelBuilder);
            EntityConfiguration<CountdownNotificationType>(modelBuilder);
        }

        public DbSet<NotificationType> NotificationTypes { get; set; } = null!;

        public DbSet<NotificationTypeActiveDay> NotificationTypeActiveDays { get; set; } = null!;

        public DbSet<CountdownNotificationType> CountdownNotificationTypes { get; set; } = null!;
    }
}
