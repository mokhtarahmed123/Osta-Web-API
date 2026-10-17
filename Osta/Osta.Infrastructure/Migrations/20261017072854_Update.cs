using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_YourTable_EndDate_After_StartDate",
                schema: "Payment",
                table: "Coupons",
                sql: "[EndDate] > [StartDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_ScheduledEnd_After_ScheduledStart",
                schema: "Appointment",
                table: "Appointments",
                sql: "[ScheduledEnd] > [ScheduledStart]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_YourTable_EndDate_After_StartDate",
                schema: "Payment",
                table: "Coupons");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_ScheduledEnd_After_ScheduledStart",
                schema: "Appointment",
                table: "Appointments");
        }
    }
}
