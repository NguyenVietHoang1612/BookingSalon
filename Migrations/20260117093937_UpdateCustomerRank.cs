using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerRank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdate",
                table: "CustomerRanks",
                newName: "UpdateAt");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "CustomerRanks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CustomerRanks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "CustomerRanks");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CustomerRanks");

            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "CustomerRanks",
                newName: "LastUpdate");
        }
    }
}
