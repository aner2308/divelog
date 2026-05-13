namespace divelog.ViewModels
{
    public class DivePurposeStatisticsRowViewModel
    {
        public string PersonName { get; set; } = "";
        public string Signature { get; set; } = "";

        public Dictionary<string, int> PurposeCounts { get; set; } = new();

        public int Total =>
            PurposeCounts.Values.Sum();
    }
}