using System.ComponentModel.DataAnnotations;

namespace Recipes.Models.ProfileUtil
{
    public class EditProfileVM
    {
        [Required(ErrorMessage = "Please enter your user name")]
        public string Name { get; set; }
        public string Bio { get; set; }
        public string PicturePath { get; set; }
    }
}