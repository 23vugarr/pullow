using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pullow_api.Migrations
{
    public partial class AddGrossSalary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "gross_salary",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "gross_salary",
                table: "AspNetUsers");
        }
    }
}
