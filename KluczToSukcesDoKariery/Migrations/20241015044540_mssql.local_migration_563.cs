using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KluczToSukcesDoKariery.Migrations
{
    public partial class mssqllocal_migration_563 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "QuizyZawodoweWynik",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizyZawodoweWynik_CustomerId",
                table: "QuizyZawodoweWynik",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizyZawodoweWynik_CustomerModel_CustomerId",
                table: "QuizyZawodoweWynik",
                column: "CustomerId",
                principalTable: "CustomerModel",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizyZawodoweWynik_CustomerModel_CustomerId",
                table: "QuizyZawodoweWynik");

            migrationBuilder.DropIndex(
                name: "IX_QuizyZawodoweWynik_CustomerId",
                table: "QuizyZawodoweWynik");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "QuizyZawodoweWynik");
        }
    }
}
