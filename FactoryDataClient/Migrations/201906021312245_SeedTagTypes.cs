namespace FactoryDataClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedTagTypes : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                 INSERT INTO [dbo].[TagTypes] ([Name]) VALUES ('Event Tag')
                 INSERT INTO [dbo].[TagTypes] ([Name]) VALUES ('Subscription Tag')
                 INSERT INTO [dbo].[TagTypes] ([Name]) VALUES ('Transaction Tag')
                ");
        }
        
        public override void Down()
        {
        }
    }
}
