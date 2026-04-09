namespace divelog.Models;

public class Dive
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? LocationName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Notes { get; set; }

    //Lista som innehåller alla deltagare kopplade till ett dyk
    public ICollection<DiveParticipant>? DiveParticipants { get; set; }
}