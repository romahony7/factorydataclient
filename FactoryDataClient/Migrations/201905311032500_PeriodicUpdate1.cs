namespace FactoryDataClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PeriodicUpdate1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlcViewModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        IPAddress = c.String(maxLength: 100),
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
            DropTable("dbo.PlcViewModels");
        }
    }
}
