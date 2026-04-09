using System.ComponentModel.DataAnnotations;

namespace divelog.Models;

public class Group
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    // Personer som tillhör gruppen
    public ICollection<Person> Persons { get; set; } = new List<Person>();

}