using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RedisStudy.Migrations
{
    public partial class adddatetimefield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "HappenTime",
                table: "DE_ProjectLostPoint",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "HappenTime",
                table: "DE_LostPoint",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HappenTime",
                table: "DE_ProjectLostPoint");

            migrationBuilder.DropColumn(
                name: "HappenTime",
                table: "DE_LostPoint");
        }
    }
}
