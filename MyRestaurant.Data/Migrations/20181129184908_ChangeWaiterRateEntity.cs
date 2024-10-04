using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyRestaurant.Data.Migrations
{
    public partial class ChangeWaiterRateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WaiterRate",
                table: "WaiterRate");

            migrationBuilder.AlterColumn<string>(
                name: "WaiterID",
                table: "WaiterRate",
                nullable: true,
                oldClrType: typeof(int))
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "WaiterRateID",
                table: "WaiterRate",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaiterRate",
                table: "WaiterRate",
                column: "WaiterRateID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WaiterRate",
                table: "WaiterRate");

            migrationBuilder.DropColumn(
                name: "WaiterRateID",
                table: "WaiterRate");

            migrationBuilder.AlterColumn<int>(
                name: "WaiterID",
                table: "WaiterRate",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaiterRate",
                table: "WaiterRate",
                column: "WaiterID");
        }
    }
}
