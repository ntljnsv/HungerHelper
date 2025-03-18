using Recipes.Models.Recipes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Recipes.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<RecipeStep> Steps { get; set; } = new List<RecipeStep>();
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set;} = new List<RecipeIngredient>();
        public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public int LikeCount { get; set; } = 0;
        public string TimeNeeded { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NumServings { get; set; }
        public string AuthorId { get; set; }
        public virtual Author Author { get; set; }
        public virtual ICollection<RecipeGroup> Groups { get; set; }
    }
}