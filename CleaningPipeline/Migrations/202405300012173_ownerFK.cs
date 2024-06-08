namespace CleaningPipeline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ownerFK : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Chores", "OwnerId", c => c.Int(nullable: false));
            CreateIndex("dbo.Chores", "OwnerId");
            AddForeignKey("dbo.Chores", "OwnerId", "dbo.Owners", "OwnerId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Chores", "OwnerId", "dbo.Owners");
            DropIndex("dbo.Chores", new[] { "OwnerId" });
            DropColumn("dbo.Chores", "OwnerId");
        }
    }
}
