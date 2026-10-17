using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianPayoutsandWalletTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechnicianPayout",
                schema: "Technician",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianPayout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianPayout_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalSchema: "Technician",
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianWallet",
                schema: "Technician",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianWallet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianWallet_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalSchema: "Technician",
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianPayout_TechnicianId",
                schema: "Technician",
                table: "TechnicianPayout",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianWallet_TechnicianId",
                schema: "Technician",
                table: "TechnicianWallet",
                column: "TechnicianId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianPayout",
                schema: "Technician");

            migrationBuilder.DropTable(
                name: "TechnicianWallet",
                schema: "Technician");
        }
    }
}
