using Microsoft.EntityFrameworkCore.Migrations;

namespace RedisStudy.Migrations
{
    public partial class addremovefield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Passed",
                table: "DE_Stuendt");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "DE_Stuendt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Passed",
                table: "DE_Stuendt",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "DE_Stuendt",
                nullable: true);
        }
    }
}
