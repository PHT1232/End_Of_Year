using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Discount",
                table: "customers",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "customers");
        }
    }
}
