// ProvinceGroups.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElectronicCard.Models;
using ElectronicCard.Data;

namespace ElectronicCard.Pages.Ecard.ProvinceUser.ProvinceGroups
{
    public class ProvinceGroupsModel : PageModel
    {
        private readonly MyAppDbContext _context;

        public ProvinceGroupsModel(MyAppDbContext context)
        {
            _context = context;  // Fixed: Assign to field, not parameter
        }

        public Province Province { get; set; }
        public List<Group> Groups { get; set; } = new List<Group>();

        public async Task<IActionResult> OnGetAsync()
        {
            const int PROVINCE_ID = 2; // Hardcoded Province ID

            Province = await _context.Provinces
                .Include(p => p.Groups)
                .FirstOrDefaultAsync(p => p.Id == PROVINCE_ID);

            if (Province == null)
            {
                return NotFound($"Province with ID {PROVINCE_ID} was not found.");
            }

            Groups = await _context.Groups
                .Where(g => g.Province_ID == PROVINCE_ID)
                .ToListAsync();

            return Page();
        }
    }
}