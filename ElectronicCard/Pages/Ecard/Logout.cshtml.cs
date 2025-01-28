using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ElectronicCard.Models;
using Microsoft.AspNetCore.Identity;


namespace ElectronicCard.Pages.Ecard
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            // Sign out the user
            await _signInManager.SignOutAsync();

            ////cleared user data
            HttpContext.Session.Clear();

            // Log the user out and redirect to the home page or any other page
            _logger.LogInformation("User logged out at {Time}.", DateTime.UtcNow);

            return RedirectToPage("/Ecard/Login"); 
        }
    }
}
