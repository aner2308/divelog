using System.ComponentModel.DataAnnotations;

namespace divelog.Models;

public class DiveRole
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    //Koppling till personer
    public ICollection<PersonRole> PersonRoles { get; set; } = [];

}