namespace Port.Entities.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class OneToOne : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Port_User",
                c => new
                {
                    pu_Id = c.Long(nullable: false, identity: true),
                    pu_FirstName = c.String(maxLength: 50),
                    pu_LastName = c.String(maxLength: 50),
                    pu_Email = c.String(maxLength: 50),
                    pu_Pasaword = c.String(),
                    pu_BirthDate = c.DateTime(),
                })
                .PrimaryKey(t => t.pu_Id);

            CreateTable(
                "dbo.Port_UserDetail",
                c => new
                {
                    pud_Id = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.pud_Id)
                .ForeignKey("dbo.Port_User", t => t.pud_Id, cascadeDelete: true)
                .Index(t => t.pud_Id);


            CreateStoredProcedure("GET_USER",
                p => new
                {
                    // ReSharper disable once RedundantArgumentDefaultValue
                    email = p.String(maxLength: 50, defaultValueSql: null),
                    // ReSharper disable once RedundantArgumentDefaultValue
                    pass = p.String(maxLength: 50, defaultValueSql: null)
                },

                body: @"
                    IF (@email IS NULL OR  @pass IS NULL)
                    BEGIN
                    SELECT 
                             [pu_Id]
                            ,[pu_FirstName]
                            ,[pu_LastName]
                            ,[pu_Email]
                            ,[pu_Pasaword]
                            ,[pu_BirthDate]
                    	    ,[pud_Id]
                    FROM Port_User  LEFT JOIN Port_UserDetail  ON pu_Id=pud_Id
                    END
                    ELSE
                    BEGIN
                    SELECT 
                           [pu_Id]
                          ,[pu_FirstName]
                          ,[pu_LastName]
                          ,[pu_Email]
                          ,[pu_Pasaword]
                          ,[pu_BirthDate]
                    	  ,[pud_Id]
                    FROM Port_User  LEFT JOIN Port_UserDetail  ON pu_Id=pud_Id WHERE pu_email=@email AND pu_Pasaword=@pass
                    END
                ");
            CreateStoredProcedure("INSERT_USER",
                i => new
                {
                    pu_FirstName = i.String(maxLength: 50),
                    pu_LastName = i.String(),
                    pu_Email = i.String(),
                    pu_Pasaword = i.String(),
                    pu_BirthDate = i.DateTime()
                },

                body: @"
DECLARE @scopeIdentity bigint
INSERT INTO PORT_USER ([pu_FirstName]
                       ,[pu_LastName]
                       ,[pu_Email]
                       ,[pu_Pasaword]
                       ,[pu_BirthDate])
                       VALUES
                       (@pu_FirstName,
                        @pu_LastName,
                        @pu_Email,                                                  
                        @pu_Pasaword,                
                        @pu_BirthDate)

SET @scopeIdentity=SCOPE_IDENTITY(); 
INSERT INTO PORT_USERDETAIL(pud_Id) values (@scopeIdentity) ");

            CreateStoredProcedure("UPDATE_USER",
     i => new
     {
         pu_Id=i.Long(),
         pu_FirstName = i.String(maxLength: 50),
         pu_LastName = i.String(),
         pu_Email = i.String(),
         pu_Pasaword = i.String(),
         pu_BirthDate = i.DateTime()
     },

     body: @"
UPDATE  PORT_USER SET  [pu_FirstName] =@pu_FirstName
                   ,[pu_LastName]=@pu_LastName
                   ,[pu_Email]=@pu_Email
                   ,[pu_Pasaword]=@pu_Pasaword
                   ,[pu_BirthDate]=@pu_BirthDate WHERE pu_Id=@pu_Id ");

        CreateStoredProcedure("DELETE_USER", i=> new {pu_Id=i.Long() },body:@" DELETE PORT_USER WHERE pu_Id=@pu_Id");
           
        }

        public override void Down()
        {
            DropForeignKey("dbo.Port_UserDetail", "pud_Id", "dbo.Port_User");
            DropIndex("dbo.Port_UserDetail", new[] { "pud_Id" });
            DropTable("dbo.Port_UserDetail");
            DropTable("dbo.Port_User");
        }
    }
}
