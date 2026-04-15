using System.ComponentModel.DataAnnotations;
using divelog.Models;

namespace divelog.ViewModels
{
    public class PersonCreateViewModel
    {

        [MaxLength(100)]
        public string? Name { get; set; }
        
        [MaxLength(10)]
        public string? Signature { get; set; }

        [Required]
        public int GroupId { get; set; }

        public DateTime? StartedDiving { get; set; }

        //De roller som användaren bockar i
        public List<int> SelectedRoleIds { get; set; } = new();

        //Alla tillgängliga roller (för checkboxar)
        public List<DiveRole> AvailableRoles { get; set; } = new();
    }
}