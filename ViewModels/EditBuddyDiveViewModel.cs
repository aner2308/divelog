using divelog.Models;

namespace divelog.ViewModels;

public class EditBuddyDiveViewModel
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

    public List<PairDiverViewModel> Divers { get; set; } = new();

    public List<Person> AvailableDivers { get; set; } = new();

    public List<Person> DiveLeaders { get; set; } = new();

    public List<DivePurpose> DivePurposes { get; set; } = new();
}