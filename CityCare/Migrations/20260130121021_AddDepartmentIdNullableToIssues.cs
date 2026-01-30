using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityCare.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentIdNullableToIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Issues",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Issues",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_DepartmentId",
                table: "Issues",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Departments_DepartmentId",
                table: "Issues",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Departments_DepartmentId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_DepartmentId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Issues");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Issues",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
