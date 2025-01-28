namespace ElectronicCard.Models
{
    public class ChairmanImages
    {
        public int Id { get; set; } // Primary Key for ChairmanId

        // Foreign Key to Chairman Account
        public string ChairmanAccountId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } // Navigation property

        // File paths for images
        public string NationalIdFrontPath { get; set; }
        public string NationalIdBackPath { get; set; }
        public string SelfiePath { get; set; }
    }
}
