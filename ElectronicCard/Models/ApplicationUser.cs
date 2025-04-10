using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ElectronicCard.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int ConnectID { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string PhoneNumber {  get; set; }    

        public bool? status {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Role { get; set; }
        [ValidateNever]
        public ICollection<Group> Groups { get; set; }
        public string CreatedByUserId { get; set; }



        public ICollection<UserGroup> UserGroups { get; set; }
        public ICollection<ChairmanImages> ChairmanImages { get; set; }
    }
}

