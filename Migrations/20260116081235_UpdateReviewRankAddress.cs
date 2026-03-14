using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviewRankAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StylistTimeWorks_StylistProfiles_Stylist_Id",
                table: "StylistTimeWorks");

            migrationBuilder.DropTable(
                name: "EyeCatchingHairstyle");

            migrationBuilder.DropTable(
                name: "StylistProfiles");

            migrationBuilder.RenameColumn(
                name: "Stylist_Id",
                table: "StylistTimeWorks",
                newName: "Staff_Id");

            migrationBuilder.RenameColumn(
                name: "Stylist_Time_Work_Id",
                table: "StylistTimeWorks",
                newName: "ScheduleId");

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SkinnerId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skinner_Id",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    ProvinceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.ProvinceId);
                });

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    RankId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RankName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinPoint = table.Column<int>(type: "int", nullable: false),
                    MaxBookingDays = table.Column<int>(type: "int", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.RankId);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Create_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Booking_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffProfile",
                columns: table => new
                {
                    StaffId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Branch_Id = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Start_Work_Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    End_Work_Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    Create_At = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Update_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffProfile", x => x.StaffId);
                    table.ForeignKey(
                        name: "FK_StaffProfile_Branches_Branch_Id",
                        column: x => x.Branch_Id,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffProfile_Users_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.DistrictId);
                    table.ForeignKey(
                        name: "FK_Districts_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "ProvinceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerRanks",
                columns: table => new
                {
                    Customer_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CurrentPoints = table.Column<int>(type: "int", nullable: false),
                    LifetimePoints = table.Column<int>(type: "int", nullable: false),
                    RankId = table.Column<int>(type: "int", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRanks", x => x.Customer_Id);
                    table.ForeignKey(
                        name: "FK_CustomerRanks_Ranks_RankId",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "RankId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerRanks_Users_Customer_Id",
                        column: x => x.Customer_Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffPortfolio",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    Create_At = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Update_At = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffPortfolio", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_StaffPortfolio_StaffProfile_StaffProfileId",
                        column: x => x.StaffProfileId,
                        principalTable: "StaffProfile",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    WardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.WardId);
                    table.ForeignKey(
                        name: "FK_Wards_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "DistrictId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_WardId",
                table: "Users",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_WardId",
                table: "Branches",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SkinnerId",
                table: "Bookings",
                column: "SkinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRanks_RankId",
                table: "CustomerRanks",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ProvinceId",
                table: "Districts",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId",
                table: "Reviews",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StaffId",
                table: "Reviews",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffPortfolio_StaffProfileId",
                table: "StaffPortfolio",
                column: "StaffProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffProfile_Branch_Id",
                table: "StaffProfile",
                column: "Branch_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_DistrictId",
                table: "Wards",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_SkinnerId",
                table: "Bookings",
                column: "SkinnerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Wards_WardId",
                table: "Branches",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "WardId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StylistTimeWorks_StaffProfile_Staff_Id",
                table: "StylistTimeWorks",
                column: "Staff_Id",
                principalTable: "StaffProfile",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Wards_WardId",
                table: "Users",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "WardId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_SkinnerId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Wards_WardId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_StylistTimeWorks_StaffProfile_Staff_Id",
                table: "StylistTimeWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Wards_WardId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "CustomerRanks");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "StaffPortfolio");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropTable(
                name: "StaffProfile");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_Users_WardId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Branches_WardId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SkinnerId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "SkinnerId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Skinner_Id",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "Staff_Id",
                table: "StylistTimeWorks",
                newName: "Stylist_Id");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "StylistTimeWorks",
                newName: "Stylist_Time_Work_Id");

            migrationBuilder.CreateTable(
                name: "StylistProfiles",
                columns: table => new
                {
                    StylistId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Branch_Id = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Create_At = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    End_Work_Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    Start_Work_Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    Update_At = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StylistProfiles", x => x.StylistId);
                    table.ForeignKey(
                        name: "FK_StylistProfiles_Branches_Branch_Id",
                        column: x => x.Branch_Id,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StylistProfiles_Users_StylistId",
                        column: x => x.StylistId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EyeCatchingHairstyle",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StylistProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContentHair = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Create_At = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TitleHair = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Update_At = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EyeCatchingHairstyle", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_EyeCatchingHairstyle_StylistProfiles_StylistProfileId",
                        column: x => x.StylistProfileId,
                        principalTable: "StylistProfiles",
                        principalColumn: "StylistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EyeCatchingHairstyle_StylistProfileId",
                table: "EyeCatchingHairstyle",
                column: "StylistProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StylistProfiles_Branch_Id",
                table: "StylistProfiles",
                column: "Branch_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StylistTimeWorks_StylistProfiles_Stylist_Id",
                table: "StylistTimeWorks",
                column: "Stylist_Id",
                principalTable: "StylistProfiles",
                principalColumn: "StylistId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
