using Microsoft.EntityFrameworkCore;

namespace CardWords.Core.Contexts
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
