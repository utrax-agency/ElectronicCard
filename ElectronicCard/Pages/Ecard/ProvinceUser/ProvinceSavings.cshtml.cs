using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElectronicCard.Models;
using ElectronicCard.Data;

namespace ElectronicCard.Pages.Ecard.ProvinceUser
{
    public class ProvinceSavingsModel : PageModel
    {
        private readonly MyAppDbContext _context;
        private readonly ILogger<ProvinceSavingsModel> _logger;

        public ProvinceSavingsModel(MyAppDbContext context, ILogger<ProvinceSavingsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Province Province { get; set; }
        public Dictionary<string, decimal> GroupSavings { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, Dictionary<string, decimal>> MonthlyGroupSavings { get; set; } = new Dictionary<string, Dictionary<string, decimal>>();
        public decimal TotalProvinceSavings { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Fetching data for province ID: {id}");

                // Fetch Province with Groups and Members
                Province = await _context.Provinces
                    .Include(p => p.Groups)
                        .ThenInclude(g => g.Members)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (Province == null)
                {
                    _logger.LogWarning($"Province with ID {id} not found");
                    return NotFound();
                }

                _logger.LogInformation($"Found province: {Province.Province_Name} with {Province.Groups.Count} groups");

                // Preload all savings for members in the province
                var allMemberIds = Province.Groups
                    .SelectMany(g => g.Members)
                    .Select(m => m.Id)
                    .ToList();

                var allSavings = await _context.Savings
                    .Where(s => allMemberIds.Contains(s.Member_id))
                    .ToListAsync();

                _logger.LogInformation($"Total savings records found: {allSavings.Count}");

                // Calculate savings for each group
                foreach (var group in Province.Groups)
                {
                    var groupMemberIds = group.Members.Select(m => m.Id).ToList();
                    var groupTotalSavings = allSavings
                        .Where(s => groupMemberIds.Contains(s.Member_id))
                        .Sum(s => s.Amount_saved);

                    GroupSavings[group.Group_Name] = groupTotalSavings;
                    TotalProvinceSavings += groupTotalSavings;

                    _logger.LogInformation($"Group {group.Group_Name}: {groupTotalSavings:C2}");
                }

                // Prepare monthly savings data
                var monthlyData = allSavings
                    .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        GroupedSavings = g.ToList()
                    })
                    .ToList();

                // Now we create the dictionary with grouped data
                foreach (var data in monthlyData)
                {
                    var groupName = Province.Groups
                        .FirstOrDefault(g => g.Members.Any(m => m.Id == data.GroupedSavings.First().Member_id))?.Group_Name;

                    if (groupName != null)
                    {
                        if (!MonthlyGroupSavings.ContainsKey(groupName))
                        {
                            MonthlyGroupSavings[groupName] = new Dictionary<string, decimal>();
                        }

                        var monthKey = $"{data.Year}-{data.Month:D2}";

                        var totalSavingsForMonth = data.GroupedSavings.Sum(s => s.Amount_saved);

                        MonthlyGroupSavings[groupName][monthKey] = totalSavingsForMonth;

                        _logger.LogInformation($"Group: {groupName}, Month: {monthKey}, Total Savings: {totalSavingsForMonth:C2}");
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing data: {ex.Message}");
                return Page();
            }
        }
    }
}
