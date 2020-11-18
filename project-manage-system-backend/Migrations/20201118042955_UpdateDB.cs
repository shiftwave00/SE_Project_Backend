using Microsoft.EntityFrameworkCore.Migrations;

namespace project_manage_system_backend.Migrations
{
    public partial class UpdateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repositories_Users_OwnerAccount",
                table: "Repositories");

            migrationBuilder.DropIndex(
                name: "IX_Repositories_OwnerAccount",
                table: "Repositories");

            migrationBuilder.RenameColumn(
                name: "OwnerAccount",
                table: "Repositories",
                newName: "Owner");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Owner",
                table: "Repositories",
                newName: "OwnerAccount");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_OwnerAccount",
                table: "Repositories",
                column: "OwnerAccount");

            migrationBuilder.AddForeignKey(
                name: "FK_Repositories_Users_OwnerAccount",
                table: "Repositories",
                column: "OwnerAccount",
                principalTable: "Users",
                principalColumn: "Account",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
