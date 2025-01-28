using ElectronicCard.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

public class Group
{
    public int Id { get; set; }
    public string Group_Name { get; set; }

    public string Treasurer_FirstName { get; set; }

    public string Treasurer_LastName { get; set; }


    public string Treasurer_Email { get; set; }
    public string Treasurer_PhoneNumber { get; set; }

    public string Chairman_FirstName { get; set; }
    public string Chairman_LastName { get; set; }
    public string Chairman_Email { get; set; }
    public string Chairman_PhoneNumber { get; set; }

    public string Secretary_FirstName { get; set; }
    public string Secretary_LastName { get; set; }
    public string Secretary_Email { get; set; }
    public string Secretary_PhoneNumber { get; set; }

    // Foreign Key for ProvinceUser
    [ForeignKey("ProvinceUser")]
    public string? Province_User_id { get; set; }  // Unique foreign key for ProvinceUser
    public ProvinceUser ProvinceUser { get; set; }  // Navigation property for ProvinceUser

    // Foreign Key for Province
    [ForeignKey("Province")]
    public int Province_ID { get; set; }  // Unique foreign key for Province
    public Province Province { get; set; }  // Navigation property for Province

    public string?  Role { get; set; }//roles for memebers eg chariman , secretary, treasurer

    public ICollection<UserGroup> GroupUsers { get; set; }//mapping to users of the group

    [ValidateNever]
    public ICollection<Member> Members { get; set; }  // Navigation property for Members



    
}
