using Recipes.Models.Recipes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recipes.Models.RecipeUtil
{
    public class CreateRecipeVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a recipe title")]
        public string Title { get; set; }
        public string Category { get; set; }    
        public List<string> Categories { get; set; }
        [Required(ErrorMessage = "Please enter the cooking time")]
        public string TimeNeeded { get; set; }
        [Required(ErrorMessage = "Please enter the number of servings")]
        public string NumServings { get; set; }
        public Recipe Recipe { get; set; }
        public List<RecipeIngredient> Ingredients { get; set;}
        public List<RecipeStep> Steps { get; set; }
        public List<Content> Contents { get; set; }

    }
}