using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElectronicCard.Models;
using ElectronicCard.Data;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicCard.Pages.Ecard.Admin.ProvinveOverlook
{
    [Authorize(Roles = "Admin")]
    public class ProvincePerformanceModel : PageModel
    {
        private readonly MyAppDbContext _context;
        private readonly ILogger<ProvincePerformanceModel> _logger;

        public ProvincePerformanceModel(MyAppDbContext context, ILogger<ProvincePerformanceModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Province> Provinces { get; set; }
        public Dictionary<int, decimal> ProvinceSavings { get; set; } = new Dictionary<int, decimal>();
        public Dictionary<int, ProvinceChartData> ProvinceCharts { get; set; } = new Dictionary<int, ProvinceChartData>();
        public decimal TotalSystemSavings { get; set; }

        public class ProvinceChartData
        {
            public string[] MonthlyLabels { get; set; }
            public int[] MonthlyValues { get; set; }
            public string[] GroupLabels { get; set; }
            public int[] GroupValues { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Fetching data for all provinces");

                // Fetch all Provinces with Groups and Members
                Provinces = await _context.Provinces
                    .Include(p => p.Groups)
                        .ThenInclude(g => g.Members)
                    .ToListAsync();

                if (!Provinces.Any())
                {
                    _logger.LogWarning("No provinces found in the system");
                    return Page();
                }

                // Get all member IDs across all provinces
                var allMemberIds = Provinces
                    .SelectMany(p => p.Groups)
                    .SelectMany(g => g.Members)
                    .Select(m => m.Id)
                    .ToList();

                // Preload all savings
                var allSavings = await _context.Savings
                    .Where(s => allMemberIds.Contains(s.Member_id))
                    .ToListAsync();

                foreach (var province in Provinces)
                {
                    var provinceMemberIds = province.Groups
                        .SelectMany(g => g.Members)
                        .Select(m => m.Id)
                        .ToList();

                    // Calculate total savings per province
                    var provinceTotalSavings = allSavings
                        .Where(s => provinceMemberIds.Contains(s.Member_id))
                        .Sum(s => s.Amount_saved);

                    ProvinceSavings[province.Id] = provinceTotalSavings;
                    TotalSystemSavings += provinceTotalSavings;

                    // Prepare monthly data
                    var monthlyData = allSavings
                        .Where(s => provinceMemberIds.Contains(s.Member_id))
                        .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
                        .Select(g => new
                        {
                            YearMonth = $"{g.Key.Year}-{g.Key.Month:D2}",
                            TotalSavings = g.Sum(s => s.Amount_saved)
                        })
                        .OrderBy(x => x.YearMonth)
                        .ToList();

                    // Prepare group data
                    var groupData = province.Groups
                        .Select(g => new
                        {
                            GroupName = g.Group_Name,
                            TotalSavings = allSavings
                                .Where(s => g.Members.Select(m => m.Id).Contains(s.Member_id))
                                .Sum(s => s.Amount_saved)
                        })
                        .ToList();

                    // Create chart data object
                    ProvinceCharts[province.Id] = new ProvinceChartData
                    {
                        MonthlyLabels = monthlyData.Select(m => m.YearMonth).ToArray(),
                        MonthlyValues = monthlyData.Select(m => m.TotalSavings).ToArray(),
                        GroupLabels = groupData.Select(g => g.GroupName).ToArray(),
                        GroupValues = groupData.Select(g => g.TotalSavings).ToArray()
                    };
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
