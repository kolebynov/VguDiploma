using Microsoft.EntityFrameworkCore.Migrations;

namespace Diploma.IndexingService.Core.Migrations
{
    public partial class Add_ErrorInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorInfo",
                table: "InProgressDocuments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorInfo",
                table: "InProgressDocuments");
        }
    }
}
