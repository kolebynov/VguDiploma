using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Diploma.IndexingService.Core.Migrations
{
    public partial class asdas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InProgressDocuments",
                table: "InProgressDocuments");

            migrationBuilder.AddColumn<string>(
                name: "UserIdentity",
                table: "InProgressDocuments",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastStatusUpdateTime",
                table: "InProgressDocuments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddPrimaryKey(
                name: "PK_InProgressDocuments",
                table: "InProgressDocuments",
                columns: new[] { "Id", "UserIdentity" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InProgressDocuments",
                table: "InProgressDocuments");

            migrationBuilder.DropColumn(
                name: "UserIdentity",
                table: "InProgressDocuments");

            migrationBuilder.DropColumn(
                name: "LastStatusUpdateTime",
                table: "InProgressDocuments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InProgressDocuments",
                table: "InProgressDocuments",
                column: "Id");
        }
    }
}
