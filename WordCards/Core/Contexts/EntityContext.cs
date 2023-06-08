
namespace WordCards.Core.Contexts
{
    public class EntityContext : BaseContext
    {
        public EntityContext() : base()
        {
            Database.CanConnect();
        }
    }
}
