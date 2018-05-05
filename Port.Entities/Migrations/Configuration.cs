using System;
using Port.Entities.Entities;

namespace Port.Entities.Migrations
{
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<PortDbContext>
    {

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            //data kayýplarý yaþanabilir.
            //this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(PortDbContext context)
        {

        }

        public  void CreateAdmin(PortDbContext context)
        {
            //var guid = Guid.NewGuid();
            //Port_User user = new Port_User
            //{
            //    pu_FirstName = "admin",
            //    pu_LastName = "admin",
            //    pu_Pasaword = "1234",
            //    pu_Email = "admin@la.com.tr",
            //    pu_BirthDate = DateTime.Now,
            //    pu_RememberMe = true,
            //    responseGuidId = guid,
            //    status = Entities.Enums.ResponseResultEnums.CMP,

            //};

            //user.Port_UserDetail = new Port_UserDetail { pud_Id = 1 };
            //context.Port_User.Attach(user);
            //context.SaveChanges();
        }
    }
}
