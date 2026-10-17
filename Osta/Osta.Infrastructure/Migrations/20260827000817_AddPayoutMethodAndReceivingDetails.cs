using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayoutMethodAndReceivingDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Method",
                schema: "Technician",
                table: "TechnicianPayout",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReceivingDetails",
                schema: "Technician",
                table: "TechnicianPayout",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Method",
                schema: "Technician",
                table: "TechnicianPayout");

            migrationBuilder.DropColumn(
                name: "ReceivingDetails",
                schema: "Technician",
                table: "TechnicianPayout");
        }
    }
}
