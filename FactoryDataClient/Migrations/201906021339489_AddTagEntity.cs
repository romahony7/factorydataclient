namespace FactoryDataClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        TagTypeId = c.Int(nullable: false),
                        PlcId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plcs", t => t.PlcId, cascadeDelete: true)
                .ForeignKey("dbo.TagTypes", t => t.TagTypeId, cascadeDelete: true)
                .Index(t => t.TagTypeId)
                .Index(t => t.PlcId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "TagTypeId", "dbo.TagTypes");
            DropForeignKey("dbo.Tags", "PlcId", "dbo.Plcs");
            DropIndex("dbo.Tags", new[] { "PlcId" });
            DropIndex("dbo.Tags", new[] { "TagTypeId" });
            DropTable("dbo.Tags");
        }
    }
}
