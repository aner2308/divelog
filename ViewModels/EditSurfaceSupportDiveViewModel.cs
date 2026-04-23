using divelog.Models;

namespace divelog.ViewModels;

public class EditSurfaceSupportDiveViewModel
{
    public int DiveId { get; set; }

    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    
    public string? LocationName { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Notes { get; set; }

    public int? DivePurposeId { get; set; }

    public int? DiveLeaderId { get; set; }

    public int? DiverId { get; set; }

    public int? SurfaceSupportId { get; set; }

    public double? Depth { get; set; }

    public int? DiveTime { get; set; }

    public int? AirPressureBefore { get; set; }

    public int? AirPressureAfter { get; set; }

    public List<Person> AvailableDivers { get; set; } = new();

    public List<Person> DiveLeaders { get; set; } = new();

    public List<Person> SurfaceSupports { get; set; } = new();

    public List<DivePurpose> DivePurposes { get; set; } = new();
}