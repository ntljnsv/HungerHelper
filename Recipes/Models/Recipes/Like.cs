using System;
using System.ComponentModel.DataAnnotations;


namespace Recipes.Models.Recipes
{
    public class Like
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateLiked { get; set; }
        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}