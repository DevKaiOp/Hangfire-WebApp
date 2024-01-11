using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime Timestamp { get; set; }
        public bool InOutType { get; set; }
        public DateTime SyncedTime { get; set; }
    }
}
