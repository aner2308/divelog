namespace divelog.ViewModels;

public class DiveGroupViewModel
{
    // Dykare (ska alltid finnas)
    public int? DiverId { get; set; }

    // Dykskötare (bara i vissa fall)
    public int? SurfaceSupportId { get; set; }

    // Dykdata
    public double? Depth { get; set; }
    public int? DiveTime { get; set; }
    public int? AirPressureBefore { get; set; }
    public int? AirPressureAfter { get; set; }

    // Syften (bara för dykare)
    public List<int> PurposeIds { get; set; } = new();
}