using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentStaffId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_StaffProfile_ProcessedBy",
                table: "Payments");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_ProcessedBy",
                table: "Payments",
                column: "ProcessedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_ProcessedBy",
                table: "Payments");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_StaffProfile_ProcessedBy",
                table: "Payments",
                column: "ProcessedBy",
                principalTable: "StaffProfile",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
