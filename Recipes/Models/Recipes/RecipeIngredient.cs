using System.ComponentModel.DataAnnotations;

namespace Recipes.Models.Recipes
{
    public class RecipeIngredient
    {
        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; }
        public int IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; }
        public string Quantity { get; set; }
        public string Unit { get; set; }
    }
}