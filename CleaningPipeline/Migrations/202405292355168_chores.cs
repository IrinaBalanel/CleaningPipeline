namespace CleaningPipeline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chores : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chores",
                c => new
                    {
                        ChoreId = c.Int(nullable: false, identity: true),
                        ChoreName = c.String(),
                        ChoreDescription = c.String(),
                        ChoreFrequency = c.String(),
                    })
                .PrimaryKey(t => t.ChoreId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Chores");
        }
    }
}
