using Microsoft.EntityFrameworkCore.Migrations;

namespace MyRestaurant.Data.Migrations
{
    public partial class changeToDecimalRatingInWaiterRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "WaiterRate",
                nullable: false,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                table: "WaiterRate",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
