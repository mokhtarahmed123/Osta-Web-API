using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAttributeFromAppointmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualEnd",
                schema: "Appointment",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ActualStart",
                schema: "Appointment",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualEnd",
                schema: "Appointment",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStart",
                schema: "Appointment",
                table: "Appointments",
                type: "datetime2",
                nullable: true);
        }
    }
}
