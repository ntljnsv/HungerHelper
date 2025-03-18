using System.Collections.Generic;


namespace Recipes.Models.RecipeUtil
{
    public class SearchFilter
    {
        public string Query { get; set; }
        public List<int> IngredientIds { get; set; }    
        public string Category { get; set; }
    }
}