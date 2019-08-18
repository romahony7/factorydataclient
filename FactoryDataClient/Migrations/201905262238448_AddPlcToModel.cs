namespace FactoryDataClient.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddPlcToModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Plcs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        IPAddress = c.String(nullable: false, maxLength: 100),
                        DisableSubscriptions = c.Boolean(nullable: false),
                        PollRateOverride = c.Int(nullable: false),
                        ProcessorSlot = c.Int(nullable: false),
                        Port = c.Int(nullable: false),
                        EventPollRate = c.Int(nullable: false),
                        SubscriptionPollRate = c.Int(nullable: false),
                        TransactionPollRate = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Plcs");
        }
    }
}
