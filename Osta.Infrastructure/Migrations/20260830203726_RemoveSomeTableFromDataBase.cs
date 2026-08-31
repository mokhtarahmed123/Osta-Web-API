using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSomeTableFromDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "Administration");

            migrationBuilder.DropTable(
                name: "SupportTickets",
                schema: "Support");

            migrationBuilder.DropTable(
                name: "SystemSettings",
                schema: "Administration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Support");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "Administration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                schema: "Support",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTickets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                schema: "Administration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "Administration",
                table: "SystemSettings",
                columns: new[] { "Id", "Key", "Value" },
                values: new object[,]
                {
                    { 1, "RefreshTokenDays", "7" },
                    { 2, "AccessTokenMinutes", "30" },
                    { 3, "MaxLoginAttempts", "5" },
                    { 4, "EmailConfirmationExpiryMinutes", "30" },
                    { 5, "ResetPasswordCodeExpiryMinutes", "15" },
                    { 6, "MaintenanceMode", "false" },
                    { 7, "DefaultPageSize", "20" },
                    { 8, "MaxPageSize", "100" },
                    { 9, "EmailConfirmationRequired", "true" },
                    { 10, "PasswordMinLength", "8" },
                    { 11, "PasswordMaxFailedAttempts", "5" },
                    { 12, "ServiceRequestExpiryDays", "30" },
                    { 13, "MaxServiceImages", "5" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                schema: "Administration",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_UserId",
                schema: "Support",
                table: "SupportTickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                schema: "Administration",
                table: "SystemSettings",
                column: "Key",
                unique: true);
        }
    }
}
