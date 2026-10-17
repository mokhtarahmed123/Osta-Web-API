using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDataTypeOfTechnicianIdinTechnicianServiceFromintToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AlterColumn<string>(
                name: "TechnicianId",
                schema: "Technician",
                table: "TechnicianServices",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianServices_TechnicianId",
                schema: "Technician",
                table: "TechnicianServices",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicianServices_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianServices",
                column: "TechnicianId",
                principalSchema: "Technician",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianServices_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianServices");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianServices_TechnicianId",
                schema: "Technician",
                table: "TechnicianServices");

            migrationBuilder.AlterColumn<int>(
                name: "TechnicianId",
                schema: "Technician",
                table: "TechnicianServices",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "TechnicianId1",
                schema: "Technician",
                table: "TechnicianServices",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianServices_TechnicianId1",
                schema: "Technician",
                table: "TechnicianServices",
                column: "TechnicianId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicianServices_Technicians_TechnicianId1",
                schema: "Technician",
                table: "TechnicianServices",
                column: "TechnicianId1",
                principalSchema: "Technician",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
