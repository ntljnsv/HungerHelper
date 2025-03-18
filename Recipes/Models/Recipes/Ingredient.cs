using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Recipes.Models.Recipes
{
    public class Ingredient
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}