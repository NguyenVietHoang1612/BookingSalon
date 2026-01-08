using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class Updatedbmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "BookingDetails");

            migrationBuilder.DropColumn(
                name: "TotalDuration",
                table: "BookingDetails");

            migrationBuilder.AddColumn<int>(
                name: "TotalDuration",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalDuration",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BookingDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalDuration",
                table: "BookingDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
