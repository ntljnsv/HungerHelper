using System;
using System.Collections.Generic;
using Recipes.Models.Recipes;

namespace Recipes.Models.RecipeUtil
{
    public class RecipeDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<RecipeStep> Steps { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        public virtual ICollection<Content> Contents { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public int LikeCount { get; set; } = 0;
        public string TimeNeeded { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NumServings { get; set; }
        public string AuthorId { get; set; }
        public virtual Author Author { get; set; }
        public virtual ICollection<RecipeGroup> UserGroups { get; set; }
    }
}