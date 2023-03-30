using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "exportImportProduct");

            migrationBuilder.AddColumn<float>(
                name: "TotalPrice",
                table: "exportImport",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "exportImport");

            migrationBuilder.AddColumn<float>(
                name: "TotalPrice",
                table: "exportImportProduct",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
