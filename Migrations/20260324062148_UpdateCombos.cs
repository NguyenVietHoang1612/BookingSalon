using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCombos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Combo_Id",
                table: "BookingDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_Combo_Id",
                table: "BookingDetails",
                column: "Combo_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_Combos_Combo_Id",
                table: "BookingDetails",
                column: "Combo_Id",
                principalTable: "Combos",
                principalColumn: "ComboId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetails_Combos_Combo_Id",
                table: "BookingDetails");

            migrationBuilder.DropIndex(
                name: "IX_BookingDetails_Combo_Id",
                table: "BookingDetails");

            migrationBuilder.DropColumn(
                name: "Combo_Id",
                table: "BookingDetails");
        }
    }
}
