using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChnageNameOfAttributeInMediaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "Booking",
                table: "Media",
                newName: "RepairMediaType");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Booking",
                table: "Media",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RepairMediaType",
                schema: "Booking",
                table: "Media",
                newName: "Type");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Booking",
                table: "Media",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
