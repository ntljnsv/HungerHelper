using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipes.Models.Recipes
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(350)]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}