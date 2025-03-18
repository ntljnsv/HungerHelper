
namespace Recipes.Models.RecipeUtil
{
    public class RecipeBasicsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int LikeCount { get; set; }
        public string AuthorName { get; set; }
        public string NumServings { get; set; }
        public string TimeNeeded { get; set; }
        public string FirstPhotoPath { get; set; } 
        public string AuhtorId { get; set; }
    }
}