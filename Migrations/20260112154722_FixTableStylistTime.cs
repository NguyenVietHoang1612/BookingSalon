using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class FixTableStylistTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_FixedTimeSlots_Slot_Id",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_stylistTimeWorks_FixedTimeSlots_Slot_Id",
                table: "stylistTimeWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_stylistTimeWorks_StylistProfiles_Stylist_Id",
                table: "stylistTimeWorks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stylistTimeWorks",
                table: "stylistTimeWorks");

            migrationBuilder.DropIndex(
                name: "IX_FixedTimeSlots_Sort_Order",
                table: "FixedTimeSlots");

            migrationBuilder.DropColumn(
                name: "Is_Available",
                table: "stylistTimeWorks");

            migrationBuilder.DropColumn(
                name: "StylistExperience",
                table: "StylistProfiles");

            migrationBuilder.DropColumn(
                name: "Sort_Order",
                table: "FixedTimeSlots");

            migrationBuilder.RenameTable(
                name: "stylistTimeWorks",
                newName: "StylistTimeWorks");

            //migrationBuilder.RenameIndex(
            //    name: "IX_stylistTimeWorks_Slot_Id",
            //    table: "StylistTimeWorks",
            //    newName: "IX_StylistTimeWorks_Slot_Id");

            migrationBuilder.CreateIndex(
                name: "IX_StylistTimeWorks_Slot_Id",
                table: "StylistTimeWorks",
                column: "Slot_Id");

            //migrationBuilder.RenameIndex(
            //    name: "IX_stylistTimeWorks_Stylist_Id_Work_Date_Slot_Id",
            //    table: "StylistTimeWorks",
            //    newName: "IX_Unique_Stylist_Slot_Per_Day");

            migrationBuilder.RenameColumn(
                name: "StylistSkill",
                table: "StylistProfiles",
                newName: "Bio");

            migrationBuilder.RenameColumn(
                name: "Slot_Id",
                table: "Bookings",
                newName: "Start_Slot_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_Slot_Id",
                table: "Bookings",
                newName: "IX_Bookings_Start_Slot_Id");

            migrationBuilder.AddColumn<int>(
                name: "Booking_Id",
                table: "StylistTimeWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Bookings",
                type: "int",
                maxLength: 1,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Booking_Date",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "End_Slot_Id",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StylistTimeWorks",
                table: "StylistTimeWorks",
                column: "Stylist_Time_Work_Id");

            migrationBuilder.CreateIndex(
                name: "IX_StylistTimeWorks_Booking_Id",
                table: "StylistTimeWorks",
                column: "Booking_Id");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTimeSlots_TimeLabel",
                table: "FixedTimeSlots",
                column: "TimeLabel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_End_Slot_Id",
                table: "Bookings",
                column: "End_Slot_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_FixedTimeSlots_End_Slot_Id",
                table: "Bookings",
                column: "End_Slot_Id",
                principalTable: "FixedTimeSlots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_FixedTimeSlots_Start_Slot_Id",
                table: "Bookings",
                column: "Start_Slot_Id",
                principalTable: "FixedTimeSlots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StylistTimeWorks_Bookings_Booking_Id",
                table: "StylistTimeWorks",
                column: "Booking_Id",
                principalTable: "Bookings",
                principalColumn: "Booking_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StylistTimeWorks_FixedTimeSlots_Slot_Id",
                table: "StylistTimeWorks",
                column: "Slot_Id",
                principalTable: "FixedTimeSlots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StylistTimeWorks_StylistProfiles_Stylist_Id",
                table: "StylistTimeWorks",
                column: "Stylist_Id",
                principalTable: "StylistProfiles",
                principalColumn: "StylistId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_FixedTimeSlots_End_Slot_Id",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_FixedTimeSlots_Start_Slot_Id",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_StylistTimeWorks_Bookings_Booking_Id",
                table: "StylistTimeWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_StylistTimeWorks_FixedTimeSlots_Slot_Id",
                table: "StylistTimeWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_StylistTimeWorks_StylistProfiles_Stylist_Id",
                table: "StylistTimeWorks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StylistTimeWorks",
                table: "StylistTimeWorks");

            migrationBuilder.DropIndex(
                name: "IX_StylistTimeWorks_Booking_Id",
                table: "StylistTimeWorks");

            migrationBuilder.DropIndex(
                name: "IX_FixedTimeSlots_TimeLabel",
                table: "FixedTimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_End_Slot_Id",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Booking_Id",
                table: "StylistTimeWorks");

            migrationBuilder.DropColumn(
                name: "End_Slot_Id",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "StylistTimeWorks",
                newName: "stylistTimeWorks");

            migrationBuilder.RenameIndex(
                name: "IX_StylistTimeWorks_Slot_Id",
                table: "stylistTimeWorks",
                newName: "IX_stylistTimeWorks_Slot_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Unique_Stylist_Slot_Per_Day",
                table: "stylistTimeWorks",
                newName: "IX_stylistTimeWorks_Stylist_Id_Work_Date_Slot_Id");

            migrationBuilder.RenameColumn(
                name: "Bio",
                table: "StylistProfiles",
                newName: "StylistSkill");

            migrationBuilder.RenameColumn(
                name: "Start_Slot_Id",
                table: "Bookings",
                newName: "Slot_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_Start_Slot_Id",
                table: "Bookings",
                newName: "IX_Bookings_Slot_Id");

            migrationBuilder.AddColumn<bool>(
                name: "Is_Available",
                table: "stylistTimeWorks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StylistExperience",
                table: "StylistProfiles",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sort_Order",
                table: "FixedTimeSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 1);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Booking_Date",
                table: "Bookings",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stylistTimeWorks",
                table: "stylistTimeWorks",
                column: "Stylist_Time_Work_Id");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTimeSlots_Sort_Order",
                table: "FixedTimeSlots",
                column: "Sort_Order",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_FixedTimeSlots_Slot_Id",
                table: "Bookings",
                column: "Slot_Id",
                principalTable: "FixedTimeSlots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stylistTimeWorks_FixedTimeSlots_Slot_Id",
                table: "stylistTimeWorks",
                column: "Slot_Id",
                principalTable: "FixedTimeSlots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stylistTimeWorks_StylistProfiles_Stylist_Id",
                table: "stylistTimeWorks",
                column: "Stylist_Id",
                principalTable: "StylistProfiles",
                principalColumn: "StylistId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
