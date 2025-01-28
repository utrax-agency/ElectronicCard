using ElectronicCard.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElectronicCard.Pages.Ecard
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        [Required]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }

        [BindProperty]
        [Required]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if passwords match
            if (Password != ConfirmPassword)
            {
                StatusMessage = "Passwords do not match.";
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                StatusMessage = "User not found.";
                return Page();
            }

            // Reset password
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, Password);

            if (result.Succeeded)
            {
                StatusMessage = "Password reset successfully.";
                return RedirectToPage("/Ecard/Login"); // Redirect to login page
            }
            else
            {
                StatusMessage = "Error resetting password.";
                return Page();
            }
        }
    }
}