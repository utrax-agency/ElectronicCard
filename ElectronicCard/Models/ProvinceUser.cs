using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ElectronicCard.Models
{
    public class ProvinceUser :ApplicationUser
    {
        public string ProvinceId { get; set; }  
        public string ProvinceName { get; set; }

        public string District { get; set; }    

        public string Village { get; set; }

        [ValidateNever]
        public ICollection<Group> Groups { get; set; }

    }
}
