using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bankAccounts_employees_EmployeeId",
                table: "bankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_bankAccounts_EmployeeId",
                table: "bankAccounts");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "bankAccounts");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountBankId",
                table: "employees",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_BankAccountBankId",
                table: "employees",
                column: "BankAccountBankId");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_bankAccounts_BankAccountBankId",
                table: "employees",
                column: "BankAccountBankId",
                principalTable: "bankAccounts",
                principalColumn: "BankId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_bankAccounts_BankAccountBankId",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "IX_employees_BankAccountBankId",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "BankAccountBankId",
                table: "employees");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "bankAccounts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_bankAccounts_EmployeeId",
                table: "bankAccounts",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_bankAccounts_employees_EmployeeId",
                table: "bankAccounts",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id");
        }
    }
}
