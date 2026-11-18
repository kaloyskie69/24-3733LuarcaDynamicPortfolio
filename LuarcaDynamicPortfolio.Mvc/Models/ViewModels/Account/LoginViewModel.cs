using System.ComponentModel.DataAnnotations;

namespace LuarcaDynamicPortfolio.Mvc.Models.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username or email is required.")]
        [StringLength(150, ErrorMessage = "Username or email cannot exceed 150 characters.")]
        [Display(Name = "Username or Email")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(200, ErrorMessage = "Password cannot exceed 200 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
