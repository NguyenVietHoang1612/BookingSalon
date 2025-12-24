using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class FixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetails_Combos_Combo_Id",
                table: "BookingDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ComboServices_Combos_Combo_Id",
                table: "ComboServices");

            migrationBuilder.DropIndex(
                name: "IX_ComboServices_Combo_Id",
                table: "ComboServices");

            migrationBuilder.DropIndex(
                name: "IX_BookingDetails_Combo_Id",
                table: "BookingDetails");

            migrationBuilder.DropColumn(
                name: "Combo_Id",
                table: "ComboServices");

            migrationBuilder.DropColumn(
                name: "Combo_Id",
                table: "BookingDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Combo_Id",
                table: "ComboServices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Combo_Id",
                table: "BookingDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComboServices_Combo_Id",
                table: "ComboServices",
                column: "Combo_Id");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_Combo_Id",
                table: "BookingDetails",
                column: "Combo_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_Combos_Combo_Id",
                table: "BookingDetails",
                column: "Combo_Id",
                principalTable: "Combos",
                principalColumn: "Combo_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComboServices_Combos_Combo_Id",
                table: "ComboServices",
                column: "Combo_Id",
                principalTable: "Combos",
                principalColumn: "Combo_Id");
        }
    }
}
