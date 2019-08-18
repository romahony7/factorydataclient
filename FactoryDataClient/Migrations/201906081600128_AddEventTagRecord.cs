namespace FactoryDataClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEventTagRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventTagRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TagId = c.Int(nullable: false),
                        Data = c.Long(nullable: false),
                        PlcTS = c.DateTime(nullable: false),
                        RecordTS = c.DateTime(nullable: false, defaultValueSql: "GETDATE()"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.TagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventTagRecords", "TagId", "dbo.Tags");
            DropIndex("dbo.EventTagRecords", new[] { "TagId" });
            DropTable("dbo.EventTagRecords");
        }
    }
}
