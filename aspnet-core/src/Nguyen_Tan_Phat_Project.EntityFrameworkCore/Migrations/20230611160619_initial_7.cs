using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageId",
                table: "retailProducts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_retailProducts_StorageId",
                table: "retailProducts",
                column: "StorageId");

            migrationBuilder.AddForeignKey(
                name: "FK_retailProducts_storage_StorageId",
                table: "retailProducts",
                column: "StorageId",
                principalTable: "storage",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_retailProducts_storage_StorageId",
                table: "retailProducts");

            migrationBuilder.DropIndex(
                name: "IX_retailProducts_StorageId",
                table: "retailProducts");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "retailProducts");
        }
    }
}
