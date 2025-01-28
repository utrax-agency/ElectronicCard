using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicCard.Models
{
    public class Attendance
    {
        public int id { get; set; }

        public DateTime MeetingDay { get; set; }//day of which group is meeting

        public bool ConfirmAttendance { get; set; }//did memeber attend or not

        [ForeignKey("Member")]

        public int MemberId { get; set; }

        public Member Member { get; set; }  

    }
}
