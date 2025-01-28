using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ElectronicCard.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronicCard.Data;
using Microsoft.AspNetCore.Authorization;



namespace ElectronicCard.Pages.Ecard.Admin
{
    [Authorize(Roles = "Admin")]
    public class AllProvincesModel : PageModel
    {
        private readonly MyAppDbContext _context;
        private readonly ILogger<AllProvincesModel> _logger;

        public AllProvincesModel(MyAppDbContext context, ILogger<AllProvincesModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Province> Provinces { get; set; } = new();
        public decimal TotalNationalSavings { get; set; }
        public object ProvinceSavingsPieData { get; set; }
        public Dictionary<string, Dictionary<string, decimal>> MonthlyProvinceSavings { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Fetching data for all provinces...");

                // Fetch all provinces with related entities
                Provinces = await _context.Provinces
                    .Include(p => p.Groups)
                        .ThenInclude(g => g.Members)
                            .ThenInclude(m => m.Savings)
                    .ToListAsync();

                if (!Provinces.Any())
                {
                    _logger.LogWarning("No provinces found.");
                    return NotFound();
                }

                _logger.LogInformation($"Found {Provinces.Count} provinces.");

                var provinceSavingsLabels = new List<string>();
                var provinceSavingsData = new List<decimal>();
                var monthlyProvinceSavings = new Dictionary<string, Dictionary<string, decimal>>();

                foreach (var province in Provinces)
                {
                    decimal provinceTotalSavings = 0;
                    var provinceMonthlyData = new Dictionary<string, decimal>();

                    foreach (var group in province.Groups)
                    {
                        // Calculate savings for each group
                        var groupSavings = group.Members?
                            .SelectMany(m => m.Savings ?? Enumerable.Empty<Savings>())
                            .ToList() ?? new List<Savings>();

                        provinceTotalSavings += groupSavings.Sum(s => s.Amount_saved);

                        // Aggregate monthly savings for the group
                        var groupMonthlyData = groupSavings
                            .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
                            .Select(g => new
                            {
                                MonthKey = $"{g.Key.Year}-{g.Key.Month:D2}",
                                TotalSavings = g.Sum(s => s.Amount_saved)
                            });

                        foreach (var monthData in groupMonthlyData)
                        {
                            if (!provinceMonthlyData.ContainsKey(monthData.MonthKey))
                                provinceMonthlyData[monthData.MonthKey] = 0;
                            provinceMonthlyData[monthData.MonthKey] += monthData.TotalSavings;
                        }
                    }

                    // Update total national savings
                    TotalNationalSavings += provinceTotalSavings;

                    // Prepare chart data
                    provinceSavingsLabels.Add(province.Province_Name);
                    provinceSavingsData.Add(provinceTotalSavings);

                    monthlyProvinceSavings[province.Province_Name] = provinceMonthlyData;
                }

                // Prepare pie chart data
                var totalSavings = provinceSavingsData.Sum();
                if (totalSavings > 0) // Avoid division by zero
                {
                    var pieChartData = provinceSavingsData.Select(s => (s / totalSavings) * 100).ToList();
                    ProvinceSavingsPieData = new
                    {
                        labels = provinceSavingsLabels,
                        data = pieChartData
                    };
                }
                else
                {
                    ProvinceSavingsPieData = new
                    {
                        labels = provinceSavingsLabels,
                        data = new List<decimal>() // No data available
                    };
                }

                // Store monthly savings data for the line graph
                MonthlyProvinceSavings = monthlyProvinceSavings;

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing data: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        public object GetProvinceMonthlyData(int provinceId)
        {
            var province = Provinces.FirstOrDefault(p => p.Id == provinceId);
            if (province == null || !MonthlyProvinceSavings.ContainsKey(province.Province_Name))
                return null;

            var monthlyData = MonthlyProvinceSavings[province.Province_Name];
            return new
            {
                labels = monthlyData.Keys.OrderBy(k => k).ToList(),
                data = monthlyData.OrderBy(k => k.Key).Select(v => v.Value).ToList()
            };
        }
    }
}

