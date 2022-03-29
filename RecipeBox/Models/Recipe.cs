using System.Collections.Generic;

namespace RecipeBox.Models
{
  public class Recipe
  {

    public Recipe()
    {
      this.JoinIngredientRecipes = new HashSet<IngredientRecipe>();
      this.JoinCategoryRecipes = new HashSet<CategoryRecipe>();
    }

    public int RecipeId { get; set; }
    public string Name { get; set; }
    public string Instructions { get; set;}

    public int Rating { get; set; }
    public virtual ApplicationUser User { get; set; }
    public virtual ICollection<IngredientRecipe> JoinIngredientRecipes { get; set; }
    public virtual ICollection<CategoryRecipe> JoinCategoryRecipes { get; set; }

  }
}