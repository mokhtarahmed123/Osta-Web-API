using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureAndTotalReviewsAndCompletedBookingsAndReasonOfRejectAndStatusTOTechniciansTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompletedBookings",
                schema: "Technician",
                table: "Technicians",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                schema: "Technician",
                table: "Technicians",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReasonOfReject",
                schema: "Technician",
                table: "Technicians",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "Technician",
                table: "Technicians",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalReviews",
                schema: "Technician",
                table: "Technicians",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedBookings",
                schema: "Technician",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                schema: "Technician",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "ReasonOfReject",
                schema: "Technician",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Technician",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "TotalReviews",
                schema: "Technician",
                table: "Technicians");
        }
    }
}
