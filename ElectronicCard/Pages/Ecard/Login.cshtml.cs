using ElectronicCard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ElectronicCard.Pages.Ecard.Login
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            try
            {
                ReturnUrl = returnUrl ?? Url.Content("~/Ecard/AddMember");
                _logger.LogInformation("Login page accessed. Return URL: {ReturnUrl}", ReturnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the login page.");
                TempData["ErrorMessage"] = "An error occurred while loading the page. Please try again.";
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                ReturnUrl = returnUrl ?? Url.Content("~/Ecard/ViewGroups");
                _logger.LogInformation("Login form submission started. Return URL: {ReturnUrl}", ReturnUrl);

                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Model validation passed. Attempting login for email: {Email}.", Input.Email);

                    var result = await _signInManager.PasswordSignInAsync(
                        Input.Email,
                        Input.Password,
                        Input.RememberMe,
                        lockoutOnFailure: false
                    );

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User {Email} logged in successfully.", Input.Email);

                        // Fetch the logged-in user
                        var user = await _userManager.FindByEmailAsync(Input.Email);
                        if (user != null)
                        {
                            if ((bool)!user.status)
                            {
                                _logger.LogInformation("User {Email} has a pending review status. Redirecting to ReviewPending.", Input.Email);
                                return LocalRedirect(Url.Content("~/Ecard/ReviewPending"));
                            }

                            if ((bool)user.status)
                            {
                                _logger.LogInformation("User {Email} found in database with active status.", Input.Email);

                                // Check if the user is in the "Admin" role
                                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                                if (isAdmin)
                                {
                                    _logger.LogInformation("Admin role detected for user {Email}. Redirecting to Province Overlook.", Input.Email);
                                    return LocalRedirect(Url.Content("~/Ecard/Admin/ProvinceOverlook"));
                                }
                                else
                                {
                                    _logger.LogInformation("Non-admin user {Email} logged in. Redirecting to default ReturnUrl.", Input.Email);
                                }
                            }

                            // Redirect active users to the default ReturnUrl
                            return LocalRedirect(ReturnUrl);
                        }
                        else
                        {
                            _logger.LogWarning("User {Email} could not be found in the database.", Input.Email);
                        }
                    }
                    else if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User {Email} account is locked out.", Input.Email);
                        return Page();
                    }
                    else
                    {
                        _logger.LogWarning("Invalid login attempt for {Email}.", Input.Email);
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                }
                else
                {
                    _logger.LogWarning("Model validation failed during login attempt for email: {Email}.", Input.Email);

                    // Log specific validation errors
                    var validationErrors = ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .Select(ms => new
                        {
                            Field = ms.Key,
                            Errors = ms.Value.Errors.Select(e => e.ErrorMessage)
                        });

                    foreach (var fieldError in validationErrors)
                    {
                        foreach (var error in fieldError.Errors)
                        {
                            _logger.LogWarning("Validation error on field {Field}: {Error}", fieldError.Field, error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during the login process for email: {Email}.", Input.Email);
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
            }

            // Return the login page if login fails or an exception occurs
            return Page();
        }
    }
}

