using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StylistImages_StylistProfiles_StylistId",
                table: "StylistImages");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StylistImages");

            migrationBuilder.RenameColumn(
                name: "StylistId",
                table: "StylistImages",
                newName: "StylistProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_StylistImages_StylistId",
                table: "StylistImages",
                newName: "IX_StylistImages_StylistProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_StylistImages_StylistProfiles_StylistProfileId",
                table: "StylistImages",
                column: "StylistProfileId",
                principalTable: "StylistProfiles",
                principalColumn: "StylistId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StylistImages_StylistProfiles_StylistProfileId",
                table: "StylistImages");

            migrationBuilder.RenameColumn(
                name: "StylistProfileId",
                table: "StylistImages",
                newName: "StylistId");

            migrationBuilder.RenameIndex(
                name: "IX_StylistImages_StylistProfileId",
                table: "StylistImages",
                newName: "IX_StylistImages_StylistId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StylistImages",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StylistImages_StylistProfiles_StylistId",
                table: "StylistImages",
                column: "StylistId",
                principalTable: "StylistProfiles",
                principalColumn: "StylistId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
