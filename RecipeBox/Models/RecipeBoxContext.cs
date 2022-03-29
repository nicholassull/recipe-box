using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RecipeBox.Models
{
  //declaring that ApplicationUser is the type of IdentityDbContext we are inheriting
  public class RecipeBoxContext : IdentityDbContext<ApplicationUser>
  {
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<IngredientRecipe> IngredientRecipes { get; set; }
    public DbSet<CategoryRecipe> CategoryRecipes { get; set; }

    public RecipeBoxContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseLazyLoadingProxies();
    }
  }
}