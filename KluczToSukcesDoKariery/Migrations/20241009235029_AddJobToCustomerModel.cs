using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KluczToSukcesDoKariery.Migrations
{
    public partial class AddJobToCustomerModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerModel_UserId",
                table: "CustomerModel");

            migrationBuilder.AddColumn<string>(
                name: "Job",
                table: "CustomerModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerModel_UserId",
                table: "CustomerModel",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerModel_UserId",
                table: "CustomerModel");

            migrationBuilder.DropColumn(
                name: "Job",
                table: "CustomerModel");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerModel_UserId",
                table: "CustomerModel",
                column: "UserId");
        }
    }
}
