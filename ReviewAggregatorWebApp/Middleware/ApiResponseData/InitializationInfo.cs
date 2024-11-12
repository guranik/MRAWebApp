namespace ReviewAggregatorWebApp.Middleware.ApiResponseData
{
    public class InitializationInfo
    {
        public int LastProcessedPage { get; set; }
        public int RemainingRequests { get; set; }
        public DateTime LastRequestDate { get; set; }
    }
}
