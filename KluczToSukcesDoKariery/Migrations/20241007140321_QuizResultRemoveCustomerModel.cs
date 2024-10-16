using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KluczToSukcesDoKariery.Migrations
{
    public partial class QuizResultRemoveCustomerModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_CustomerModel_UserId1",
                table: "QuizResults");

            migrationBuilder.DropIndex(
                name: "IX_QuizResults_UserId1",
                table: "QuizResults");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "QuizResults");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerModelId",
                table: "QuizResults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_CustomerModelId",
                table: "QuizResults",
                column: "CustomerModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_CustomerModel_CustomerModelId",
                table: "QuizResults",
                column: "CustomerModelId",
                principalTable: "CustomerModel",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_CustomerModel_CustomerModelId",
                table: "QuizResults");

            migrationBuilder.DropIndex(
                name: "IX_QuizResults_CustomerModelId",
                table: "QuizResults");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerModelId",
                table: "QuizResults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "QuizResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_UserId1",
                table: "QuizResults",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_CustomerModel_UserId1",
                table: "QuizResults",
                column: "UserId1",
                principalTable: "CustomerModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
