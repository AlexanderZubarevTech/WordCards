using Microsoft.EntityFrameworkCore;

namespace Reminder.Core.Contexts
{
    public class EntityContext : BaseContext
    {
        public EntityContext() : base()
        {
            Database.CanConnect();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<Id>()
                .HaveConversion<Id.IdConverter>();
        }
    }
}
