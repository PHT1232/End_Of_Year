using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Discount",
                table: "exportImportCustomer",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Discount",
                table: "exportImport",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "exportImportCustomer");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "exportImport");
        }
    }
}
