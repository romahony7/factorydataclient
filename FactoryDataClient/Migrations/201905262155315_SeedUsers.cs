namespace FactoryDataClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedUsers : DbMigration
    {
        public override void Up()
        {
            Sql(@"
            INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'384a3753-227e-4bc6-876d-3e22030d6c85', N'admin@HanleyAutomation.com', 0, N'AKv+k89ZNiHkpwdH1TaRB8xue55PkT+qeR77wW9lkYFDz/zLkJ9tGUjJW8HZ995Odg==', N'd3695451-9020-41e4-8291-bb9de8c5a815', NULL, 0, 0, NULL, 1, 0, N'admin@HanleyAutomation.com')
            INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'a45036fa-dcbe-4dbe-81ab-3eefca8281fe', N'guest@HanleyAutomation.com', 0, N'AM2Q3FhqFvkSHDmZeqEL9A6Fl1hAis3eJjsXCTAywYRaOy72ln4g9yOmGEP6a81eTA==', N'9179cbd5-9292-495f-9188-5a8ac7a9d1a5', NULL, 0, 0, NULL, 1, 0, N'guest@HanleyAutomation.com')
            ");

            Sql(@"
            INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'ce74a039-bd8e-4605-a841-0b40c653e84e', N'Administrator')
            ");

            Sql(@"
            INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'384a3753-227e-4bc6-876d-3e22030d6c85', N'ce74a039-bd8e-4605-a841-0b40c653e84e')
            ");
        }
        
        public override void Down()
        {
        }
    }
}
