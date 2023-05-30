using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace Updater.Core.Contexts
{
    public class BaseContext : DbContext
    {
        public BaseContext()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }        
    }
}
