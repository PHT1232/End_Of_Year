using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StructureId",
                table: "storage",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_storage_StructureId",
                table: "storage",
                column: "StructureId");

            migrationBuilder.AddForeignKey(
                name: "FK_storage_structure_StructureId",
                table: "storage",
                column: "StructureId",
                principalTable: "structure",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_storage_structure_StructureId",
                table: "storage");

            migrationBuilder.DropIndex(
                name: "IX_storage_StructureId",
                table: "storage");

            migrationBuilder.DropColumn(
                name: "StructureId",
                table: "storage");
        }
    }
}
