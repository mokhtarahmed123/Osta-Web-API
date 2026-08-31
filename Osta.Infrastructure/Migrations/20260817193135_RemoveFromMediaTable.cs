using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFromMediaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                schema: "Booking",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "FileSize",
                schema: "Booking",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                schema: "Booking",
                table: "Media");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "Booking",
                table: "Media",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "Booking",
                table: "Media");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                schema: "Booking",
                table: "Media",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                schema: "Booking",
                table: "Media",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                schema: "Booking",
                table: "Media",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }
    }
}
