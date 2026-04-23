namespace divelog.ViewModels;

public class EditParticipantViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Signature { get; set; }

    public int DiveRoleId { get; set; }

    public double? Depth { get; set; }
    public int? DiveTime { get; set; }
    public int? AirPressureBefore { get; set; }
    public int? AirPressureAfter { get; set; }
}