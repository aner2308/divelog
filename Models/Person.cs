using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace divelog.Models;

public class Person
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(10)]
    public string? Signature { get; set; }

    [Required]
    public int GroupId { get; set; }

    [ValidateNever]
    [ForeignKey(nameof(GroupId))]
    public Group Group { get; set; } = null!;

    public DateTime? StartedDiving { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Roller personen har (dykare, dykledare, dykskötare)
    public ICollection<PersonRole> PersonRoles { get; set; } = new List<PersonRole>();

    //Dyk där person är dykledare
    public ICollection<Dive> LedDives { get; set; } = new List<Dive>();

    //Markera bottagen person, men är kvar i databasen
    public bool IsDeleted { get; set; } = false;
}