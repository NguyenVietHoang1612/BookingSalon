using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSalon.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServicePromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsPromotionActive",
                table: "Services",
                type: "bit",
                nullable: false,
                computedColumnSql: "CASE \r\n                            WHEN [Promotion_Price] IS NOT NULL \r\n                                 AND [Promotion_Price] > 0\r\n                                 AND [Promotion_Start] <= GETDATE() \r\n                                 AND [Promotion_End] >= GETDATE() THEN CAST(1 AS BIT) \r\n                            ELSE CAST(0 AS BIT) \r\n                     END",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComputedColumnSql: "CASE \r\n                    WHEN [Promotion_Price] IS NOT NULL \r\n                         AND [Promotion_Start] <= GETDATE() \r\n                         AND [Promotion_End] >= GETDATE() THEN CAST(1 AS BIT) \r\n                    ELSE CAST(0 AS BIT) \r\n                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsPromotionActive",
                table: "Services",
                type: "bit",
                nullable: false,
                computedColumnSql: "CASE \r\n                    WHEN [Promotion_Price] IS NOT NULL \r\n                         AND [Promotion_Start] <= GETDATE() \r\n                         AND [Promotion_End] >= GETDATE() THEN CAST(1 AS BIT) \r\n                    ELSE CAST(0 AS BIT) \r\n                END",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComputedColumnSql: "CASE \r\n                            WHEN [Promotion_Price] IS NOT NULL \r\n                                 AND [Promotion_Price] > 0\r\n                                 AND [Promotion_Start] <= GETDATE() \r\n                                 AND [Promotion_End] >= GETDATE() THEN CAST(1 AS BIT) \r\n                            ELSE CAST(0 AS BIT) \r\n                     END");
        }
    }
}
