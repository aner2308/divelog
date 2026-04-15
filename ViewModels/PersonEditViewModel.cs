using divelog.Models;
using System.ComponentModel.DataAnnotations;

namespace divelog.ViewModels
{
    public class PersonEditViewModel
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(10)]
        public string? Signature { get; set; }

        [Required]
        public int GroupId { get; set; }

        public DateTime? StartedDiving { get; set; }

        // Visar alla tillgängliga roller
        public List<DiveRole> AvailableRoles { get; set; } = new List<DiveRole>();

        // Roller som är valda för personen
        public List<int> SelectedRoleIds { get; set; } = new List<int>();
    }
}