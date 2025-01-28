using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElectronicCard.Models;
using ElectronicCard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
namespace ElectronicCard.Pages.Ecard
{
        
    [Authorize(Roles = "Admin ")]
    public class ChairmanVerificationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MyAppDbContext _context;
        private readonly ILogger<ChairmanVerificationModel> _logger;

        public ChairmanVerificationModel(
            UserManager<ApplicationUser> userManager,
            MyAppDbContext context,
            ILogger<ChairmanVerificationModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public class ChairmanVerificationViewModel
        {
            public string ChairmanAccountId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string NationalIdFrontPath { get; set; }
            public string NationalIdBackPath { get; set; }
            public string SelfiePath { get; set; }
            public VerificationStatus Status { get; set; }
        }

        public enum VerificationStatus
        {
            Pending,
            Approved,
            Rejected
        }

        public List<ChairmanVerificationViewModel> Chairmen { get; set; }

        public async Task<IActionResult> OnGetAsync(string status)
        {
            try
            {
                _logger.LogInformation("Fetching chairman verification list...");

                // Initialize the queryable users collection
                IQueryable<ApplicationUser> usersQuery = _context.Users.Where(u => u.Role == "Chairman");

                // Apply filters based on status parameter
                if (status == "approved")
                {
                    // Query for approved users
                    usersQuery = usersQuery.Where(u => u.status == true);
                    _logger.LogInformation("Filtering for approved chairman users.");
                }
                else if (status == "rejected")
                {
                    // Query for rejected users
                    usersQuery = usersQuery.Where(u => u.status == false);
                    _logger.LogInformation("Filtering for rejected chairman users.");
                }
                else
                {
                    // Query for all users
                    _logger.LogInformation("Fetching all chairman users.");
                }

                // Execute the query and retrieve the users
                var users = await usersQuery.ToListAsync();

                _logger.LogInformation($"Retrieved {users.Count} chairman users.");

                // Retrieve chairman images linked to these users
                var chairmanIds = users.Select(u => u.Id).ToList();
                var chairmanImages = await _context.ChairmanImages
                    .Where(ci => chairmanIds.Contains(ci.ChairmanAccountId))
                    .ToListAsync();

                _logger.LogInformation($"Retrieved images for {chairmanImages.Count} chairmen.");

                // Map data to the view model
                Chairmen = users.Select(u =>
                {
                    var images = chairmanImages.FirstOrDefault(ci => ci.ChairmanAccountId == u.Id);

                    return new ChairmanVerificationViewModel
                    {
                        ChairmanAccountId = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        NationalIdFrontPath = images?.NationalIdFrontPath,
                        NationalIdBackPath = images?.NationalIdBackPath,
                        SelfiePath = images?.SelfiePath,
                        Status = DetermineVerificationStatus(u.status)
                    };
                }).ToList();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the chairman verification list.");
                return RedirectToPage("/Error");
            }
        }

        private static VerificationStatus DetermineVerificationStatus(bool? status)
        {
            return status switch
            {
                true => VerificationStatus.Approved,  // Approved
                false => VerificationStatus.Rejected, // Rejected
                null => VerificationStatus.Pending,   // Pending (if null)
            };
        }


        public async Task<IActionResult> OnPostUpdateStatusAsync(string userId, bool isApproved)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Invalid user ID provided.");
                ModelState.AddModelError(string.Empty, "Invalid user ID.");
                return Page();
            }

            try
            {
                _logger.LogInformation($"Updating verification status for user {userId} to {(isApproved ? "Approved" : "Rejected")}");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"No user found with ID {userId}");
                    return NotFound();
                }

                user.status = isApproved;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Successfully updated verification status for user {userId}.");
                    return RedirectToPage();
                }
                else
                {
                    _logger.LogError($"Failed to update verification status for user {userId}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    ModelState.AddModelError(string.Empty, "Failed to update user status.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating verification status for user {userId}");
            }

            return RedirectToPage("/Error");
        }

       
    }
}

