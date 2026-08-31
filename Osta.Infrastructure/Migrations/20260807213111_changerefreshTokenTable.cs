using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changerefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                schema: "Identity",
                table: "RefreshTokens",
                newName: "refreshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "refreshToken",
                schema: "Identity",
                table: "RefreshTokens",
                newName: "Token");
        }
    }
}
