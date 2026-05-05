using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstCoreWebApp.Migrations
{
    public partial class AddIsRefreshTokenRevoked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRefreshTokenRevoked",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRefreshTokenRevoked",
                table: "Users");
        }
    }
}
