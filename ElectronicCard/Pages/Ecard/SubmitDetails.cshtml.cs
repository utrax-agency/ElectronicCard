using ElectronicCard.Models;
using ElectronicCard.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Identity;

namespace ElectronicCard.Pages.Ecard
{
    public class RegisterModel : PageModel
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<RegisterModel> _logger;
        private readonly MyAppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor to inject dependencies
        public RegisterModel(IWebHostEnvironment hostEnvironment, ILogger<RegisterModel> logger, MyAppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        [Required(ErrorMessage = "Please upload the front of your National ID")]
        public IFormFile NationalIdFront { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please upload the back of your National ID")]
        public IFormFile NationalIdBack { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please upload your selfie")]
        public IFormFile Selfie { get; set; }

        [BindNever]
        public string NationalIdFrontPath { get; set; }

        [BindNever]
        public string NationalIdBackPath { get; set; }

        [BindNever]
        public string SelfiePath { get; set; }

        public string ChairmanAccountId { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Register POST action started.");

            // Validate model state
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed. Errors: {@ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                return Page();
            }

            try
            {
                // Create the "uploads" folder if it doesn't exist
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                // Handle file uploads for National ID Front, Back, and Selfie
                if (NationalIdFront != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(NationalIdFront.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await NationalIdFront.CopyToAsync(fileStream);
                    }
                    NationalIdFrontPath = "/uploads/" + fileName;
                }

                if (NationalIdBack != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(NationalIdBack.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await NationalIdBack.CopyToAsync(fileStream);
                    }
                    NationalIdBackPath = "/uploads/" + fileName;
                }

                if (Selfie != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Selfie.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Selfie.CopyToAsync(fileStream);
                    }
                    SelfiePath = "/uploads/" + fileName;
                }

                // Fetch the logged-in user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found for the current session.");
                    ModelState.AddModelError("", "User not found.");
                    return Page();
                }

                // Create a ChairmanImages object to store the file paths and the user ID
                var chairmanImages = new ChairmanImages
                {
                    ChairmanAccountId = user.Id,  // Set ID from the logged-in user
                    NationalIdFrontPath = NationalIdFrontPath,
                    NationalIdBackPath = NationalIdBackPath,
                    SelfiePath = SelfiePath
                };

                // Add the new ChairmanImages to the database and save changes
                _context.ChairmanImages.Add(chairmanImages);
                await _context.SaveChangesAsync();

                // Redirect to ReviewPending page after saving
                return RedirectToPage("/Ecard/ReviewPending");
            }
            catch (Exception ex)
            {
                // Log error and add a model state error for unexpected issues
                _logger.LogError(ex, "Error uploading files.");
                ModelState.AddModelError("", "An error occurred while uploading files.");
                return Page();
            }
        }
    }
}

