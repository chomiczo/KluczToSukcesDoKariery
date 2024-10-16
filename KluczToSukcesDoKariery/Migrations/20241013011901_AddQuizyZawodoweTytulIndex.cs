using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KluczToSukcesDoKariery.Migrations
{
    public partial class AddQuizyZawodoweTytulIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tytul",
                table: "QuizyZawodowe",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_QuizyZawodowe_Tytul",
                table: "QuizyZawodowe",
                column: "Tytul",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizyZawodowe_Tytul",
                table: "QuizyZawodowe");

            migrationBuilder.AlterColumn<string>(
                name: "Tytul",
                table: "QuizyZawodowe",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
