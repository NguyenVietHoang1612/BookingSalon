using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class dbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_StylistProfiles_Stylist_Profile_Id",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_TypeOfServices_Type_Service",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_StylistProfiles_Users_StylistId",
                table: "StylistProfiles");

            migrationBuilder.RenameColumn(
                name: "Type_Service",
                table: "Services",
                newName: "Type_Service_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Services_Type_Service",
                table: "Services",
                newName: "IX_Services_Type_Service_Id");

            migrationBuilder.RenameColumn(
                name: "Stylist_Profile_Id",
                table: "Bookings",
                newName: "Stylist_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_Stylist_Profile_Id",
                table: "Bookings",
                newName: "IX_Bookings_Stylist_Id");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageName",
                table: "Services",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Branches",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BookingDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FullName",
                table: "Users",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_Branch_Name",
                table: "Branches",
                column: "Branch_Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_Stylist_Id",
                table: "Bookings",
                column: "Stylist_Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_TypeOfServices_Type_Service_Id",
                table: "Services",
                column: "Type_Service_Id",
                principalTable: "TypeOfServices",
                principalColumn: "TypeOfServiceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StylistProfiles_Users_StylistId",
                table: "StylistProfiles",
                column: "StylistId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_Stylist_Id",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_TypeOfServices_Type_Service_Id",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_StylistProfiles_Users_StylistId",
                table: "StylistProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FullName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Branches_Branch_Name",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Type_Service_Id",
                table: "Services",
                newName: "Type_Service");

            migrationBuilder.RenameIndex(
                name: "IX_Services_Type_Service_Id",
                table: "Services",
                newName: "IX_Services_Type_Service");

            migrationBuilder.RenameColumn(
                name: "Stylist_Id",
                table: "Bookings",
                newName: "Stylist_Profile_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_Stylist_Id",
                table: "Bookings",
                newName: "IX_Bookings_Stylist_Profile_Id");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageName",
                table: "Services",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "BookingDetails",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_StylistProfiles_Stylist_Profile_Id",
                table: "Bookings",
                column: "Stylist_Profile_Id",
                principalTable: "StylistProfiles",
                principalColumn: "StylistId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_TypeOfServices_Type_Service",
                table: "Services",
                column: "Type_Service",
                principalTable: "TypeOfServices",
                principalColumn: "TypeOfServiceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StylistProfiles_Users_StylistId",
                table: "StylistProfiles",
                column: "StylistId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
