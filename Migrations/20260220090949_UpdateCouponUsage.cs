using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCouponUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxUsagePerUser",
                table: "Coupon",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponCodeSnapshot",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountTypeSnapshot",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountValueSnapshot",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxUsagePerUser",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "CouponCodeSnapshot",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DiscountTypeSnapshot",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DiscountValueSnapshot",
                table: "Bookings");
        }
    }
}
