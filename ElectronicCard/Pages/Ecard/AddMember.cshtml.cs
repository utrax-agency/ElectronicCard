
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using ElectronicCard.Models;
using ElectronicCard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicCard.Pages.AddMember
{

    [Authorize(Roles= "Secretary ")]
    public class AddMemberModel : PageModel
    {
        private readonly MyAppDbContext _context;
        public AddMemberModel(MyAppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public SelectList Groups { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "First Name is required")]
            [Display(Name = "First Name")]
            public string First_Name { get; set; }

            [Required(ErrorMessage = "Last Name is required")]
            [Display(Name = "Last Name")]
            public string Last_Name { get; set; }

            [Display(Name = "Card Number")]
            public string Card_no { get; set; }

            [Required(ErrorMessage = "Age is required")]
            [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
            public int Age { get; set; }

            [Required(ErrorMessage = "Gender is required")]
            public string Gender { get; set; }

            [Required(ErrorMessage = "Phone Number is required")]
            [Phone(ErrorMessage = "Invalid Phone Number")]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Group is required")]
            [Display(Name = "Group")]
            public int Group_id { get; set; }
            [Required(ErrorMessage = "NIN is required")]
            [Display(Name = "NIN")]
            [StringLength(14, ErrorMessage = "NIN must not exceed 14 characters.")]
            [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{1,14}$", ErrorMessage = "NIN must contain both letters and numbers, and must not exceed 14 characters.")]
            public string NIN { get; set; }

        }

        private string GenerateCardNumber(string lastName)
        {
            // Get last two letters of last name, handle cases where last name might be too short
            string lastTwoLetters = lastName.Length >= 2
                ? lastName.Substring(lastName.Length - 2).ToUpper()
                : lastName.PadRight(2, 'X').ToUpper();

            // Generate 6 random digits
            Random random = new Random();
            string randomDigits = random.Next(0, 1000000).ToString("D6");

            // Combine to create 8-character card number (6 digits + 2 letters)
            return randomDigits + lastTwoLetters;
        }

        public void OnGet()
        {
            // Load groups for dropdown
            Groups = new SelectList(_context.Groups, "Id", "Group_Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove Card_no from ModelState validation since we're generating it
            ModelState.Remove("Input.Card_no");

            if (!ModelState.IsValid)
            {
                Groups = new SelectList(_context.Groups, "Id", "Group_Name");
                return Page();
            }

            // Generate card number using last name
            string cardNumber = GenerateCardNumber(Input.Last_Name);

            var member = new Member
            {
                First_Name = Input.First_Name,
                Last_Name = Input.Last_Name,
                Card_no = cardNumber,
                Age = Input.Age,
                Gender = Input.Gender,
                PhoneNumber = Input.PhoneNumber,
                Email = Input.Email,
                NIN = Input.NIN,
                Group_id = Input.Group_id
            };

            try
            {
                // Check if card number already exists
                while (await _context.Members.AnyAsync(m => m.Card_no == member.Card_no))
                {
                    // If exists, generate a new one
                    cardNumber = GenerateCardNumber(Input.Last_Name);
                    member.Card_no = cardNumber;
                }

                _context.Members.Add(member);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Member added successfully!";
                return RedirectToPage("/Ecard/ViewGroups");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error adding member. Please try again.");
                Groups = new SelectList(_context.Groups, "Id", "Group_Name");
                return Page();
            }
        }
    }
}