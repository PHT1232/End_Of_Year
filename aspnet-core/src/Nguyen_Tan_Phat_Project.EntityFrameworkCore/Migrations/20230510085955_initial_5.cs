using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orgCustomer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TaxIdentification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgCustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgCustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgCustomerPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgCustomerAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgCustomerWebsite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgCustomerDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankId = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    table.PrimaryKey("PK_orgCustomer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orgCustomer_bankAccounts_BankId",
                        column: x => x.BankId,
                        principalTable: "bankAccounts",
                        principalColumn: "BankId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_orgCustomer_BankId",
                table: "orgCustomer",
                column: "BankId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orgCustomer");
        }
    }
}
