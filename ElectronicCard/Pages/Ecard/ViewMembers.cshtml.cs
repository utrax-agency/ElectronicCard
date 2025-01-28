//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.EntityFrameworkCore;
//using ElectronicCard.Models;
//using ElectronicCard.Data;
//using Microsoft.AspNetCore.Authorization;
//using ElectronicCard.Pages.Ecard.Login;
//using Microsoft.Extensions.Logging;


//namespace ElectronicCard.Pages.Ecard.ViewMembers
//{
//    [Authorize(Roles = "Chairman,Treasurer,Secretary")]

//    public class GroupDetailsModel : PageModel
//    {
//        private readonly MyAppDbContext _context;
//        private readonly ILogger<GroupDetailsModel> _logger;
//        public List<Attendance> AttendanceHistory { get; set; }

//        public GroupDetailsModel(MyAppDbContext context, ILogger<GroupDetailsModel> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        public Group Group { get; set; }
//        public IList<Member> Members { get; set; }
//        public int TotalGroupSavings { get; private set; }
//        private IList<Savings> AllGroupSavings { get; set; }

//        [BindProperty(SupportsGet = true)]
//        public int MemberId { get; set; }

//        public async Task<IActionResult> OnGetAsync(int id)
//        {
//            try
//            {
//                // Fetch the group
//                Group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id);

//                if (Group == null)
//                {
//                    return NotFound();
//                }

//                // Fetch members belonging to this group
//                Members = await _context.Members
//                    .Where(m => m.Group_id == id)
//                    .ToListAsync();

//                // Fetch all savings for members in this group
//                AllGroupSavings = await _context.Savings
//                    .Where(s => Members.Select(m => m.Id).Contains(s.Member_id))
//                    .OrderByDescending(s => s.CreatedAt)
//                    .ToListAsync();

//                // Calculate total group savings
//                TotalGroupSavings = AllGroupSavings.Sum(s => s.Amount_saved);

//                return Page();
//            }
//            catch (Exception ex)
//            {
//                // Log the exception and show an error page or message
//                _logger.LogError(ex, "Error occurred while fetching group details for ID {Id}", id);
//                TempData["ErrorMessage"] = "An error occurred while loading group details. Please try again later.";
//                return RedirectToPage("/Error");
//            }
//        }

//        // Modified OnPost method

//        // Replace the existing OnGetAsync methods with this single one
//        public async Task<IActionResult> OnGetAsync(int id, int? memberId = null)
//        {
//            try
//            {
//                // Existing group details logic
//                Group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id);

//                if (Group == null)
//                {
//                    return NotFound();
//                }

//                // Fetch members belonging to this group
//                Members = await _context.Members
//                    .Where(m => m.Group_id == id)
//                    .ToListAsync();

//                // Fetch all savings for members in this group
//                AllGroupSavings = await _context.Savings
//                    .Where(s => Members.Select(m => m.Id).Contains(s.Member_id))
//                    .OrderByDescending(s => s.CreatedAt)
//                    .ToListAsync();

//                // Calculate total group savings
//                TotalGroupSavings = AllGroupSavings.Sum(s => s.Amount_saved);

//                // Handle attendance history if memberId is provided
//                if (memberId.HasValue && memberId > 0)
//                {
//                    _logger.LogInformation("Retrieving attendance history for MemberId: {MemberId}", memberId);
//                    AttendanceHistory = await GetMemberAttendanceHistoryAsync(memberId.Value);
//                    _logger.LogInformation("Retrieved {Count} attendance records for MemberId: {MemberId}",
//                        AttendanceHistory?.Count ?? 0, memberId);
//                }

//                return Page();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while fetching group details for ID {Id}", id);
//                TempData["ErrorMessage"] = "An error occurred while loading details. Please try again later.";
//                return RedirectToPage("/Error");
//            }
//        }
//        public async Task<IActionResult> OnPostMemberAttendanceAsync(int id, int memberId, DateTime meetingDay, bool isPresent)
//        {
//            try
//            {
//                var existingAttendance = await _context.Attendances
//                    .FirstOrDefaultAsync(a => a.MemberId == memberId && a.MeetingDay.Date == meetingDay.Date);

