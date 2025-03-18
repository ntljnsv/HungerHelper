using PagedList;
using Recipes.Models.RecipeUtil;
using System.ComponentModel.DataAnnotations;

namespace Recipes.Models.ProfileUtil
{
    public class ProfileVM
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Bio { get; set; }
        public string PicturePath { get; set; }
        public IPagedList<RecipeBasicsDTO> Recipes;
        public IPagedList<RecipeBasicsDTO> LikedRecipes;

    }
}