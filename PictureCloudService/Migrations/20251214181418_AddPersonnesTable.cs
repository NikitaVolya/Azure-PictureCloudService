using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PictureCloudService.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonnesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admin_Personne_PersonneId",
                table: "Admin");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Personne_PersonneId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Personne",
                table: "Personne");

            migrationBuilder.RenameTable(
                name: "Personne",
                newName: "Personnes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Personnes",
                table: "Personnes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Admin_Personnes_PersonneId",
                table: "Admin",
                column: "PersonneId",
                principalTable: "Personnes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Personnes_PersonneId",
                table: "Users",
                column: "PersonneId",
                principalTable: "Personnes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admin_Personnes_PersonneId",
                table: "Admin");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Personnes_PersonneId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Personnes",
                table: "Personnes");

            migrationBuilder.RenameTable(
                name: "Personnes",
                newName: "Personne");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Personne",
                table: "Personne",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Admin_Personne_PersonneId",
                table: "Admin",
                column: "PersonneId",
                principalTable: "Personne",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Personne_PersonneId",
                table: "Users",
                column: "PersonneId",
                principalTable: "Personne",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
