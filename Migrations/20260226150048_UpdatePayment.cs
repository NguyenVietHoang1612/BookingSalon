using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_StaffProfile_StaffProfileStaffId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_StaffProfileStaffId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentResponseCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StaffProfileStaffId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Payments");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ProcessedBy",
                table: "Payments",
                column: "ProcessedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_StaffProfile_ProcessedBy",
                table: "Payments",
                column: "ProcessedBy",
                principalTable: "StaffProfile",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_StaffProfile_ProcessedBy",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ProcessedBy",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "PaymentResponseCode",
                table: "Payments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffProfileStaffId",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StaffProfileStaffId",
                table: "Payments",
                column: "StaffProfileStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_StaffProfile_StaffProfileStaffId",
                table: "Payments",
                column: "StaffProfileStaffId",
                principalTable: "StaffProfile",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
