namespace FactoryDataClient.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddSubscriptionTagRecords : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubscriptionTagRecords",
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
            DropForeignKey("dbo.SubscriptionTagRecords", "TagId", "dbo.Tags");
            DropIndex("dbo.SubscriptionTagRecords", new[] { "TagId" });
            DropTable("dbo.SubscriptionTagRecords");
        }
    }
}
