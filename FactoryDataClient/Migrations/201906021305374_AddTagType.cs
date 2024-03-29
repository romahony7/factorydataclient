namespace FactoryDataClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TagTypes");
        }
    }
}
