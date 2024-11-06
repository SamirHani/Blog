using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Email is Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "UserName is Required")]
        [MinLength(4 , ErrorMessage ="Username must me longer than 3 characters")]
        [MaxLength(20, ErrorMessage = "Username must me less than 20 characters")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Passwrod is Required")]
        [MinLength(8 , ErrorMessage ="Password must be longer than 8 characters ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="Required")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
