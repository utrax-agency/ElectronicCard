using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElectronicCard.Models;
using ElectronicCard.Data;

namespace ElectronicCard.Pages.Ecard.ProvinceUser.Report
{
    public class GroupPerformanceModel : PageModel
    {
        private readonly MyAppDbContext _context;

        public GroupPerformanceModel(MyAppDbContext context)
        {
            _context = context;
        }

        public Group Group { get; set; }
        public List<Savings> GroupSavings { get; set; } = new List<Savings>();
        public int TotalMembers { get; set; }
        public decimal TotalSavings { get; set; }
        public decimal AverageSavingsPerMember { get; set; }
        public Dictionary<string, int> MonthlySavings { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> SavingsDistribution { get; set; } = new Dictionary<string, int>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get group with its members
            Group = await _context.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (Group == null)
            {
                return NotFound();
            }

            // Get all savings for members of this group
            var memberIds = Group.Members.Select(m => m.Id).ToList();
            GroupSavings = await _context.Savings
                .Where(s => memberIds.Contains(s.Member_id))
                .ToListAsync();

            // Calculate basic statistics
            TotalMembers = Group.Members.Count;
            TotalSavings = GroupSavings.Sum(s => s.Amount_saved);
            AverageSavingsPerMember = TotalMembers > 0 ? TotalSavings / TotalMembers : 0;

            // Calculate monthly savings
            MonthlySavings = GroupSavings
                .GroupBy(s => s.CreatedAt.ToString("MMM yyyy"))
                .OrderBy(g => DateTime.ParseExact(g.Key, "MMM yyyy", null))
                .ToDictionary(
                    g => g.Key,
                    g => (int)g.Sum(s => s.Amount_saved)
                );

            // Calculate savings distribution
            var memberSavingTotals = GroupSavings
                .GroupBy(s => s.Member_id)
                .Select(g => g.Sum(s => s.Amount_saved))
                .ToList();

            if (memberSavingTotals.Any())
            {
                var maxSavings = memberSavingTotals.Max();
                var highThreshold = maxSavings * 0.66m;
                var mediumThreshold = maxSavings * 0.33m;

                var highSavers = memberSavingTotals.Count(s => s >= highThreshold);
                var mediumSavers = memberSavingTotals.Count(s => s >= mediumThreshold && s < highThreshold);
                var lowSavers = memberSavingTotals.Count(s => s < mediumThreshold);

                SavingsDistribution = new Dictionary<string, int>
                {
                    { "High Savers", highSavers },
                    { "Medium Savers", mediumSavers },
                    { "Low Savers", lowSavers }
                };
            }
            else
            {
                // Initialize with zeros if no savings data
                SavingsDistribution = new Dictionary<string, int>
                {
                    { "High Savers", 0 },
                    { "Medium Savers", 0 },
                    { "Low Savers", 0 }
                };
            }

            return Page();
        }
    }
}
