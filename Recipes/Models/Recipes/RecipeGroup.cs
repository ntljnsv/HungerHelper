using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recipes.Models.Recipes
{
    public class RecipeGroup
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a Group Name")]
        public string Name { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}