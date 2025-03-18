using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Recipes.Models.Recipes
{
    public abstract class Content
    {
        [Key]
        public int Id { get; set; }
        public int Order { get; set; }
        public abstract string ContentType { get; }
        public int RecipeId { get; set; }   
        public virtual Recipe Recipe { get; set; }
    }


    public class RecipeText : Content
    {
        [Column(TypeName = "nvarchar(max)")]
        public string Text { get; set; }

        public override string ContentType => "Text";
    }

    public class RecipePhoto : Content
    {
        public string Path { get; set; }
        public override string ContentType => "Photo";
    }
}