using Microsoft.EntityFrameworkCore;
//using CardWords.Business.NotificationTypes.CountdownTypes;
//using CardWords.Business.NotificationTypes.Usual;

namespace CardWords.Core.Contexts
{
    public sealed class ApplicationContext : EntityContext
    {
        public ApplicationContext() : base() 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //EntityConfiguration<NotificationType>(modelBuilder);
            //EntityConfiguration<NotificationTypeActiveDay>(modelBuilder);
            //EntityConfiguration<CountdownNotificationType>(modelBuilder);
        }

        //public DbSet<NotificationType> NotificationTypes { get; set; } = null!;

        //public DbSet<NotificationTypeActiveDay> NotificationTypeActiveDays { get; set; } = null!;

        //public DbSet<CountdownNotificationType> CountdownNotificationTypes { get; set; } = null!;
    }
}
