using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityCare.Migrations
{
    /// <inheritdoc />
    public partial class MakeDepartmentIdRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Departments_DepartmentId",
                table: "Issues");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Issues",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Departments_DepartmentId",
                table: "Issues",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Departments_DepartmentId",
                table: "Issues");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Issues",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Departments_DepartmentId",
                table: "Issues",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }
    }
}
