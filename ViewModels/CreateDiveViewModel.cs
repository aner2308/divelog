using divelog.Models;

namespace divelog.ViewModels;

public class CreateDiveViewModel
{
    public DateTime Date { get; set; }
    public string? LocationName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Notes { get; set; }

    // Dykledare (en)
    public int DiveLeaderId { get; set; }

    // Typ av dyk (styr flik)
    public string DiveType { get; set; } = "LuftFrånYtan"; 

    public List<PairGroupViewModel> PairGroups { get; set; } = new();
    public List<DiveGroupViewModel> SurfaceSupportGroups { get; set; } = new();

    //Listor för dropdowns
    public List<Person> DiveLeaders { get; set; } = new();

    public List<Person> Divers { get; set; } = new();

    public List<Person> SurfaceSupports { get; set; } = new();
}