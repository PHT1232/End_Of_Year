using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerPhone",
                table: "exportImport",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerPhone",
                table: "exportImport");
        }
    }
}
