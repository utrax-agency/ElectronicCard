using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ElectronicCard.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ElectronicCard.Pages.Ecard.Register
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Phone number is required.")]
            [StringLength(10, ErrorMessage = "Phone number cannot exceed 10 characters.")]
            [RegularExpression(@"^\+?[0-9\-\(\)\.]{6,15}$", ErrorMessage = "Invalid phone number format.")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "First name is required.")]
            [StringLength(30, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 50 characters.")]
            [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "First name can only contain letters, spaces, hyphens and apostrophes.")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last name is required.")]
            [StringLength(30, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters.")]
            [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "Last name can only contain letters, spaces, hyphens and apostrophes.")]
            public string LastName { get; set; }
            [Required(ErrorMessage = "Password is required.")]
            [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters.")]
         
            public string Password { get; set; }



            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
            public string Role { get; set; }
        }

        public void OnGet()
        {
            try
            {
                _logger.LogInformation("Register page accessed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while accessing the register page.");
                TempData["ErrorMessage"] = "An error occurred while loading the register page. Please try again later.";
            }
        }
        [HttpGet]
        public async Task<IActionResult> OnGetCheckStatusAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User not found");

            return new JsonResult(new { status = user.status });
        }


        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Register form submission started.");

            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Model validation passed.");

                    // Check if the user already exists
                    var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                    if (existingUser != null)
                    {
                        _logger.LogWarning("User with email {Email} already exists.", Input.Email);
                        ModelState.AddModelError(string.Empty, "User with this email already exists.");
                        return Page();
                    }

                    var user = new ApplicationUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        PhoneNumber = Input.PhoneNumber,
                        status = null,
                        Role = Input.Role,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUserId = Input.Role
                    };

                    _logger.LogInformation("Creating a new user with email: {Email}", Input.Email);

                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User {Email} created successfully.", Input.Email);

                        // Assign the appropriate role to the user
                        if (!string.IsNullOrEmpty(Input.Role))
                        {
                            await _userManager.AddToRoleAsync(user, Input.Role);
                            _logger.LogInformation("User {Email} assigned to role: {Role}.", Input.Email, Input.Role);
                        }

                        // Sign in the user
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User {Email} signed in successfully.", Input.Email);

                        return RedirectToPage("/Ecard/SubmitDetails");
                    }

                    _logger.LogWarning("User creation failed for {Email}. Errors: {Errors}", Input.Email, string.Join(", ", result.Errors.Select(e => e.Description)));

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    _logger.LogWarning("Model validation failed.");

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
                _logger.LogError(ex, "An unexpected error occurred during the registration process.");
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
            }

            return Page();
        }
    }
}

