using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exportImportCustomer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExportImportCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CustomerCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReciveAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneToCall = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exportImportCustomer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_exportImportCustomer_customers_CustomerCode",
                        column: x => x.CustomerCode,
                        principalTable: "customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_exportImportCustomer_exportImport_ExportImportCode",
                        column: x => x.ExportImportCode,
                        principalTable: "exportImport",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_exportImportCustomer_CustomerCode",
                table: "exportImportCustomer",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_exportImportCustomer_ExportImportCode",
                table: "exportImportCustomer",
                column: "ExportImportCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exportImportCustomer");
        }
    }
}
