using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureMoneyPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianPayout_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianPayout");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianWallet_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianWallet");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                schema: "Technician",
                table: "TechnicianPayout",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicianPayout_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianPayout",
                column: "TechnicianId",
                principalSchema: "Technician",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicianWallet_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianWallet",
                column: "TechnicianId",
                principalSchema: "Technician",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianPayout_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianPayout");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianWallet_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianWallet");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                schema: "Technician",
                table: "TechnicianPayout",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicianPayout_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianPayout",
                column: "TechnicianId",
                principalSchema: "Technician",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicianWallet_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianWallet",
                column: "TechnicianId",
                principalSchema: "Technician",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
