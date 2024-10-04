using Microsoft.EntityFrameworkCore.Migrations;

namespace MyRestaurant.Data.Migrations
{
    public partial class AddCancledToRateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Cancled",
                table: "Rate",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cancled",
                table: "Rate");
        }
    }
}
