using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrderCreator",
                table: "exportImport",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "OrderCreator",
                table: "exportImport",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
