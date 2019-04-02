using Microsoft.EntityFrameworkCore.Migrations;

namespace RedisStudy.Migrations
{
    public partial class add_examorder1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DE_ExamOrder",
                columns: table => new
                {
                    ExamOrderID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Order = table.Column<int>(nullable: false),
                    ExamProjectID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_ExamOrder", x => x.ExamOrderID);
                    table.ForeignKey(
                        name: "FK_DE_ExamOrder_DE_ExamProject_ExamProjectID",
                        column: x => x.ExamProjectID,
                        principalTable: "DE_ExamProject",
                        principalColumn: "ExamProjectID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DE_ExamOrder_ExamProjectID",
                table: "DE_ExamOrder",
                column: "ExamProjectID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DE_ExamOrder");
        }
    }
}
