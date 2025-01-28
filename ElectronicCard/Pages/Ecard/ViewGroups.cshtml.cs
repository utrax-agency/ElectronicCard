using Microsoft.AspNetCore.Mvc.RazorPages;
using ElectronicCard.Models;
using ElectronicCard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace ElectronicCard.Pages.Ecard
{
    [Authorize(Roles = "Chairman,Treasurer,Secretary")]//authorised users 

    public class GroupListModel : PageModel
    {
        private readonly MyAppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<GroupListModel> _logger;

        // Constructor to inject dependencies
        public GroupListModel(MyAppDbContext context, UserManager<ApplicationUser> userManager, ILogger<GroupListModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // List to store groups
        public List<Group> Groups { get; set; }

        // Properties for stats
        public int TotalGroups { get; set; }
        public int TotalMembersInAllGroups { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Get the ID of the currently logged-in user
                var userId = _userManager.GetUserId(User);

                if (!string.IsNullOrEmpty(userId))
                {
                    // Fetch groups associated with the logged-in user
                    Groups = await _context.UserGroups
                        .Where(ug => ug.UserId == userId)
                        .Include(ug => ug.Group.GroupUsers) // Include related GroupUsers
                            .ThenInclude(gu => gu.User)    // Include related User
                        .Select(ug => ug.Group)            // Select the Group after Include
                        .ToListAsync();

                    // Calculate total groups
                    TotalGroups = Groups.Count;

                    // Calculate total members across all groups
                    TotalMembersInAllGroups = Groups.Sum(g => g.GroupUsers?.Count ?? 0);
                }
                else
                {
                    // If no user is logged in, set empty values
                    Groups = new List<Group>();
                    TotalGroups = 0;
                    TotalMembersInAllGroups = 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception and handle the error gracefully
                _logger.LogError(ex, "Error occurred while fetching group list for the user.");
                TempData["ErrorMessage"] = "An error occurred while loading the group list. Please try again later.";

                // Set default values in case of an error
                Groups = new List<Group>();
                TotalGroups = 0;
                TotalMembersInAllGroups = 0;
            }
        }
    }
}

