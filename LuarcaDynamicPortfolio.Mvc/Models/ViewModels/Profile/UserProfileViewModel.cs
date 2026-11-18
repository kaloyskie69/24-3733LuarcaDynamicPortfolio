using System;
using System.ComponentModel.DataAnnotations;

namespace LuarcaDynamicPortfolio.Mvc.Models.ViewModels.Profile
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(150, ErrorMessage = "Full Name cannot exceed 150 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string RoleName { get; set; } = "User";

        [Display(Name = "Date Registered")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Account Status")]
        public bool IsActive { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required.")]
        [StringLength(200, ErrorMessage = "Password cannot exceed 200 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be between 6 and 100 characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your new password.")]
        [StringLength(100, ErrorMessage = "Confirmation password cannot exceed 100 characters.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation do not match.")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
