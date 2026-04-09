namespace divelog.Models;

public class DivePurpose
{
    public int Id { get; set; }
    public string? Name { get; set; }

    //Lista som innehåller kopplingar mellan syften och deltagare
    public ICollection<ParticipantPurpose>? ParticipantPurposes { get; set; }
}