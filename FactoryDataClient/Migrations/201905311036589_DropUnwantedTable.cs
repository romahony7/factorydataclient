namespace FactoryDataClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropUnwantedTable : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.PlcViewModels");
        }
        
        public override void Down()
        {
        }
    }
}
