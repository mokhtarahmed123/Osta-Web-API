using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitiesConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Bookings_BookingId",
                schema: "Appointment",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_CustomerId",
                schema: "Booking",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Technicians_TechnicianId",
                schema: "Booking",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServices_Bookings_BookingId",
                schema: "Booking",
                table: "BookingServices");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServices_Services_ServiceId",
                schema: "Booking",
                table: "BookingServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Bookings_BookingId",
                schema: "Administration",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_CouponUsages_AspNetUsers_UserId",
                schema: "Payment",
                table: "CouponUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_CouponUsages_Bookings_BookingId",
                schema: "Payment",
                table: "CouponUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_AspNetUsers_UploadedByUserId",
                schema: "Booking",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Bookings_BookingId",
                schema: "Booking",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Bookings_BookingId",
                schema: "Chat",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                schema: "Payment",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Bookings_BookingId",
                schema: "Review",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_AspNetRoles_RoleId",
                schema: "Identity",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                schema: "Identity",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Category_CategoryId",
                schema: "Service",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianWallet_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianWallet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnicianServices",
                schema: "Technician",
                table: "TechnicianServices");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianServices_TechnicianId",
                schema: "Technician",
                table: "TechnicianServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnicianServiceAreas",
                schema: "Technician",
                table: "TechnicianServiceAreas");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianServiceAreas_TechnicianId",
                schema: "Technician",
                table: "TechnicianServiceAreas");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BookingId",
                schema: "Review",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermission",
                schema: "Identity",
                table: "RolePermission");

            migrationBuilder.RenameTable(
                name: "RolePermission",
                schema: "Identity",
                newName: "RolePermissions",
                newSchema: "Identity");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermission_PermissionId",
                schema: "Identity",
                table: "RolePermissions",
                newName: "IX_RolePermissions_PermissionId");

            migrationBuilder.AlterColumn<int>(
                name: "TotalReviews",
                schema: "Technician",
                table: "Technicians",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "Technician",
                table: "Technicians",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "None",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                schema: "Technician",
                table: "Technicians",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerified",
                schema: "Technician",
                table: "Technicians",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "CompletedBookings",
                schema: "Technician",
                table: "Technicians",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Technician",
                table: "Technicians",
                type: "nvarchar(450)",
                nullable: true);



            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                schema: "Technician",
                table: "TechnicianAvailabilities",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Service",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "BookingsId",
                schema: "Review",
                table: "Reviews",
                type: "int",
                nullable: true);



            migrationBuilder.AddColumn<int>(
                name: "BookingsId",
                schema: "Payment",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                schema: "Chat",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                schema: "Chat",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "Chat",
                table: "Messages",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Booking",
                table: "Media",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Booking",
                table: "Media",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UsedCount",
                schema: "Payment",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Payment",
                table: "Coupons",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Service",
                table: "Category",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                schema: "Booking",
                table: "Bookings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "Booking",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Governorate",
                schema: "Booking",
                table: "Bookings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "Booking",
                table: "Bookings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Area",
                schema: "Booking",
                table: "Bookings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "ReminderSent",
                schema: "Appointment",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                schema: "Appointment",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");




            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnicianServices",
                schema: "Technician",
                table: "TechnicianServices",
                columns: new[] { "TechnicianId", "ServiceId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnicianServiceAreas",
                schema: "Technician",
                table: "TechnicianServiceAreas",
                columns: new[] { "TechnicianId", "ServiceAreaId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                schema: "Identity",
                table: "RolePermissions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianServices_ServiceId",
                schema: "Technician",
                table: "TechnicianServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianServiceAreas_ServiceAreaId",
                schema: "Technician",
                table: "TechnicianServiceAreas",
                column: "ServiceAreaId");


            migrationBuilder.CreateIndex(
                name: "IX_Technicians_UserId",
                schema: "Technician",
                table: "Technicians",
                column: "UserId");


            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId",
                schema: "Review",
                table: "Reviews",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingsId",
                schema: "Review",
                table: "Reviews",
                column: "BookingsId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reviews_Rating",
                schema: "Review",
                table: "Reviews",
                sql: "[Rating] >= 1 AND [Rating] <= 5");


            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingsId",
                schema: "Payment",
                table: "Payments",
                column: "BookingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_BookingId_SentAt",
                schema: "Chat",
                table: "Messages",
                columns: new[] { "BookingId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                schema: "Chat",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_UserId",
                schema: "Booking",
                table: "Media",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId1",
                schema: "Identity",
                table: "RolePermissions",
                column: "PermissionId1");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "Identity",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId1",
                schema: "Identity",
                table: "RolePermissions",
                column: "RoleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Bookings_BookingId",
                schema: "Appointment",
                table: "Appointments",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_CustomerId",
                schema: "Booking",
                table: "Bookings",
                column: "CustomerId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Technicians_TechnicianId",
                schema: "Booking",
                table: "Bookings",
                column: "TechnicianId",
                principalSchema: "Technician",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServices_Bookings_BookingId",
                schema: "Booking",
                table: "BookingServices",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServices_Services_ServiceId",
                schema: "Booking",
                table: "BookingServices",
                column: "ServiceId",
                principalSchema: "Service",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Bookings_BookingId",
                schema: "Administration",
                table: "Complaints",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CouponUsages_AspNetUsers_UserId",
                schema: "Payment",
                table: "CouponUsages",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CouponUsages_Bookings_BookingId",
                schema: "Payment",
                table: "CouponUsages",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_AspNetUsers_UploadedByUserId",
                schema: "Booking",
                table: "Media",
                column: "UploadedByUserId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_AspNetUsers_UserId",
                schema: "Booking",
                table: "Media",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Bookings_BookingId",
                schema: "Booking",
                table: "Media",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                schema: "Chat",
                table: "Messages",
                column: "SenderId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Bookings_BookingId",
                schema: "Chat",
                table: "Messages",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                schema: "Payment",
                table: "Payments",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Bookings_BookingsId",
                schema: "Payment",
                table: "Payments",
                column: "BookingsId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Bookings_BookingId",
                schema: "Review",
                table: "Reviews",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Bookings_BookingsId",
                schema: "Review",
                table: "Reviews",
                column: "BookingsId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_AspNetRoles_RoleId",
                schema: "Identity",
                table: "RolePermissions",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);



            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permission_PermissionId",
                schema: "Identity",
                table: "RolePermissions",
                column: "PermissionId",
                principalSchema: "Identity",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);



            migrationBuilder.AddForeignKey(
                name: "FK_Services_Category_CategoryId",
                schema: "Service",
                table: "Services",
                column: "CategoryId",
                principalSchema: "Service",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);


            migrationBuilder.AddForeignKey(
                name: "FK_Technicians_AspNetUsers_UserId",
                schema: "Technician",
                table: "Technicians",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Bookings_BookingId",
                schema: "Appointment",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AspNetUsers_CustomerId",
                schema: "Booking",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Technicians_TechnicianId",
                schema: "Booking",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServices_Bookings_BookingId",
                schema: "Booking",
                table: "BookingServices");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServices_Services_ServiceId",
                schema: "Booking",
                table: "BookingServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Bookings_BookingId",
                schema: "Administration",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_CouponUsages_AspNetUsers_UserId",
                schema: "Payment",
                table: "CouponUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_CouponUsages_Bookings_BookingId",
                schema: "Payment",
                table: "CouponUsages");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_AspNetUsers_UploadedByUserId",
                schema: "Booking",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_AspNetUsers_UserId",
                schema: "Booking",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Bookings_BookingId",
                schema: "Booking",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                schema: "Chat",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Bookings_BookingId",
                schema: "Chat",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                schema: "Payment",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Bookings_BookingsId",
                schema: "Payment",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId1",
                schema: "Identity",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Bookings_BookingId",
                schema: "Review",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Bookings_BookingsId",
                schema: "Review",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_AspNetRoles_RoleId",
                schema: "Identity",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_AspNetRoles_RoleId1",
                schema: "Identity",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permission_PermissionId",
                schema: "Identity",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permission_PermissionId1",
                schema: "Identity",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Category_CategoryId",
                schema: "Service",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianEarning_Bookings_BookingId1",
                schema: "Technician",
                table: "TechnicianEarning");

            migrationBuilder.DropForeignKey(
                name: "FK_Technicians_AspNetUsers_UserId",
                schema: "Technician",
                table: "Technicians");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianWallet_Technicians_TechnicianId",
                schema: "Technician",
                table: "TechnicianWallet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnicianServices",
                schema: "Technician",
                table: "TechnicianServices");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianServices_ServiceId",
                schema: "Technician",
                table: "TechnicianServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnicianServiceAreas",
                schema: "Technician",
                table: "TechnicianServiceAreas");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianServiceAreas_ServiceAreaId",
                schema: "Technician",
                table: "TechnicianServiceAreas");

            migrationBuilder.DropIndex(
                name: "IX_Technicians_Status_IsVerified",
                schema: "Technician",
                table: "Technicians");

            migrationBuilder.DropIndex(
                name: "IX_Technicians_UserId",
                schema: "Technician",
                table: "Technicians");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianEarning_BookingId1",
                schema: "Technician",
                table: "TechnicianEarning");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BookingId",
                schema: "Review",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BookingsId",
                schema: "Review",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reviews_Rating",
                schema: "Review",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UserId1",
                schema: "Identity",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_Payments_BookingsId",
                schema: "Payment",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Messages_BookingId_SentAt",
                schema: "Chat",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderId",
                schema: "Chat",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Media_UserId",
                schema: "Booking",
                table: "Media");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                schema: "Identity",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_PermissionId1",
                schema: "Identity",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "Identity",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId1",
                schema: "Identity",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Technician",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "BookingId",
                schema: "Technician",
                table: "TechnicianEarning");

            migrationBuilder.DropColumn(
                name: "BookingsId",
                schema: "Review",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserId1",
                schema: "Identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "BookingsId",
                schema: "Payment",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Booking",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "PermissionId1",
                schema: "Identity",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "RoleId1",
                schema: "Identity",
                table: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                schema: "Identity",
                newName: "RolePermission",
                newSchema: "Identity");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_PermissionId",
                schema: "Identity",
                table: "RolePermission",
                newName: "IX_RolePermission_PermissionId");

            migrationBuilder.AlterColumn<int>(
                name: "TotalReviews",
                schema: "Technician",
                table: "Technicians",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "Technician",
                table: "Technicians",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "None");

            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                schema: "Technician",
                table: "Technicians",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 0.0);

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerified",
                schema: "Technician",
                table: "Technicians",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "CompletedBookings",
                schema: "Technician",
                table: "Technicians",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                schema: "Technician",
                table: "TechnicianAvailabilities",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Service",
                table: "Services",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                schema: "Chat",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                schema: "Chat",
                table: "Messages",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "Chat",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Booking",
                table: "Media",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UsedCount",
                schema: "Payment",
                table: "Coupons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Payment",
                table: "Coupons",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Service",
                table: "Category",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                schema: "Booking",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "Booking",
                table: "Bookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Governorate",
                schema: "Booking",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "Booking",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Area",
                schema: "Booking",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "ReminderSent",
                schema: "Appointment",
                table: "Appointments",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                schema: "Appointment",
                table: "Appointments",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnicianServices",
                schema: "Technician",
                table: "TechnicianServices",
                columns: new[] { "ServiceId", "TechnicianId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnicianServiceAreas",
                schema: "Technician",
                table: "TechnicianServiceAreas",
                columns: new[] { "ServiceAreaId", "TechnicianId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermission",
                schema: "Identity",
                table: "RolePermission",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianServices_TechnicianId",
                schema: "Technician",
                table: "TechnicianServices",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianServiceAreas_TechnicianId",
                schema: "Technician",
                table: "TechnicianServiceAreas",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId",
                schema: "Review",
                table: "Reviews",
                column: "BookingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Bookings_BookingId",
                schema: "Appointment",
                table: "Appointments",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AspNetUsers_CustomerId",
                schema: "Booking",
                table: "Bookings",
                column: "CustomerId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Technicians_TechnicianId",
                schema: "Booking",
                table: "Bookings",
                column: "TechnicianId",
                principalSchema: "Technician",
                principalTable: "Technicians",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServices_Bookings_BookingId",
                schema: "Booking",
                table: "BookingServices",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServices_Services_ServiceId",
                schema: "Booking",
                table: "BookingServices",
                column: "ServiceId",
                principalSchema: "Service",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Bookings_BookingId",
                schema: "Administration",
                table: "Complaints",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CouponUsages_AspNetUsers_UserId",
                schema: "Payment",
                table: "CouponUsages",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CouponUsages_Bookings_BookingId",
                schema: "Payment",
                table: "CouponUsages",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_AspNetUsers_UploadedByUserId",
                schema: "Booking",
                table: "Media",
                column: "UploadedByUserId",
                principalSchema: "Identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Bookings_BookingId",
                schema: "Booking",
                table: "Media",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Bookings_BookingId",
                schema: "Chat",
                table: "Messages",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                schema: "Payment",
                table: "Payments",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Bookings_BookingId",
                schema: "Review",
                table: "Reviews",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_AspNetRoles_RoleId",
                schema: "Identity",
                table: "RolePermission",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                schema: "Identity",
                table: "RolePermission",
                column: "PermissionId",
                principalSchema: "Identity",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Category_CategoryId",
                schema: "Service",
                table: "Services",
                column: "CategoryId",
                principalSchema: "Service",
                principalTable: "Category",
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
                onDelete: ReferentialAction.Restrict);
        }
    }
}
