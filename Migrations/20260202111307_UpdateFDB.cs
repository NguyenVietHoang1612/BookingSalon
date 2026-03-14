using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Branches_Branch_Id",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Ward_WardId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRanks_Ranks_RankId",
                table: "CustomerRanks");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRanks_Users_Customer_Id",
                table: "CustomerRanks");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Bookings_BookingId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_StaffId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffProfile_Branches_Branch_Id",
                table: "StaffProfile");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_StylistTimeWorks_Bookings_Booking_Id",
            //    table: "StylistTimeWorks");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_StaffId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StylistTimeWorks",
                table: "StylistTimeWorks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerRanks",
                table: "CustomerRanks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Branches",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "StaffProfile");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "StaffPortfolio");

            migrationBuilder.DropColumn(
                name: "Create_At",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Update_At",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "StylistTimeWorks",
                newName: "StaffSchedule");

            migrationBuilder.RenameTable(
                name: "CustomerRanks",
                newName: "CustomerRank");

            migrationBuilder.RenameTable(
                name: "Branches",
                newName: "Branch");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Services",
                newName: "Base_Price");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "Reviews",
                newName: "Booking_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_BookingId",
                table: "Reviews",
                newName: "IX_Reviews_Booking_Id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "News",
                newName: "Updated_At");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "News",
                newName: "Created_At");

            //migrationBuilder.RenameIndex(
            //    name: "IX_StylistTimeWorks_Slot_Id",
            //    table: "StaffSchedule",
            //    newName: "IX_StaffSchedule_Slot_Id");

            //migrationBuilder.RenameIndex(
            //    name: "IX_StylistTimeWorks_Booking_Id",
            //    table: "StaffSchedule",
            //    newName: "IX_StaffSchedule_Booking_Id");

            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "CustomerRank",
                newName: "Update_At");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "CustomerRank",
                newName: "Created_At");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "CustomerRank",
                newName: "Is_Active");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerRanks_RankId",
                table: "CustomerRank",
                newName: "IX_CustomerRank_RankId");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Branch",
                newName: "PhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Branches_WardId",
                table: "Branch",
                newName: "IX_Branch_WardId");

            migrationBuilder.RenameIndex(
                name: "IX_Branches_Branch_Name",
                table: "Branch",
                newName: "IX_Branch_Branch_Name");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Avatar_Name",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Users",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "TypeOfServices",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Type_Service_Name",
                table: "TypeOfServices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "StaffProfile",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Start_Work_Time",
                table: "StaffProfile",
                type: "time",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "End_Work_Time",
                table: "StaffProfile",
                type: "time",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<string>(
                name: "Staff_Bio",
                table: "StaffProfile",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "StaffPortfolio",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "Services",
                type: "nvarchar(max)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "Services",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Service_Name",
                table: "Services",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "ImageName",
                table: "Services",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Promotion_End",
                table: "Services",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<decimal>(
                name: "Promotion_Price",
                table: "Services",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Promotion_Start",
                table: "Services",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created_At",
                table: "Reviews",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Reviews",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Customer_Id",
                table: "Reviews",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RepliedById",
                table: "Reviews",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reply_Comment",
                table: "Reviews",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Staff_Id",
                table: "Reviews",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated_At",
                table: "Reviews",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Ranks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created_At",
                table: "Ranks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_At",
                table: "Ranks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "News",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Thumbnail",
                table: "News",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "News",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "FixedTimeSlots",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TimeLabel",
                table: "FixedTimeSlots",
                type: "time",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Bookings",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Create_At",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "Coupon_Id",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPrice",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Bookings",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "BookingDetails",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "StaffSchedule",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Branch_Name",
                table: "Branch",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Branch_Image",
                table: "Branch",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Branch",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<bool>(
                name: "Is_Main_Branch",
                table: "Branch",
                type: "bit",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffSchedule",
                table: "StaffSchedule",
                column: "ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerRank",
                table: "CustomerRank",
                column: "Customer_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Branch",
                table: "Branch",
                column: "BranchId");

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Discount_Type = table.Column<int>(type: "int", nullable: false),
                    Discount_value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Min_Order_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Usage_Limit = table.Column<int>(type: "int", nullable: false),
                    Used_Count = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Expires_At = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RepliedById",
                table: "Reviews",
                column: "RepliedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Staff_Id",
                table: "Reviews",
                column: "Staff_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Coupon_Id",
                table: "Bookings",
                column: "Coupon_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UpdatedById",
                table: "Bookings",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Branch_Branch_Id",
                table: "Bookings",
                column: "Branch_Id",
                principalTable: "Branch",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Coupon_Coupon_Id",
                table: "Bookings",
                column: "Coupon_Id",
                principalTable: "Coupon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UpdatedById",
                table: "Bookings",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Ward_WardId",
                table: "Branch",
                column: "WardId",
                principalTable: "Ward",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRank_Ranks_RankId",
                table: "CustomerRank",
                column: "RankId",
                principalTable: "Ranks",
                principalColumn: "RankId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRank_Users_Customer_Id",
                table: "CustomerRank",
                column: "Customer_Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Bookings_Booking_Id",
                table: "Reviews",
                column: "Booking_Id",
                principalTable: "Bookings",
                principalColumn: "Booking_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_CustomerId",
                table: "Reviews",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_RepliedById",
                table: "Reviews",
                column: "RepliedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_Staff_Id",
                table: "Reviews",
                column: "Staff_Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffProfile_Branch_Branch_Id",
                table: "StaffProfile",
                column: "Branch_Id",
                principalTable: "Branch",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffSchedule_Bookings_Booking_Id",
                table: "StaffSchedule",
                column: "Booking_Id",
                principalTable: "Bookings",
                principalColumn: "Booking_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffSchedule_FixedTimeSlots_Slot_Id",
                table: "StaffSchedule",
                column: "Slot_Id",
                principalTable: "FixedTimeSlots",
                principalColumn: "SlotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffSchedule_StaffProfile_Staff_Id",
                table: "StaffSchedule",
                column: "Staff_Id",
                principalTable: "StaffProfile",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Branch_Branch_Id",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Coupon_Coupon_Id",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UpdatedById",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Ward_WardId",
                table: "Branch");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRank_Ranks_RankId",
                table: "CustomerRank");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRank_Users_Customer_Id",
                table: "CustomerRank");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Bookings_Booking_Id",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_CustomerId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_RepliedById",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_Staff_Id",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffProfile_Branch_Branch_Id",
                table: "StaffProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffSchedule_Bookings_Booking_Id",
                table: "StaffSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffSchedule_FixedTimeSlots_Slot_Id",
                table: "StaffSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffSchedule_StaffProfile_Staff_Id",
                table: "StaffSchedule");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_RepliedById",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_Staff_Id",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_Coupon_Id",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UpdatedById",
                table: "Bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffSchedule",
                table: "StaffSchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerRank",
                table: "CustomerRank");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Branch",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "Staff_Bio",
                table: "StaffProfile");

            migrationBuilder.DropColumn(
                name: "Promotion_End",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Promotion_Price",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Promotion_Start",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Created_At",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "RepliedById",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Reply_Comment",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Staff_Id",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Updated_At",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Created_At",
                table: "Ranks");

            migrationBuilder.DropColumn(
                name: "Update_At",
                table: "Ranks");

            migrationBuilder.DropColumn(
                name: "Coupon_Id",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "FinalPrice",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Is_Main_Branch",
                table: "Branch");

            migrationBuilder.RenameTable(
                name: "StaffSchedule",
                newName: "StylistTimeWorks");

            migrationBuilder.RenameTable(
                name: "CustomerRank",
                newName: "CustomerRanks");

            migrationBuilder.RenameTable(
                name: "Branch",
                newName: "Branches");

            migrationBuilder.RenameColumn(
                name: "Base_Price",
                table: "Services",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "Booking_Id",
                table: "Reviews",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_Booking_Id",
                table: "Reviews",
                newName: "IX_Reviews_BookingId");

            migrationBuilder.RenameColumn(
                name: "Updated_At",
                table: "News",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "News",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_StaffSchedule_Slot_Id",
                table: "StylistTimeWorks",
                newName: "IX_StylistTimeWorks_Slot_Id");

            migrationBuilder.RenameIndex(
                name: "IX_StaffSchedule_Booking_Id",
                table: "StylistTimeWorks",
                newName: "IX_StylistTimeWorks_Booking_Id");

            migrationBuilder.RenameColumn(
                name: "Update_At",
                table: "CustomerRanks",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "Is_Active",
                table: "CustomerRanks",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "CustomerRanks",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerRank_RankId",
                table: "CustomerRanks",
                newName: "IX_CustomerRanks_RankId");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Branches",
                newName: "Phone");

            migrationBuilder.RenameIndex(
                name: "IX_Branch_WardId",
                table: "Branches",
                newName: "IX_Branches_WardId");

            migrationBuilder.RenameIndex(
                name: "IX_Branch_Branch_Name",
                table: "Branches",
                newName: "IX_Branches_Branch_Name");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Avatar_Name",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "TypeOfServices",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Type_Service_Name",
                table: "TypeOfServices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "StaffProfile",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Start_Work_Time",
                table: "StaffProfile",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "End_Work_Time",
                table: "StaffProfile",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "StaffProfile",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "StaffPortfolio",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "StaffPortfolio",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "Services",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "Services",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Service_Name",
                table: "Services",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "ImageName",
                table: "Services",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_At",
                table: "Reviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "StaffId",
                table: "Reviews",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_At",
                table: "Reviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Ranks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "News",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Thumbnail",
                table: "News",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "News",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "FixedTimeSlots",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TimeLabel",
                table: "FixedTimeSlots",
                type: "time",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Bookings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Create_At",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "BookingDetails",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Update_At",
                table: "StylistTimeWorks",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Branch_Name",
                table: "Branches",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "Branch_Image",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Branches",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StylistTimeWorks",
                table: "StylistTimeWorks",
                column: "ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerRanks",
                table: "CustomerRanks",
                column: "Customer_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Branches",
                table: "Branches",
                column: "BranchId");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Payment_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypePay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Update_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Booking_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StaffId",
                table: "Reviews",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Branches_Branch_Id",
                table: "Bookings",
                column: "Branch_Id",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Ward_WardId",
                table: "Branches",
                column: "WardId",
                principalTable: "Ward",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRanks_Ranks_RankId",
                table: "CustomerRanks",
                column: "RankId",
                principalTable: "Ranks",
                principalColumn: "RankId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRanks_Users_Customer_Id",
                table: "CustomerRanks",
                column: "Customer_Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Bookings_BookingId",
                table: "Reviews",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Booking_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_StaffId",
                table: "Reviews",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffProfile_Branches_Branch_Id",
                table: "StaffProfile",
                column: "Branch_Id",
                principalTable: "Branches",
                principalColumn: "BranchId",
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
                name: "FK_StylistTimeWorks_StaffProfile_Staff_Id",
                table: "StylistTimeWorks",
                column: "Staff_Id",
                principalTable: "StaffProfile",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
