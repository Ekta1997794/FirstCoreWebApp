using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstCoreWebApp.Migrations
{
    public partial class AddCategoryToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Courses");
        }
    }
}
