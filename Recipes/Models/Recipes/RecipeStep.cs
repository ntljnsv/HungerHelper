using System.ComponentModel.DataAnnotations;


namespace Recipes.Models.Recipes
{
    public class RecipeStep
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public int RecipeId { get; set; }   
        public int StepNumber { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}