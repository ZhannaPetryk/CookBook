using System.ComponentModel.DataAnnotations;

namespace CookBook.WebUI.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your login")]
        public string UserName { get; set; }
         
        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}