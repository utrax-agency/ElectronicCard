using ElectronicCard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
namespace ElectronicCard.Pages.Ecard
{

    [Authorize  (Roles = "Chairman")]
    public class VerificationLoadingModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<VerificationLoadingModel> _logger;

        public string VerificationMessage { get; set; } = "We are currently verifying your account details...";

        public VerificationLoadingModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<VerificationLoadingModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Simulate verification process
                await Task.Delay(2000);

                // Retrieve the currently logged-in user
                var user = await _userManager.GetUserAsync(User);

                // Check if the user is approved
                if (user != null && user.status == true) // Assuming 'status' is the field that stores approval state
                {
                    // User is approved, authenticate them and redirect to the ViewGroups page
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation($"User {user.UserName} is approved and redirected to ViewGroups.");
                    return RedirectToPage("/Ecard/ViewGroups");
                }
                else
                {
                    // User is not approved, show a message
                    VerificationMessage = "Your account is still pending verification.";
                    _logger.LogInformation($"User {user?.UserName} is not approved, awaiting verification.");
                    return Page(); // Stay on the verification loading page
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the verification process.");
                // Optionally, show a user-friendly error page or message
                VerificationMessage = "An error occurred while verifying your account. Please try again later.";
                return Page(); // Stay on the verification loading page
            }
        }
    }
}

