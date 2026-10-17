using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Osta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveConversationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                schema: "Chat",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Conversations",
                schema: "Chat");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                schema: "Chat",
                table: "Messages",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ConversationId",
                schema: "Chat",
                table: "Messages",
                newName: "IX_Messages_BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Bookings_BookingId",
                schema: "Chat",
                table: "Messages",
                column: "BookingId",
                principalSchema: "Booking",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Bookings_BookingId",
                schema: "Chat",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                schema: "Chat",
                table: "Messages",
                newName: "ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_BookingId",
                schema: "Chat",
                table: "Messages",
                newName: "IX_Messages_ConversationId");

            migrationBuilder.CreateTable(
                name: "Conversations",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversations_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "Booking",
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversations_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalSchema: "Technician",
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_BookingId",
                schema: "Chat",
                table: "Conversations",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_CustomerId",
                schema: "Chat",
                table: "Conversations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_TechnicianId",
                schema: "Chat",
                table: "Conversations",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                schema: "Chat",
                table: "Messages",
                column: "ConversationId",
                principalSchema: "Chat",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
