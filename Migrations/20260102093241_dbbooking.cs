using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class dbbooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Branch_Image",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Booking_Date",
                table: "Bookings",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Branch_Image",
                table: "Branches");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Booking_Date",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
