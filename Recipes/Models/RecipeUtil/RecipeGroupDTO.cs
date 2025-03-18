using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Recipes.Models.RecipeUtil
{
    public class RecipeGroupDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<RecipeBasicsDTO> Recipes { get; set; }
        public int Count { get; set; }
    }
}