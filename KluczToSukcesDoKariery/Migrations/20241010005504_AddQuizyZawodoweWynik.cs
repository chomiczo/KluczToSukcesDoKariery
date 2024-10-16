using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KluczToSukcesDoKariery.Migrations
{
    public partial class AddQuizyZawodoweWynik : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuizyZawodoweWynik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Wynik = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizyZawodoweWynik", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizyZawodoweWynik_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizyZawodoweWynik_QuizyZawodowe_QuizId",
                        column: x => x.QuizId,
                        principalTable: "QuizyZawodowe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizyZawodoweWynik_UserId",
                table: "QuizyZawodoweWynik",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizyZawodoweWynik");
        }
    }
}
