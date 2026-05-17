public class StatisticsRowViewModel
{
    public string PersonName { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;

    public Dictionary<int, int> MonthlyCounts { get; set; } = new();

    public int Total => MonthlyCounts.Values.Sum();
    public int TotalDiveMinutes { get; set; }

    public string TotalDiveTimeFormatted
    {
        get
        {
            var hours = TotalDiveMinutes / 60;
            var minutes = TotalDiveMinutes % 60;

            return $"{hours}:{minutes:D2}";
        }
    }
}