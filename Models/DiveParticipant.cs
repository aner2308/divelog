namespace divelog.Models;

//DiveParticipant kopplar ihop Dive, Person och Role
public class DiveParticipant
{
    public int Id { get; set; }
    public int DiveId { get; set; }
    public int PersonId { get; set; }
    public int DiveRoleId { get; set; }
    public double? Depth { get; set; }
    public int? DiveTime { get; set; }
    public int? ExposureTime { get; set; }
    public double? NitrogenLoad { get; set; }

    //Luftförbrukning vid pardyk
    public int? AirPressureBefore { get; set; }
    public int? AirPressureAfter { get; set; }

    //Navigation till dyk
    public Dive Dive { get; set; } = null!;
    //Navigation till person
    public Person Person { get; set; } = null!;
    //Navigation till roll
    public DiveRole DiveRole { get; set; } = null!;

    //Lista med syften som kopplas till deltagaren
    public ICollection<ParticipantPurpose>? ParticipantPurposes { get; set; } = new List<ParticipantPurpose>();
}