//                if (existingAttendance != null)
//                {
//                    existingAttendance.ConfirmAttendance = isPresent;
//                    _context.Attendances.Update(existingAttendance);
//                }
//                else
//                {
//                    var attendance = new Attendance
//                    {
//                        MemberId = memberId,
//                        MeetingDay = meetingDay,
//                        ConfirmAttendance = isPresent
//                    };
//                    _context.Attendances.Add(attendance);
//                }

//                await _context.SaveChangesAsync();
//                TempData["SuccessMessage"] = $"Attendance recorded for member {memberId}.";

//                return RedirectToPage(new { id });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error recording attendance for Member ID {MemberId}", memberId);
//                TempData["ErrorMessage"] = "Failed to record attendance.";
//                return RedirectToPage(new { id });
//            }
//        }
//        public async Task<IActionResult> OnPostAddSavingsAsync(int id, int memberId, decimal amount)
//        {
//            if (!ModelState.IsValid || amount <= 0)
//            {
//                TempData["ErrorMessage"] = "Please enter a valid amount.";
//                return RedirectToPage(new { id });
//            }

//            try
//            {
//                var saving = new Savings
//                {
//                    Member_id = memberId,
//                    Amount_saved = (int)amount,
//                    UpdatedAt = DateTime.Now,
//                    CreatedAt = DateTime.Now
//                };

//                _context.Savings.Add(saving);
//                await _context.SaveChangesAsync();

//                TempData["SuccessMessage"] = "Savings successfully recorded.";
//                return RedirectToPage(new { id });
//            }
//            catch (Exception ex)
//            {
//                // Log the exception and show an error message
//                _logger.LogError(ex, "Error occurred while adding savings for Group ID {GroupId} and Member ID {MemberId}", id, memberId);
//                TempData["ErrorMessage"] = "An error occurred while recording savings. Please try again later.";
//                return RedirectToPage(new { id });
//            }
//        }

//        public IEnumerable<Savings> GetMemberSavingsHistory(int memberId)
//        {
//            try
//            {
//                return _context.Savings
//                    .Where(s => s.Member_id == memberId)
//                    .OrderByDescending(s => s.CreatedAt);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while fetching savings history for Member ID {MemberId}", memberId);
//                return Enumerable.Empty<Savings>();
//            }
//        }

//        public decimal GetMemberSavings(int memberId)
//        {
//            try
//            {
//                return AllGroupSavings
//                    .Where(s => s.Member_id == memberId)
//                    .Sum(s => s.Amount_saved);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred while calculating savings for Member ID {MemberId}", memberId);
//                return 0;
//            }
//        }
//    }
//}







