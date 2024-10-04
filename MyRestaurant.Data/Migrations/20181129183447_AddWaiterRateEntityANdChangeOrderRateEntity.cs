using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyRestaurant.Data.Migrations
{
    public partial class AddWaiterRateEntityANdChangeOrderRateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rate");

            migrationBuilder.CreateTable(
                name: "OrderRate",
                columns: table => new
                {
                    RateID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    OrderLineId = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    Rating = table.Column<double>(nullable: false),
                    Cancled = table.Column<bool>(nullable: false),
                    UserSign = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRate", x => x.RateID);
                    table.ForeignKey(
                        name: "FK_OrderRate_OrderLine_OrderLineId",
                        column: x => x.OrderLineId,
                        principalTable: "OrderLine",
                        principalColumn: "OrderLineId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaiterRate",
                columns: table => new
                {
                    WaiterID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    NotificationID = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    Rating = table.Column<double>(nullable: false),
                    Cancled = table.Column<bool>(nullable: false),
                    UserSign = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaiterRate", x => x.WaiterID);
                    table.ForeignKey(
                        name: "FK_WaiterRate_Notification_NotificationID",
                        column: x => x.NotificationID,
                        principalTable: "Notification",
                        principalColumn: "NotificationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderRate_OrderLineId",
                table: "OrderRate",
                column: "OrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_WaiterRate_NotificationID",
                table: "WaiterRate",
                column: "NotificationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderRate");

            migrationBuilder.DropTable(
                name: "WaiterRate");

            migrationBuilder.CreateTable(
                name: "Rate",
                columns: table => new
                {
                    RateID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Cancled = table.Column<bool>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    OrderLineId = table.Column<int>(nullable: false),
                    Rating = table.Column<double>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UserSign = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rate", x => x.RateID);
                    table.ForeignKey(
                        name: "FK_Rate_OrderLine_OrderLineId",
                        column: x => x.OrderLineId,
                        principalTable: "OrderLine",
                        principalColumn: "OrderLineId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rate_OrderLineId",
                table: "Rate",
                column: "OrderLineId");
        }
    }
}
