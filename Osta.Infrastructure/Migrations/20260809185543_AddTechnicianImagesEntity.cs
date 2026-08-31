using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianImagesEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                schema: "Technician",
                table: "Technicians");

            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                schema: "Technician",
                table: "Technicians",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TechnicianImages",
                schema: "Technician",
                columns: table => new
                {
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FrontNationalIdImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackNationalIdImage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianImages", x => x.TechnicianId);
                    table.ForeignKey(
                        name: "FK_TechnicianImages_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalSchema: "Technician",
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianImages",
                schema: "Technician");

            migrationBuilder.DropColumn(
                name: "NationalId",
                schema: "Technician",
                table: "Technicians");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                schema: "Technician",
                table: "Technicians",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
