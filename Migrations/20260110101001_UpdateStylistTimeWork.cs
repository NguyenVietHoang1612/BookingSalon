using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStylistTimeWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_stylistTimeWorks_Users_Stylist_Id",
                table: "stylistTimeWorks");

            migrationBuilder.AddForeignKey(
                name: "FK_stylistTimeWorks_StylistProfiles_Stylist_Id",
                table: "stylistTimeWorks",
                column: "Stylist_Id",
                principalTable: "StylistProfiles",
                principalColumn: "StylistId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_stylistTimeWorks_StylistProfiles_Stylist_Id",
                table: "stylistTimeWorks");

            migrationBuilder.AddForeignKey(
                name: "FK_stylistTimeWorks_Users_Stylist_Id",
                table: "stylistTimeWorks",
                column: "Stylist_Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
