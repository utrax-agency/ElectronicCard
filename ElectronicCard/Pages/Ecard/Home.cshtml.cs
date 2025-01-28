using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using ElectronicCard.Models;
using ElectronicCard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicCard.Pages.Ecard
{
    [Authorize(Roles = "Chairman,Treasurer,Secretary")]//authorised users to create group

    public class ChairmanSignupModel : PageModel
    {
        private readonly MyAppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ChairmanSignupModel> _logger;

        public ChairmanSignupModel(MyAppDbContext context, UserManager<ApplicationUser> userManager, ILogger<ChairmanSignupModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public GroupInput Input { get; set; }

        public SelectList Provinces { get; set; }

        public async Task OnGetAsync()
        {
            _logger.LogInformation("Starting OnGetAsync method for ChairmanSignup page");

            try
            {
                // Load provinces for dropdown
                var provinces = await _context.ProvinceUsers.ToListAsync();
                _logger.LogInformation("Successfully loaded {Count} provinces", provinces.Count);
                Provinces = new SelectList(provinces, "Id", "Name");

                // Capture logged-in user details
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    string userRole = roles.FirstOrDefault();

                    _logger.LogInformation("Logged-in user details - ID: {UserId}, Name: {UserName}, Email: {Email}, Role: {UserRole}",
                        user.Id, user.UserName, user.Email, userRole);

                    // Auto-fill fields based on role
                    Input = new GroupInput
                    {
                        Chairman_FirstName = userRole == "Chairman" ? user.FirstName : null,
                        Chairman_LastName = userRole == "Chairman" ? user.LastName : null,
                        Chairman_Email = userRole == "Chairman" ? user.Email : null,
                        Chairman_PhoneNumber = userRole == "Chairman" ? user.PhoneNumber : null,
                        Treasurer_FirstName = userRole == "Treasurer" ? user.FirstName : null,
                        Treasurer_LastName = userRole == "Treasurer" ? user.LastName : null,
                        Treasurer_Email = userRole == "Treasurer" ? user.Email : null,
                        Treasurer_PhoneNumber = userRole == "Treasurer" ? user.PhoneNumber : null,
                        Secretary_FirstName = userRole == "Secretary" ? user.FirstName : null,
                        Secretary_LastName = userRole == "Secretary" ? user.LastName : null,
                        Secretary_Email = userRole == "Secretary" ? user.Email : null,
                        Secretary_PhoneNumber = userRole == "Secretary" ? user.PhoneNumber : null
                    };

                    _logger.LogInformation("Successfully auto-filled form fields based on user role: {Role}", userRole);
                }
                else
                {
                    _logger.LogWarning("No logged-in user detected during form initialization");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during OnGetAsync execution");
                throw;
            }
        }

        public class GroupInput
        {
            [Required(ErrorMessage = "Group Name is required.")]
            [Display(Name = "Group Name")]
            public string Group_Name { get; set; }

            [Required(ErrorMessage = "Chairman First Name is required.")]
            [Display(Name = "Chairman First Name")]
            public string Chairman_FirstName { get; set; }

            [Required(ErrorMessage = "Chairman Last Name is required.")]
            [Display(Name = "Chairman Last Name")]
            public string Chairman_LastName { get; set; }

            [Required(ErrorMessage = "Chairman Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            [Display(Name = "Chairman Email")]
            public string Chairman_Email { get; set; }

            [Required(ErrorMessage = "Chairman Phone Number is required.")]
            [Phone(ErrorMessage = "Invalid phone number.")]
            [Display(Name = "Chairman Phone Number")]
            public string Chairman_PhoneNumber { get; set; }

            [Required(ErrorMessage = "Treasurer First Name is required.")]
            [Display(Name = "Treasurer First Name")]
            public string Treasurer_FirstName { get; set; }

            [Required(ErrorMessage = "Treasurer Last Name is required.")]
            [Display(Name = "Treasurer Last Name")]
            public string Treasurer_LastName { get; set; }

            [Required(ErrorMessage = "Treasurer Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            [Display(Name = "Treasurer Email")]
            public string Treasurer_Email { get; set; }

            [Required(ErrorMessage = "Treasurer Phone Number is required.")]
            [Phone(ErrorMessage = "Invalid phone number.")]
            [Display(Name = "Treasurer Phone Number")]
            public string Treasurer_PhoneNumber { get; set; }

            [Required(ErrorMessage = "Secretary First Name is required.")]
            [Display(Name = "Secretary First Name")]
            public string Secretary_FirstName { get; set; }

            [Required(ErrorMessage = "Secretary Last Name is required.")]
            [Display(Name = "Secretary Last Name")]
            public string Secretary_LastName { get; set; }

            [Required(ErrorMessage = "Secretary Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            [Display(Name = "Secretary Email")]
            public string Secretary_Email { get; set; }

            [Required(ErrorMessage = "Secretary Phone Number is required.")]
            [Phone(ErrorMessage = "Invalid phone number.")]
            [Display(Name = "Secretary Phone Number")]
            public string Secretary_PhoneNumber { get; set; }

            [Required(ErrorMessage = "Province is required.")]
            [Display(Name = "Province")]
            public int Province_id { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Starting OnPostAsync method for group creation");

            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    _logger.LogWarning("Model validation failed with errors: {Errors}", string.Join(", ", errors));

                    Provinces = new SelectList(await _context.ProvinceUsers.ToListAsync(), "Id", "Name");
                    return Page();
                }

                // Get logged-in user
                var loggedInUser = await _userManager.GetUserAsync(User);
                if (loggedInUser == null)
                {
                    _logger.LogWarning("No logged-in user detected during group creation");
                    return RedirectToPage("/Account/Login");
                }

                _logger.LogInformation("Creating group with name: {GroupName}", Input.Group_Name);

                // Create the group entity
                var group = new Group
                {
                    Group_Name = Input.Group_Name,
                    Chairman_FirstName = Input.Chairman_FirstName,
                    Chairman_LastName = Input.Chairman_LastName,
                    Chairman_Email = Input.Chairman_Email,
                    Chairman_PhoneNumber = Input.Chairman_PhoneNumber,
                    Secretary_FirstName = Input.Secretary_FirstName,
                    Secretary_LastName = Input.Secretary_LastName,
                    Secretary_Email = Input.Secretary_Email,
                    Secretary_PhoneNumber = Input.Secretary_PhoneNumber,
                    Treasurer_FirstName = Input.Treasurer_FirstName,
                    Treasurer_LastName = Input.Treasurer_LastName,
                    Treasurer_Email = Input.Treasurer_Email,
                    Treasurer_PhoneNumber = Input.Treasurer_PhoneNumber,
                    Province_ID = Input.Province_id,
                    Province_User_id = loggedInUser.Id
                };

                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                // Create users and assign roles
                var chairmanUser = await CreateUserAndAssignRoleAsync(
                    Input.Chairman_Email, Input.Chairman_FirstName, Input.Chairman_LastName,
                    Input.Chairman_PhoneNumber, "Chairman", loggedInUser);

                var secretaryUser = await CreateUserAndAssignRoleAsync(
                    Input.Secretary_Email, Input.Secretary_FirstName, Input.Secretary_LastName,
                    Input.Secretary_PhoneNumber, "Secretary", loggedInUser);

                var treasurerUser = await CreateUserAndAssignRoleAsync(
                    Input.Treasurer_Email, Input.Treasurer_FirstName, Input.Treasurer_LastName,
                    Input.Treasurer_PhoneNumber, "Treasurer", loggedInUser);

                // Collect unique user IDs
                var userIds = new HashSet<string>();
                if (chairmanUser != null) userIds.Add(chairmanUser.Id);
                if (secretaryUser != null) userIds.Add(secretaryUser.Id);
                if (treasurerUser != null) userIds.Add(treasurerUser.Id);
                userIds.Add(loggedInUser.Id);

                // Create UserGroup entries
                foreach (var userId in userIds)
                {
                    var exists = await _context.UserGroups
                        .AnyAsync(ug => ug.UserId == userId && ug.GroupId == group.Id);

                    if (!exists)
                    {
                        _context.UserGroups.Add(new UserGroup
                        {
                            UserId = userId,
                            GroupId = group.Id
                        });
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created group: {GroupName} with ID: {GroupId}", group.Group_Name, group.Id);

                return RedirectToPage("/Ecard/ViewGroups");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during group creation process");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the group. Please try again.");
                Provinces = new SelectList(await _context.ProvinceUsers.ToListAsync(), "Id", "Name");
                return Page();
            }
        }

        private async Task<ApplicationUser> CreateUserAndAssignRoleAsync(
        string email,
        string firstName,
        string lastName,
        string phoneNumber,
        string role,
        ApplicationUser loggedInUser)
        {
            try
            {
                // Check if the user already exists
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    _logger.LogInformation($"User with email {email} already exists.");

                    // Check if the user already has the desired role
                    var roles = await _userManager.GetRolesAsync(existingUser);
                    if (!roles.Contains(role))
                    {
                        // Assign the new role
                        var roleResult = await _userManager.AddToRoleAsync(existingUser, role);
                        if (!roleResult.Succeeded)
                        {
                            var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                            _logger.LogError($"Failed to assign role {role} to existing user {email}. Errors: {roleErrors}");
                        }
                        else
                        {
                            _logger.LogInformation($"Assigned new role {role} to existing user {email}.");
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"User {email} already has the role {role}.");
                    }

                    return existingUser;
                }

                // Create a new user
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    FirstName = firstName,
                    LastName = lastName,
                    Role = role,
                    status = true,
                    CreatedByUserId = loggedInUser.Id, // Track creator
                    CreatedAt = DateTime.UtcNow       // Timestamp
                };

                // Set a default password (replace with secure handling)
                string defaultPassword = "DefaultPassword123!";
                var createResult = await _userManager.CreateAsync(user, defaultPassword);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Failed to create user {email}. Errors: {errors}");
                    return null;
                }

                _logger.LogInformation($"User {email} successfully created.");

                // Assign role to the user
                var assignRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (!assignRoleResult.Succeeded)
                {
                    var roleErrors = string.Join(", ", assignRoleResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Failed to assign role {role} to user {email}. Errors: {roleErrors}");
                    return null;
                }

                _logger.LogInformation($"User {email} assigned to role {role}.");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while creating user {email} and assigning role {role}.");
                return null;
            }
        }

    }
}
