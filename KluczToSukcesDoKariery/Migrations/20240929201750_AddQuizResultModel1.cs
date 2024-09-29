using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KluczToSukcesDoKariery.Migrations
{
    public partial class AddQuizResultModel1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_AspNetUsers_ApplicationUserId",
                table: "QuizResults");

            migrationBuilder.DropIndex(
                name: "IX_QuizResults_ApplicationUserId",
                table: "QuizResults");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "QuizResults");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "QuizResults",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_ApplicationUserId",
                table: "QuizResults",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_AspNetUsers_ApplicationUserId",
                table: "QuizResults",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
