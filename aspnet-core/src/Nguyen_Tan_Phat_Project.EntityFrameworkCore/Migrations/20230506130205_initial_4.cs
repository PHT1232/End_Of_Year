using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_bankAccounts_BankAccountBankId",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "BankAccountBankId",
                table: "employees",
                newName: "BankId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_BankAccountBankId",
                table: "employees",
                newName: "IX_employees_BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_bankAccounts_BankId",
                table: "employees",
                column: "BankId",
                principalTable: "bankAccounts",
                principalColumn: "BankId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_bankAccounts_BankId",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "BankId",
                table: "employees",
                newName: "BankAccountBankId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_BankId",
                table: "employees",
                newName: "IX_employees_BankAccountBankId");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_bankAccounts_BankAccountBankId",
                table: "employees",
                column: "BankAccountBankId",
                principalTable: "bankAccounts",
                principalColumn: "BankId");
        }
    }
}
