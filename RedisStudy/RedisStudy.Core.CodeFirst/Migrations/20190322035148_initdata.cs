using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RedisStudy.Migrations
{
    public partial class initdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DE_AuthType",
                columns: table => new
                {
                    AuthTypeID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    AuthTypeName = table.Column<string>(maxLength: 200, nullable: true),
                    InUse = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_AuthType", x => x.AuthTypeID);
                });

            migrationBuilder.CreateTable(
                name: "DE_ExamCar",
                columns: table => new
                {
                    ExamCarID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    CarPlat = table.Column<string>(maxLength: 20, nullable: true),
                    CarNumber = table.Column<string>(maxLength: 20, nullable: true),
                    CarStyle = table.Column<string>(maxLength: 20, nullable: true),
                    SubStyle = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_ExamCar", x => x.ExamCarID);
                });

            migrationBuilder.CreateTable(
                name: "DE_ExamProject",
                columns: table => new
                {
                    ExamProjectID = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ExamProjectName = table.Column<string>(maxLength: 100, nullable: true),
                    ExamProjectCode = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_ExamProject", x => x.ExamProjectID);
                });

            migrationBuilder.CreateTable(
                name: "DE_LostPointDefine",
                columns: table => new
                {
                    LostPointDefineID = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Reason = table.Column<string>(maxLength: 100, nullable: true),
                    LostPointCode = table.Column<string>(maxLength: 100, nullable: true),
                    LostPoint = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_LostPointDefine", x => x.LostPointDefineID);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRole<string>",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 127, nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NormalizedName = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole<string>", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUser<string>",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 127, nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<short>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<short>(nullable: false),
                    TwoFactorEnabled = table.Column<short>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<short>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser<string>", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    RoleId = table.Column<string>(nullable: true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    UserId = table.Column<string>(nullable: true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 127, nullable: true),
                    ProviderKey = table.Column<string>(maxLength: 127, nullable: true),
                    ProviderDisplayName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 127, nullable: false),
                    RoleId = table.Column<string>(maxLength: 127, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 127, nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 127, nullable: true),
                    Name = table.Column<string>(maxLength: 127, nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "DE_Stuendt",
                columns: table => new
                {
                    StudentID = table.Column<long>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    StudentName = table.Column<string>(maxLength: 40, nullable: true),
                    AuthTypeID = table.Column<int>(nullable: false),
                    AuthCode = table.Column<string>(maxLength: 100, nullable: true),
                    CarStyle = table.Column<string>(nullable: true),
                    SubCarStyle = table.Column<string>(nullable: true),
                    ExamCarID = table.Column<int>(nullable: false),
                    Score = table.Column<int>(nullable: true),
                    Passed = table.Column<short>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_Stuendt", x => x.StudentID);
                    table.ForeignKey(
                        name: "FK_DE_Stuendt_DE_AuthType_AuthTypeID",
                        column: x => x.AuthTypeID,
                        principalTable: "DE_AuthType",
                        principalColumn: "AuthTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DE_Stuendt_DE_ExamCar_ExamCarID",
                        column: x => x.ExamCarID,
                        principalTable: "DE_ExamCar",
                        principalColumn: "ExamCarID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DE_Exam",
                columns: table => new
                {
                    StudentID = table.Column<long>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    ExamBegin = table.Column<DateTime>(nullable: true),
                    ExamEnd = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_Exam", x => new { x.StudentID, x.Order });
                    table.ForeignKey(
                        name: "FK_DE_Exam_DE_Stuendt_StudentID",
                        column: x => x.StudentID,
                        principalTable: "DE_Stuendt",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DE_LostPoint",
                columns: table => new
                {
                    StudentID = table.Column<long>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    LostPointDefineID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_LostPoint", x => new { x.StudentID, x.Order, x.LostPointDefineID });
                    table.ForeignKey(
                        name: "FK_DE_LostPoint_DE_LostPointDefine_LostPointDefineID",
                        column: x => x.LostPointDefineID,
                        principalTable: "DE_LostPointDefine",
                        principalColumn: "LostPointDefineID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DE_LostPoint_DE_Exam_StudentID_Order",
                        columns: x => new { x.StudentID, x.Order },
                        principalTable: "DE_Exam",
                        principalColumns: new[] { "StudentID", "Order" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DE_StudentExamProject",
                columns: table => new
                {
                    StudentID = table.Column<long>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    ExamProjectID = table.Column<long>(nullable: false),
                    ExamBegin = table.Column<DateTime>(nullable: true),
                    ExamEnd = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_StudentExamProject", x => new { x.StudentID, x.Order, x.ExamProjectID });
                    table.ForeignKey(
                        name: "FK_DE_StudentExamProject_DE_ExamProject_ExamProjectID",
                        column: x => x.ExamProjectID,
                        principalTable: "DE_ExamProject",
                        principalColumn: "ExamProjectID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DE_StudentExamProject_DE_Exam_StudentID_Order",
                        columns: x => new { x.StudentID, x.Order },
                        principalTable: "DE_Exam",
                        principalColumns: new[] { "StudentID", "Order" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DE_ProjectLostPoint",
                columns: table => new
                {
                    StudentID = table.Column<long>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    ExamProjectID = table.Column<long>(nullable: false),
                    LostPointDefineID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_ProjectLostPoint", x => new { x.StudentID, x.Order, x.ExamProjectID, x.LostPointDefineID });
                    table.ForeignKey(
                        name: "FK_DE_ProjectLostPoint_DE_LostPointDefine_LostPointDefineID",
                        column: x => x.LostPointDefineID,
                        principalTable: "DE_LostPointDefine",
                        principalColumn: "LostPointDefineID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PLPoint_SProject_StudentID_Order_ExamProjectID",
                        columns: x => new { x.StudentID, x.Order, x.ExamProjectID },
                        principalTable: "DE_StudentExamProject",
                        principalColumns: new[] { "StudentID", "Order", "ExamProjectID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DE_LostPoint_LostPointDefineID",
                table: "DE_LostPoint",
                column: "LostPointDefineID");

            migrationBuilder.CreateIndex(
                name: "IX_DE_ProjectLostPoint_LostPointDefineID",
                table: "DE_ProjectLostPoint",
                column: "LostPointDefineID");

            migrationBuilder.CreateIndex(
                name: "IX_DE_StudentExamProject_ExamProjectID",
                table: "DE_StudentExamProject",
                column: "ExamProjectID");

            migrationBuilder.CreateIndex(
                name: "IX_DE_Stuendt_AuthTypeID",
                table: "DE_Stuendt",
                column: "AuthTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_DE_Stuendt_ExamCarID",
                table: "DE_Stuendt",
                column: "ExamCarID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DE_LostPoint");

            migrationBuilder.DropTable(
                name: "DE_ProjectLostPoint");

            migrationBuilder.DropTable(
                name: "IdentityRole<string>");

            migrationBuilder.DropTable(
                name: "IdentityUser<string>");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "DE_LostPointDefine");

            migrationBuilder.DropTable(
                name: "DE_StudentExamProject");

            migrationBuilder.DropTable(
                name: "DE_ExamProject");

            migrationBuilder.DropTable(
                name: "DE_Exam");

            migrationBuilder.DropTable(
                name: "DE_Stuendt");

            migrationBuilder.DropTable(
                name: "DE_AuthType");

            migrationBuilder.DropTable(
                name: "DE_ExamCar");
        }
    }
}
