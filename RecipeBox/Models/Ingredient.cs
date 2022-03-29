using System.Collections.Generic;

namespace RecipeBox.Models
{
  public class Ingredient
  {
    public Ingredient()
    {
      this.JoinEntities = new HashSet<IngredientRecipe>();
    }

    public int IngredientId { get; set; }
    public string Name { get; set; }
    public virtual ICollection<IngredientRecipe> JoinEntities {get; set; }
  }
}