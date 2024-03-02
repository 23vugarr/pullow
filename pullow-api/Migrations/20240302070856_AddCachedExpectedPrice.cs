using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pullow_api.Migrations
{
    public partial class AddCachedExpectedPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cached_expected_price",
                table: "goals",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cached_expected_price",
                table: "goals");
        }
    }
}
