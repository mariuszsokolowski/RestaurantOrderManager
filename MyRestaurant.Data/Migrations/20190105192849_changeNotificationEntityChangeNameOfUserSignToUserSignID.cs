using Microsoft.EntityFrameworkCore.Migrations;

namespace MyRestaurant.Data.Migrations
{
    public partial class changeNotificationEntityChangeNameOfUserSignToUserSignID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserSign",
                table: "Notification",
                newName: "UserSignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserSignId",
                table: "Notification",
                newName: "UserSign");
        }
    }
}
