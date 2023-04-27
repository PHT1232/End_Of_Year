using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nguyen_Tan_Phat_Project.Migrations
{
    public partial class initial_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bankAccounts",
                columns: table => new
                {
                    BankId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankCity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bankAccounts", x => x.BankId);
                });

            migrationBuilder.CreateTable(
                name: "chungMinhND",
                columns: table => new
                {
                    SoCMND = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayCap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiCap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuocTich = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chungMinhND", x => x.SoCMND);
                });

            migrationBuilder.CreateTable(
                name: "structure",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LevelOfUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitOf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessRN = table.Column<int>(type: "int", nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedPlace = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_structure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeGender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeDateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeCMNDSoCMND = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkUnitId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TaxIdentification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeSalary = table.Column<int>(type: "int", nullable: false),
                    SalaryFactor = table.Column<float>(type: "real", nullable: false),
                    TypeOfContract = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeBankAccountBankId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employees_bankAccounts_EmployeeBankAccountBankId",
                        column: x => x.EmployeeBankAccountBankId,
                        principalTable: "bankAccounts",
                        principalColumn: "BankId");
                    table.ForeignKey(
                        name: "FK_employees_chungMinhND_EmployeeCMNDSoCMND",
                        column: x => x.EmployeeCMNDSoCMND,
                        principalTable: "chungMinhND",
                        principalColumn: "SoCMND");
                    table.ForeignKey(
                        name: "FK_employees_structure_WorkUnitId",
                        column: x => x.WorkUnitId,
                        principalTable: "structure",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_employees_EmployeeBankAccountBankId",
                table: "employees",
                column: "EmployeeBankAccountBankId");

            migrationBuilder.CreateIndex(
                name: "IX_employees_EmployeeCMNDSoCMND",
                table: "employees",
                column: "EmployeeCMNDSoCMND");

            migrationBuilder.CreateIndex(
                name: "IX_employees_WorkUnitId",
                table: "employees",
                column: "WorkUnitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "bankAccounts");

            migrationBuilder.DropTable(
                name: "chungMinhND");

            migrationBuilder.DropTable(
                name: "structure");
        }
    }
}
