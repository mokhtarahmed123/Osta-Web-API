using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionSeederAndSystemSettingSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Permission",
                columns: new[] { "Id", "Code", "action", "description", "resource" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "SERVICE_CREATE", "Create", "Create a service", "Service" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "SERVICE_UPDATE", "Update", "Update a service", "Service" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "SERVICE_DELETE", "Delete", "Delete a service", "Service" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "SERVICE_READ", "Read", "Read services", "Service" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "CATEGORY_CREATE", "Create", "Create a category", "Category" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "CATEGORY_UPDATE", "Update", "Update a category", "Category" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "CATEGORY_DELETE", "Delete", "Delete a category", "Category" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), "CATEGORY_READ", "Read", "Read categories", "Category" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), "SERVICEAREA_CREATE", "Create", "Create a service area", "ServiceArea" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "SERVICEAREA_UPDATE", "Update", "Update a service area", "ServiceArea" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "SERVICEAREA_DELETE", "Delete", "Delete a service area", "ServiceArea" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "SERVICEAREA_READ", "Read", "Read service areas", "ServiceArea" }
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                schema: "Administration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 13);
        }
    }
}
