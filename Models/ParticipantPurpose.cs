namespace divelog.Models;

//Kopplingstabell mellan DiveParticipant och DivePurpose
public class ParticipantPurpose
{
    public int Id { get; set; }
    //FK till deltagare i dykning
    public int DiveParticipantId { get; set; }
    //FK till syfte
    public int DivePurposeId { get; set; }

    //Navigation till deltagare
    public DiveParticipant? DiveParticipant { get; set; }
    //Navigation till syfte
    public DivePurpose? DivePurpose { get; set; }
}