namespace CleaningPipeline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class owners : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Owners",
                c => new
                    {
                        OwnerId = c.Int(nullable: false, identity: true),
                        OwnerName = c.String(),
                        OwnerAvailability = c.String(),
                    })
                .PrimaryKey(t => t.OwnerId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Owners");
        }
    }
}
