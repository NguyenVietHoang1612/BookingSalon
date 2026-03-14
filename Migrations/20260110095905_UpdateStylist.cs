using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStylist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StylistProfileStylistId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StylistProfileStylistId",
                table: "Bookings",
                column: "StylistProfileStylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_StylistProfiles_StylistProfileStylistId",
                table: "Bookings",
                column: "StylistProfileStylistId",
                principalTable: "StylistProfiles",
                principalColumn: "StylistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_StylistProfiles_StylistProfileStylistId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_StylistProfileStylistId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "StylistProfileStylistId",
                table: "Bookings");
        }
    }
}
