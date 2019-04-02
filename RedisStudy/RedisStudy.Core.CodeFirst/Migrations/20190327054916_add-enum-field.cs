using Microsoft.EntityFrameworkCore.Migrations;

namespace RedisStudy.Migrations
{
    public partial class addenumfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Result",
                table: "DE_Stuendt",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "DE_Stuendt");
        }
    }
}
