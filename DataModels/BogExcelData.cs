namespace ProcessServices.DataModels
{
    public enum status
    {
        Valid,
        Invalid
    }
    public class BogExcelData
    {
        public string User { get; set; }
        public TimeSpan TotalTime { get; set; }
        public status TourValidStatus { get; set; }
        
    }
}
