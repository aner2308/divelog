namespace divelog.ViewModels;

public class EditDiveViewModel
{
    public int Id { get; set; }

    public DateTime Date { get; set; }
    public string? LocationName { get; set; }
    public string? Notes { get; set; }

    public List<EditParticipantViewModel> Participants { get; set; } = new();
}