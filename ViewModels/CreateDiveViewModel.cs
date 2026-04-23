using divelog.Models;

namespace divelog.ViewModels;

public class CreateDiveViewModel
{
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public string? LocationName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Notes { get; set; }
    public int? DivePurposeId { get; set; }
    // Dykledare (en)
    public int? DiveLeaderId { get; set; }

    // Typ av dyk (styr flik)
    public DiveType DiveType { get; set; } = DiveType.LuftFranYtan; 
    public List<PairGroupViewModel> PairGroups { get; set; } = new();
    public List<DiveGroupViewModel> SurfaceSupportGroups { get; set; } = new();

    //Listor för dropdowns
    public List<Person> DiveLeaders { get; set; } = new();

    public List<Person> Divers { get; set; } = new();

    public List<Person> SurfaceSupports { get; set; } = new();

    public List<DivePurpose> DivePurposes { get; set; } = new();
}