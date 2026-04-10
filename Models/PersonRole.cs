using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace divelog.Models;

public class PersonRole
{
    public int Id { get; set; }

    [Required]
    public int PersonId { get; set; }

    //Kopplar till person i class Person
    [ForeignKey(nameof(PersonId))]
    public Person Person { get; set; } = null!;

    [Required]
    public int DiveRoleId { get; set; }

    //Kopplar till roll i class Role
    [ForeignKey(nameof(DiveRoleId))]
    public DiveRole DiveRole { get; set; } = null!;
}