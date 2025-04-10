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
            public bool? Status { get; set; }
        }

        public enum VerificationStatus
        {
            Pending,
            Approved,
            Rejected
        }

        public List<ChairmanVerificationViewModel> Chairmen { get; set; }

        public async Task<IActionResult> OnGetAsync(string status = "all", int pageNumber = 1, int pageSize = 12)
        {
            try
            {
                _logger.LogInformation("Fetching chairman verification list with status: {Status}, Page: {PageNumber}", status, pageNumber);

                // Base query for chairman users
                IQueryable<ApplicationUser> usersQuery = _context.Users.Where(u => u.Role == "Chairman");

                // Apply filters based on status parameter
                switch (status.ToLower())
                {
                    case "approved":
                        usersQuery = usersQuery.Where(u => u.status == true);
                        break;
                    case "rejected":
                        usersQuery = usersQuery.Where(u => u.status == false);
                        break;
                    case "pending":
                        usersQuery = usersQuery.Where(u => u.status == null);
                        break;
                    case "all":
                    default:
                        break;
                }

                // Get total number of records (for pagination)
                int totalRecords = await usersQuery.CountAsync();
                int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                // Fetch only the records for the current page
                var users = await usersQuery
                    .OrderBy(u => u.FirstName) // Adjust sorting if needed
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Get chairman IDs
                var chairmanIds = users.Select(u => u.Id).ToList();

                // Fetch chairman images
                var chairmanImages = await _context.ChairmanImages
                    .Where(ci => chairmanIds.Contains(ci.ChairmanAccountId))
                    .ToListAsync();

                // Map data to view model
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
                        Status = u.status
                    };
                }).ToList();

                // Set pagination data for the Razor page
                ViewData["TotalPages"] = totalPages;
                ViewData["CurrentPage"] = pageNumber;
                ViewData["Status"] = status;

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the chairman verification list.");
                TempData["ErrorMessage"] = "An error occurred while loading chairman data.";
                return RedirectToPage("/Error");
            }
        }


        public async Task<IActionResult> OnPostUpdateStatusAsync(string userId, bool? status)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Invalid user ID provided.");
                ModelState.AddModelError(string.Empty, "Invalid user ID.");
                return Page();
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"No user found with ID {userId}");
                    return NotFound();
                }

                // Assign the received status value
                user.status = status;
                string statusText = status == true ? "Approved" : status == false ? "Rejected" : "Pending";
                _logger.LogInformation($"Setting status to {statusText} for user {userId}");

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Successfully updated verification status for user {userId}");
                    TempData["SuccessMessage"] = $"Status updated to {statusText}";
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
                TempData["ErrorMessage"] = "An error occurred while updating the status.";
            }

            return RedirectToPage();
        }
    }
    }