using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElectronicCard.Models;
using ElectronicCard.Data;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicCard.Pages.Ecard.ViewMembers
{
    [Authorize(Roles = "Chairman,Treasurer,Secretary")]
    public class GroupDetailsModel : PageModel
    {
        private readonly MyAppDbContext _context;
        private readonly ILogger<GroupDetailsModel> _logger;

        public GroupDetailsModel(MyAppDbContext context, ILogger<GroupDetailsModel> logger)
        {
            _context = context;
            _logger = logger;
            AttendanceHistory = new List<Attendance>();
        }

        public Group Group { get; set; }
        public IList<Member> Members { get; set; }
        public List<Attendance> AttendanceHistory { get; set; }
        public int TotalGroupSavings { get; private set; }
        private IList<Savings> AllGroupSavings { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedMemberId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                _logger.LogInformation("OnGetAsync called with GroupId: {GroupId}, SelectedMemberId: {SelectedMemberId}",
                    id, SelectedMemberId);

                // Fetch the group and its members
                Group = await _context.Groups
                    .Include(g => g.Members)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (Group == null)
                {
                    _logger.LogWarning("Group not found with ID: {Id}", id);
                    return NotFound();
                }

                // Fetch all members in the group
                Members = await _context.Members
                    .Where(m => m.Group_id == id)
                    .Include(m => m.Attendances)
                    .ToListAsync();

                // If a member is selected, retrieve their attendance history
                if (SelectedMemberId.HasValue)
                {
                    _logger.LogInformation("Fetching attendance for SelectedMemberId: {SelectedMemberId}", SelectedMemberId.Value);

                    AttendanceHistory = await _context.Attendances
                        .Where(a => a.MemberId == SelectedMemberId.Value)
                        .OrderByDescending(a => a.MeetingDay)
                        .ToListAsync();

                    _logger.LogInformation("Found {Count} attendance records for member {MemberId}",
                        AttendanceHistory.Count, SelectedMemberId.Value);
                }

                // Fetch all savings data for members in the group
                var memberIds = Members.Select(m => m.Id).ToList();
                AllGroupSavings = await _context.Savings
                    .Where(s => memberIds.Contains(s.Member_id))
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                TotalGroupSavings = AllGroupSavings.Sum(s => s.Amount_saved);

                _logger.LogInformation(
                    "Successfully loaded group {Id} with {MemberCount} members, {SavingsCount} savings records, {AttendanceCount} attendance records",
                    id, Members.Count, AllGroupSavings.Count, AttendanceHistory?.Count ?? 0);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading group {Id}. Error: {Message}", id, ex.Message);
                TempData["ErrorMessage"] = "An error occurred while loading the group details. Please try again later.";
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostMemberAttendanceAsync(int id, int memberId, DateTime meetingDay, bool isPresent)
        {
            try
            {
                _logger.LogInformation(
                    "Recording attendance for Member {MemberId} on {MeetingDay}, Present: {IsPresent}",
                    memberId, meetingDay, isPresent);

                var existingAttendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.MemberId == memberId && a.MeetingDay.Date == meetingDay.Date);

                if (existingAttendance != null)
                {
                    _logger.LogInformation("Updating existing attendance record {Id}", existingAttendance.id);
                    existingAttendance.ConfirmAttendance = isPresent;
                    _context.Attendances.Update(existingAttendance);
                }
                else
                {
                    _logger.LogInformation("Creating new attendance record");
                    var attendance = new Attendance
                    {
                        MemberId = memberId,
                        MeetingDay = meetingDay,
                        ConfirmAttendance = isPresent
                    };
                    _context.Attendances.Add(attendance);
                }

                await _context.SaveChangesAsync();

                // Verify the save was successful
                var savedAttendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.MemberId == memberId && a.MeetingDay.Date == meetingDay.Date);

                if (savedAttendance != null)
                {
                    _logger.LogInformation("Successfully saved attendance record {Id}", savedAttendance.id);
                    TempData["SuccessMessage"] = "Attendance recorded successfully.";
                }

                return RedirectToPage(new { id, SelectedMemberId = memberId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording attendance for Member ID {MemberId}", memberId);
                TempData["ErrorMessage"] = "Failed to record attendance.";
                return RedirectToPage(new { id, SelectedMemberId = memberId });
            }
        }

        public async Task<IActionResult> OnPostAddSavingsAsync(int id, int memberId, decimal amount)
        {
            if (!ModelState.IsValid || amount <= 0)
            {
                TempData["ErrorMessage"] = "Please enter a valid amount.";
                return RedirectToPage(new { id, SelectedMemberId = memberId });
            }

            try
            {
                var saving = new Savings
                {
                    Member_id = memberId,
                    Amount_saved = (int)amount,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                _context.Savings.Add(saving);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Savings successfully recorded.";
                return RedirectToPage(new { id, SelectedMemberId = memberId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding savings for Group ID {GroupId} and Member ID {MemberId}", id, memberId);
                TempData["ErrorMessage"] = "An error occurred while recording savings. Please try again later.";
                return RedirectToPage(new { id, SelectedMemberId = memberId });
            }
        }
        public IEnumerable<Savings> GetMemberSavingsHistory(int memberId)
        {
            try
            {
                return _context.Savings
                    .Where(s => s.Member_id == memberId)
                    .OrderByDescending(s => s.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching savings history for Member ID {MemberId}", memberId);
                return Enumerable.Empty<Savings>();
            }
        }
      
        public decimal GetMemberSavings(int memberId)
        {
            return AllGroupSavings?
                .Where(s => s.Member_id == memberId)
                .Sum(s => s.Amount_saved) ?? 0;
        }
    }
}