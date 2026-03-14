using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_SkinnerId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_Stylist_Id",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SkinnerId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SkinnerId",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "Skinner_Id",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Skinner_Id",
                table: "Bookings",
                column: "Skinner_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_StaffProfile_Skinner_Id",
                table: "Bookings",
                column: "Skinner_Id",
                principalTable: "StaffProfile",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_StaffProfile_Stylist_Id",
                table: "Bookings",
                column: "Stylist_Id",
                principalTable: "StaffProfile",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_StaffProfile_Skinner_Id",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_StaffProfile_Stylist_Id",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_Skinner_Id",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "Skinner_Id",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "SkinnerId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SkinnerId",
                table: "Bookings",
                column: "SkinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_SkinnerId",
                table: "Bookings",
                column: "SkinnerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_Stylist_Id",
                table: "Bookings",
                column: "Stylist_Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
