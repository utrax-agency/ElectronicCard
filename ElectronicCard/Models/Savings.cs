namespace ElectronicCard.Models
{
    public class Savings
    {
        public int Id { get; set; }

        public int Amount_saved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int Member_id {get; set; }   
    }
}
