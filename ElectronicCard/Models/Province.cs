using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ElectronicCard.Models
{
    public class Province
    {
        public int Id { get; set; }
        public string Province_Name { get; set; }

        // Navigation property to Groups
        [ValidateNever]
        public ICollection<Group> Groups { get; set; }
    }
}
