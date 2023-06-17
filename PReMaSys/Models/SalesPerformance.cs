using PReMaSys.Data;
using System.ComponentModel.DataAnnotations;

namespace PReMaSys.Models
{
    public class SalesPerformance
    {
        [Key]
        public int SalesID { get; set; }
        public string SalesPerson { get; set; }
        public decimal UnitsSold { get; set; }
        public decimal CostPricePerUnit { get; set; }
        public decimal SellingPricePerUnit { get; set; }
        public string UnitType { get; set; }
        public string Particulars { get; set; }
        public decimal? SalesRevenue { get; set; }
        public decimal? SalesProfit { get; set; }
        public int SalesVolume { get; set; }
        public decimal ConversionR { get; set; }
        public decimal AverageDealSize { get; set; }
        public int CustomerAcquisition { get; set; }
        public decimal CustomerRetentionR { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateModified { get; set; }

        public byte[]? UserImage { get; set; }

        public SalesForecast SalesForecast { get; set; }

        
    }

    public class SalesForecast
    {
        [Key]
        public int ForecastID { get; set; }
        public int SPID { get; set; }
        public string SalesPerson { get; set; }
        public SalesPerformance SalesPerformance { get; set; }
        public decimal? DailyForecast { get; set; }
        public decimal? WeeklyForecast { get; set; }
        public decimal? MonthlyForecast { get; set; }
        public decimal? QuarterlyForecast { get; set; }
        public decimal? YearlyForecast { get; set; }
    }
}
