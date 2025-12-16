using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PictureCloudService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeManyToManyCollectionPicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CollectionPictures",
                table: "CollectionPictures");

            migrationBuilder.DropIndex(
                name: "IX_CollectionPictures_CollectionId",
                table: "CollectionPictures");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CollectionPictures");

            migrationBuilder.AlterColumn<int>(
                name: "PictureId",
                table: "CollectionPictures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CollectionPictures",
                table: "CollectionPictures",
                columns: new[] { "CollectionId", "PictureId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CollectionPictures",
                table: "CollectionPictures");

            migrationBuilder.AlterColumn<int>(
                name: "PictureId",
                table: "CollectionPictures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CollectionPictures",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CollectionPictures",
                table: "CollectionPictures",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionPictures_CollectionId",
                table: "CollectionPictures",
                column: "CollectionId");
        }
    }
}
