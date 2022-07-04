using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace sup1.data.Migrations
{
    public partial class ArticleContentImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleContentImages",
                columns: table => new
                {
                    ArticleContentImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    ArticleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleContentImages", x => x.ArticleContentImageId);
                    table.ForeignKey(
                        name: "ForeignKey_ArticleContentImage",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ArticleContentImages",
                columns: new[] { "ArticleContentImageId", "ArticleId", "Name" },
                values: new object[] { 1, null, "article-content-image.png" });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleContentImages_ArticleId",
                table: "ArticleContentImages",
                column: "ArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleContentImages");
        }
    }
}
