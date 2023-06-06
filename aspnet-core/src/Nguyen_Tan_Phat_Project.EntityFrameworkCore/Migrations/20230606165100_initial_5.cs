using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exportImport_storage_StorageId",
                table: "exportImport");

            migrationBuilder.DropIndex(
                name: "IX_exportImport_StorageId",
                table: "exportImport");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "exportImport");

            migrationBuilder.RenameColumn(
                name: "StorageInputId",
                table: "exportImport",
                newName: "StructureId");

            migrationBuilder.AddColumn<string>(
                name: "StorageId",
                table: "exportImportProduct",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageInputId",
                table: "exportImportProduct",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "exportImportProduct");

            migrationBuilder.DropColumn(
                name: "StorageInputId",
                table: "exportImportProduct");

            migrationBuilder.RenameColumn(
                name: "StructureId",
                table: "exportImport",
                newName: "StorageInputId");

            migrationBuilder.AddColumn<string>(
                name: "StorageId",
                table: "exportImport",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_exportImport_StorageId",
                table: "exportImport",
                column: "StorageId");

            migrationBuilder.AddForeignKey(
                name: "FK_exportImport_storage_StorageId",
                table: "exportImport",
                column: "StorageId",
                principalTable: "storage",
                principalColumn: "Id");
        }
    }
}
