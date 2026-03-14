using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStylistImageDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StylistImages_StylistProfiles_StylistProfileId",
                table: "StylistImages");

            migrationBuilder.AddForeignKey(
                name: "FK_StylistImages_StylistProfiles_StylistProfileId",
                table: "StylistImages",
                column: "StylistProfileId",
                principalTable: "StylistProfiles",
                principalColumn: "StylistId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StylistImages_StylistProfiles_StylistProfileId",
                table: "StylistImages");

            migrationBuilder.AddForeignKey(
                name: "FK_StylistImages_StylistProfiles_StylistProfileId",
                table: "StylistImages",
                column: "StylistProfileId",
                principalTable: "StylistProfiles",
                principalColumn: "StylistId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
