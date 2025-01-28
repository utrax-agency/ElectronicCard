using ElectronicCard.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Member
{
    public int Id { get; set; }
    public string First_Name { get; set; }
    public string Last_Name { get; set; }

    public string Card_no { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    public string NIN { get; set; }

    // Foreign Key for Group
    [ForeignKey("Group")]
    public int Group_id { get; set; }

    // Navigation property to Group
    public Group Group { get; set; }

    public ICollection<Savings> Savings { get; set; }

    public ICollection<Attendance> Attendances { get; set; }    //navigation to attendance
}
