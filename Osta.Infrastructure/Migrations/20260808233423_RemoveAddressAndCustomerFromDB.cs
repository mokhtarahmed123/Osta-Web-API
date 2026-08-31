using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAddressAndCustomerFromDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // Remove old Booking Foreign Keys
            // =====================================================

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Addresses_AddressId",
                schema: "Booking",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Customers_CustomerId",
                schema: "Booking",
                table: "Bookings");

            // =====================================================
            // Remove old FavoriteTechnicians Customer FK
            // =====================================================

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteTechnicians_Customers_CustomerId",
                schema: "Customer",
                table: "FavoriteTechnicians");

            // =====================================================
            // Drop old Customer tables
            // =====================================================

            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "Customer");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "Customer");

            // =====================================================
            // Remove AddressId from Bookings
            // =====================================================

            migrationBuilder.DropIndex(
                name: "IX_Bookings_AddressId",
                schema: "Booking",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AddressId",
                schema: "Booking",
                table: "Bookings");

            // =====================================================
            // FavoriteTechnicians
            // CustomerId is part of PK
            // =====================================================

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteTechnicians",
                schema: "Customer",
                table: "FavoriteTechnicians");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                schema: "Customer",
                table: "FavoriteTechnicians",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // =====================================================
            // Bookings CustomerId
            // =====================================================

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                schema: "Booking",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // =====================================================
            // Add fields to AspNetUsers
            // =====================================================

            migrationBuilder.AddColumn<string>(
                name: "Area",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                schema: "Identity",
                table: "AspNetUsers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Governorate",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // =====================================================
            // Re-create FavoriteTechnicians PK
            // =====================================================

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteTechnicians",
                schema: "Customer",
                table: "FavoriteTechnicians",
                columns: new[]
                {
                    "CustomerId",
                    "TechnicianId"
                });

            // =====================================================
            // IMPORTANT
            //
            // IX_FavoriteTechnicians_TechnicianId already exists.
            //
            // FK_FavoriteTechnicians_Technicians_TechnicianId
            // already exists.
            //
            // Therefore we DO NOT create either of them here.
            // =====================================================

            // =====================================================
            // Add new Booking Customer FK
            // =====================================================

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_CustomerId",
                schema: "Booking",
                table: "Bookings",
                column: "CustomerId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // =====================================================
            // Add new FavoriteTechnicians Customer FK
            // =====================================================

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteTechnicians_AspNetUsers_CustomerId",
                schema: "Customer",
                table: "FavoriteTechnicians",
                column: "CustomerId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // Remove new Foreign Keys
            // =====================================================

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_CustomerId",
                schema: "Booking",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteTechnicians_AspNetUsers_CustomerId",
                schema: "Customer",
                table: "FavoriteTechnicians");

            // =====================================================
            // Remove Technician FK
            // =====================================================

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteTechnicians_Technicians_TechnicianId",
                schema: "Customer",
                table: "FavoriteTechnicians");

            // =====================================================
            // Remove Technician Index
            // =====================================================

            migrationBuilder.DropIndex(
                name: "IX_FavoriteTechnicians_TechnicianId",
                schema: "Customer",
                table: "FavoriteTechnicians");

            // =====================================================
            // Remove Primary Key
            // =====================================================

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteTechnicians",
                schema: "Customer",
                table: "FavoriteTechnicians");

            // =====================================================
            // Remove AspNetUsers fields
            // =====================================================

            migrationBuilder.DropColumn(
                name: "Area",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Governorate",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileImage",
                schema: "Identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Street",
                schema: "Identity",
                table: "AspNetUsers");

            // =====================================================
            // Restore old CustomerId types
            // =====================================================

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                schema: "Customer",
                table: "FavoriteTechnicians",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                schema: "Booking",
                table: "Bookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // =====================================================
            // Restore FavoriteTechnicians PK
            // =====================================================

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteTechnicians",
                schema: "Customer",
                table: "FavoriteTechnicians",
                columns: new[]
                {
                    "CustomerId",
                    "TechnicianId"
                });

            // =====================================================
            // Restore AddressId
            // =====================================================

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                schema: "Booking",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // =====================================================
            // Re-create Customers
            // =====================================================

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    UserId = table.Column<string>(
                        type: "nvarchar(450)",
                        nullable: false),

                    DateOfBirth = table.Column<DateOnly>(
                        type: "date",
                        nullable: true),

                    ProfileImage = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Customers",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_Customers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // =====================================================
            // Re-create Addresses
            // =====================================================

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    CustomerId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Area = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    City = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    Governorate = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    Street = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Addresses",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_Addresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "Customer",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // =====================================================
            // Restore Indexes
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AddressId",
                schema: "Booking",
                table: "Bookings",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustomerId",
                schema: "Customer",
                table: "Addresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                schema: "Customer",
                table: "Customers",
                column: "UserId");

            // =====================================================
            // Restore Old Foreign Keys
            // =====================================================

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Addresses_AddressId",
                schema: "Booking",
                table: "Bookings",
                column: "AddressId",
                principalSchema: "Customer",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Customers_CustomerId",
                schema: "Booking",
                table: "Bookings",
                column: "CustomerId",
                principalSchema: "Customer",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteTechnicians_Customers_CustomerId",
                schema: "Customer",
                table: "FavoriteTechnicians",
                column: "CustomerId",
                principalSchema: "Customer",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // =====================================================
            // Restore Technician FK
            // =====================================================

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteTechnicians_Technicians_TechnicianId",
                schema: "Customer",
                table: "FavoriteTechnicians",
                column: "TechnicianId",
                principalSchema: "Technician",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}