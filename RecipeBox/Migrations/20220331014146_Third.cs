using Microsoft.EntityFrameworkCore.Migrations;

namespace RecipeBox.Migrations
{
    public partial class Third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "IngredientRecipes",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientRecipes_UserId",
                table: "IngredientRecipes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientRecipes_AspNetUsers_UserId",
                table: "IngredientRecipes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientRecipes_AspNetUsers_UserId",
                table: "IngredientRecipes");

            migrationBuilder.DropIndex(
                name: "IX_IngredientRecipes_UserId",
                table: "IngredientRecipes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "IngredientRecipes");
        }
    }
}
