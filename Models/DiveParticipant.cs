namespace divelog.Models;

//DiveParticipant kopplar ihop Dive, Person och Role
public class DiveParticipant
{
    public int Id { get; set; }
    public int DiveId { get; set; }
    public int PersonId { get; set; }
    public int RoleId { get; set; }
    public double Depth { get; set; }
    public int DiveTime { get; set; }
    public int ExposureTime { get; set; }
    public double NitrogenLoad { get; set; }

    //Navigation till dyk
    public Dive? Dive { get; set; }
    //Navigation till person
    public Person Person { get; set; }
    //Navigation till roll
    public Role Role { get; set; }

    //Lista med syften som kopplas till deltagaren
    public ICollection<ParticipantPurpose>? ParticipantPurposes { get; set; }
}