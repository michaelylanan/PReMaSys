using System.ComponentModel.DataAnnotations;

namespace PReMaSys.Models
{
    public class SalesPerformance
    {
        [Key]
        public int SalesID { get; set; }

        [Display(Name = "Sales Person")]
        public string SalesPerson { get; set; }

        [Display(Name = "Units Sold")]
        public decimal UnitsSold { get; set; }

        [Display(Name = "Cost Price Unit")]
        public decimal CostPricePerUnit { get; set; }

        [Display(Name = "Selling Price Unit")]
        public decimal SellingPricePerUnit { get; set; }

        [Display(Name = "Unit Type")]
        public string UnitType { get; set; }
        public string Particulars { get; set; }

        [Display(Name = "Sales Revenue")]
        public decimal? SalesRevenue { get; set; }

        [Display(Name = "Sales Profit")]
        public decimal? SalesProfit { get; set; }

        [Display(Name = "Sales Volume")]
        public int SalesVolume { get; set; }
        public decimal ConversionR { get; set; }

        [Display(Name = "Average Deal Size")]
        public decimal AverageDealSize { get; set; }
        [Display(Name = "Customer Acquisition")]
        public int CustomerAcquisition { get; set; }
        [Display(Name = "Customer Retention R")]
        public decimal CustomerRetentionR { get; set; }
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }
        [Display(Name = "User Image")]
        public byte[]? UserImage { get; set; }

        public SalesForecast SalesForecast { get; set; }


    }

    public class SalesForecast
    {
        [Key]
        public int ForecastID { get; set; }
        public int SPID { get; set; }
        [Display(Name = "Sales Person")]
        public string SalesPerson { get; set; }
        [Display(Name = "Sales Performance")]
        public SalesPerformance SalesPerformance { get; set; }
        [Display(Name = "Daily Forecast")]
        public decimal? DailyForecast { get; set; }
        [Display(Name = "Weekly Forecast")]
        public decimal? WeeklyForecast { get; set; }
        [Display(Name = "Monthly Forecast")]
        public decimal? MonthlyForecast { get; set; }
        [Display(Name = "Quarterly Forecast")]
        public decimal? QuarterlyForecast { get; set; }
        [Display(Name = "Yearly Forecast")]
        public decimal? YearlyForecast { get; set; }
    }
}
