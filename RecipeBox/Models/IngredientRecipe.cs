namespace RecipeBox.Models
{
  public class IngredientRecipe
  {
    public int IngredientRecipeId { get; set; }
    public int IngredientId { get; set; }
    public int RecipeId { get; set; }
    public int Quantity { get; set; }
    public virtual Ingredient Ingredient { get; set; }
    public virtual Recipe Recipe { get; set; }
  }
}