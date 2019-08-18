namespace FactoryDataClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cleanUpModel : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.PlcFormViewModels");
        }
        
        public override void Down()
        {
        }
    }
}
