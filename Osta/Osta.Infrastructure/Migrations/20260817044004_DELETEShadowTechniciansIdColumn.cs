using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DELETEShadowTechniciansIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TechniciansId",
                schema: "Customer",
                table: "FavoriteTechnicians",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteTechnicians_TechniciansId",
                schema: "Customer",
                table: "FavoriteTechnicians",
                column: "TechniciansId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteTechnicians_Technicians_TechniciansId",
                schema: "Customer",
                table: "FavoriteTechnicians",
                column: "TechniciansId",
                principalSchema: "Technician",
                principalTable: "Technicians",
                principalColumn: "Id");
        }
    }
}
