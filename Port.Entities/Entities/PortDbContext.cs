using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Port.Entities.Entities
{
    public class PortDbContext : DbContext
    {
        public PortDbContext() : base("LaPortalContext")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<PortDbContext>());

            Configuration.LazyLoadingEnabled = true;
        }

        // ReSharper disable once InconsistentNaming
        public DbSet<Port_User> Port_User { get; set; }

        // ReSharper disable once InconsistentNaming
        public DbSet<Port_UserDetail> Port_UserDetail { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Port_User>().HasOptional(x => x.Port_UserDetail).WithRequired(x => x.Port_User).WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }


    }
}
