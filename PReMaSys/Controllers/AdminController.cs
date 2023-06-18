using EllipticCurve;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PReMaSys.Data;
using PReMaSys.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using PReMaSys.ViewModel;

namespace PReMaSys.Controllers
{
   // [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;


        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ReportsPage() 
        {
            return View();
        }
      
        public IActionResult AdminDashboard() 
        {
            return View();
        }

        public IActionResult Ranking() 
        {
            return View();
        }

        public IActionResult Forecasts() 
        {
            return View();
        }

        public IActionResult Diagnostic() 
        {
            return View();
        }

        public IActionResult Descriptive() 
        {
            return View();
        }

        /*View of Rewards List -----------------------------------------------------------------------------------------------------------*/
        public IActionResult Reward() 
        {
            var list = _context.Rewards.ToList();
            return View(list);
        }


        /*Allocation of Sales-Profit Points-----------------------------------------------------------------------------------------------------------*/
        public IActionResult ESalesProfitPoints() 
        {
            var list = _context.SERecord.ToList();
            return View(list);
        }


        public IActionResult SPerformanceList()
        {
            var list = _context.SalesPerformances.ToList();
            return View(list);
        }


        public IActionResult AddPoints(int? id) 
        {
            if (id == null)
            {
                return RedirectToAction("ESalesProfitPoints");
            }

            var SEmployees = _context.SERecord.Where(r => r.SEmployeeRecordsID == id).SingleOrDefault();

            if (SEmployees == null)
            {
                return RedirectToAction("ESalesProfitPoints");
            }

            return View(SEmployees);
        }

        [HttpPost]
        public IActionResult AddPoints(int? id, SalesEmployeeRecord record, decimal temp) 
        {
            var SEmployees = _context.SERecord.Where(s => s.SEmployeeRecordsID == id).SingleOrDefault();

            SEmployees.EmployeeNo = record.EmployeeNo;
            SEmployees.EmployeeLastname = record.EmployeeLastname;

            temp = Convert.ToDecimal(SEmployees.EmployeePoints) + Convert.ToDecimal(record.EmployeePoints);

            SEmployees.EmployeePoints = temp.ToString();
            SEmployees.DateModified = DateTime.Now;

            _context.SERecord.Update(SEmployees);
            _context.SaveChanges();

            return RedirectToAction("ESalesProfitPoints");
        }

        public IActionResult DeductPoints(int? id) 
        {
            if (id == null)
            {
                return RedirectToAction("ESalesProfitPoints");
            }

            var SEmployees = _context.SERecord.Where(r => r.SEmployeeRecordsID == id).SingleOrDefault();

            if (SEmployees == null)
            {
                return RedirectToAction("ESalesProfitPoints");
            }

            return View(SEmployees);
        }

        [HttpPost]
        public IActionResult DeductPoints(int? id, SalesEmployeeRecord record, decimal temp) 
        {
            var SEmployees = _context.SERecord.Where(s => s.SEmployeeRecordsID == id).SingleOrDefault();

            SEmployees.EmployeeNo = record.EmployeeNo;
            SEmployees.EmployeeLastname = record.EmployeeLastname;

            temp = Convert.ToDecimal(SEmployees.EmployeePoints) - Convert.ToDecimal(record.EmployeePoints);

            SEmployees.EmployeePoints = temp.ToString();
            SEmployees.DateModified = DateTime.Now;

            _context.SERecord.Update(SEmployees);
            _context.SaveChanges();

            return RedirectToAction("ESalesProfitPoints");
        }



        //Sales Peformance Parameters
        public IActionResult SalesPerformance()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult SalesPerformance(SalesPerformance record)
        {
            var sales = new SalesPerformance()
            {
                SalesPerson = record.SalesPerson,
                UnitsSold = record.UnitsSold,
                CostPricePerUnit = record.CostPricePerUnit,
                SellingPricePerUnit = record.SellingPricePerUnit,
                UnitType = record.UnitType,
                Particulars = record.Particulars,
                SalesRevenue = (record.UnitsSold * record.SellingPricePerUnit),
                SalesProfit = (record.UnitsSold * record.SellingPricePerUnit) - (record.UnitsSold * record.CostPricePerUnit),
                SalesVolume = record.SalesVolume,
                ConversionR = record.ConversionR,
                AverageDealSize = record.AverageDealSize,
                CustomerAcquisition = record.CustomerAcquisition,
                CustomerRetentionR = record.CustomerRetentionR,
                DateAdded = DateTime.Now
            };

            // Check if there is an existing forecast for the salesperson
            var existingForecast = _context.SalesForecasts.FirstOrDefault(sf => sf.SPID == record.SalesID && sf.SalesPerson == record.SalesPerson);
            if (existingForecast != null)
            {
                // Update the existing forecast instead of creating a new one
                existingForecast.DailyForecast = CalculateForecastedDailySalesRevenue(record.SalesPerson);
                existingForecast.WeeklyForecast = CalculateForecastedWeeklySalesRevenue(record.SalesPerson);
                existingForecast.MonthlyForecast = CalculateForecastedMonthlySalesRevenue(record.SalesPerson);
                existingForecast.QuarterlyForecast = CalculateForecastedQuarterlySalesRevenue(record.SalesPerson);
                existingForecast.YearlyForecast = CalculateForecastedYearlySalesRevenue(record.SalesPerson);
            }
            else
            {
                // Create a new forecast record
                var forecast = new SalesForecast()
                {
                    SalesPerson = record.SalesPerson,
                    DailyForecast = CalculateForecastedDailySalesRevenue(record.SalesPerson),
                    WeeklyForecast = CalculateForecastedWeeklySalesRevenue(record.SalesPerson),
                    MonthlyForecast = CalculateForecastedMonthlySalesRevenue(record.SalesPerson),
                    QuarterlyForecast = CalculateForecastedQuarterlySalesRevenue(record.SalesPerson),
                    YearlyForecast = CalculateForecastedYearlySalesRevenue(record.SalesPerson)
                };

                sales.SalesForecast = forecast;
                _context.SalesForecasts.Add(forecast);
            }

            _context.SalesPerformances.Add(sales);
            _context.SaveChanges();

            return RedirectToAction("SPerformanceList");
        }
        public IActionResult Update(int? id)
        {
            // Retrieve the SalesPerformance object to edit from the database or any other source
            SalesPerformance salesPerformance = _context.SalesPerformances.Find(id);

            if (salesPerformance == null)
            {
                return NotFound();
            }

            return View(salesPerformance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int? id, SalesPerformance record)
        {
            var recs = _context.SalesPerformances.Find(id);
            recs.UnitsSold = record.UnitsSold;
            recs.CostPricePerUnit = record.CostPricePerUnit;
            recs.SellingPricePerUnit = record.SellingPricePerUnit;
            recs.UnitType = record.UnitType;
            recs.Particulars = record.Particulars;
            recs.DateModified = DateTime.Now;

            _context.SalesPerformances.Update(recs);
            _context.SaveChanges();

            // Check if there is an existing forecast for the salesperson
            var existingForecast = _context.SalesForecasts.FirstOrDefault(sf => sf.SPID == record.SalesID && sf.SalesPerson == record.SalesPerson);
            if (existingForecast != null)
            {
                // Update the existing forecast instead of creating a new one
                existingForecast.DailyForecast = CalculateForecastedDailySalesRevenue(record.SalesPerson);
                existingForecast.WeeklyForecast = CalculateForecastedWeeklySalesRevenue(record.SalesPerson);
                existingForecast.MonthlyForecast = CalculateForecastedMonthlySalesRevenue(record.SalesPerson);
                existingForecast.QuarterlyForecast = CalculateForecastedQuarterlySalesRevenue(record.SalesPerson);
                existingForecast.YearlyForecast = CalculateForecastedYearlySalesRevenue(record.SalesPerson);
            }
            else
            {
                // Create a new forecast record
                var forecast = new SalesForecast()
                {
                    SalesPerson = record.SalesPerson,
                    DailyForecast = CalculateForecastedDailySalesRevenue(record.SalesPerson),
                    WeeklyForecast = CalculateForecastedWeeklySalesRevenue(record.SalesPerson),
                    MonthlyForecast = CalculateForecastedMonthlySalesRevenue(record.SalesPerson),
                    QuarterlyForecast = CalculateForecastedQuarterlySalesRevenue(record.SalesPerson),
                    YearlyForecast = CalculateForecastedYearlySalesRevenue(record.SalesPerson)
                };
                _context.SalesForecasts.Add(forecast);
            }
            _context.SaveChanges();

            return RedirectToAction("SPerformanceList");
        }

        // Calculate forecasted sales revenue per day
        public decimal? CalculateForecastedDailySalesRevenue(string SalesPerson)
        {
            DateTime currentDate = DateTime.Today;

            // Get the start and end dates for the current day
            DateTime startDate = currentDate.Date;
            DateTime endDate = startDate.AddDays(1);

            // Get the two most recent sales records for the current day
            var recentSalesRecords = _context.SalesPerformances
                .Where(s => s.DateAdded >= startDate && s.DateAdded < endDate && s.SalesPerson == SalesPerson)
                .OrderByDescending(s => s.DateAdded)
                .Take(2)
                .ToList();

            // Calculate the average sales revenue for the current day
            decimal? averageSalesRevenue = recentSalesRecords.Count > 0 ? recentSalesRecords.Average(s => s.SalesProfit) : null;

            return averageSalesRevenue;
        }

        // Calculate forecasted sales revenue per week
        public decimal? CalculateForecastedWeeklySalesRevenue(String SalesPerson)
        {
            DateTime currentDate = DateTime.Today;

            // Get the start and end dates for the current week
            DateTime startDate = currentDate.AddDays(-((int)currentDate.DayOfWeek));
            DateTime endDate = startDate.AddDays(7);

            // Get the two most recent sales records for the current week
            var recentSalesRecords = _context.SalesPerformances
                .Where(s => s.DateAdded >= startDate && s.DateAdded < endDate && s.SalesPerson == SalesPerson)
                .OrderByDescending(s => s.DateAdded)
                .Take(2)
                .ToList();

            // Calculate the average sales revenue for the current week
            decimal? averageSalesRevenue = recentSalesRecords.Count > 0 ? recentSalesRecords.Average(s => s.SalesProfit) : null;

            return averageSalesRevenue;
        }

        // Calculate forecasted sales revenue per month
        public decimal? CalculateForecastedMonthlySalesRevenue(String SalesPerson)
        {
            DateTime currentDate = DateTime.Today;

            // Get the start and end dates for the current month
            DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime endDate = startDate.AddMonths(1);

            // Get the two most recent sales records for the current month
            var recentSalesRecords = _context.SalesPerformances
                .Where(s => s.DateAdded >= startDate && s.DateAdded < endDate && s.SalesPerson == SalesPerson)
                .OrderByDescending(s => s.DateAdded)
                .Take(2)
                .ToList();

            // Calculate the average sales revenue for the current month
            decimal? averageSalesRevenue = recentSalesRecords.Count > 0 ? recentSalesRecords.Average(s => s.SalesProfit) : null;

            return averageSalesRevenue;
        }

        // Calculate forecasted sales revenue per quarter
        public decimal? CalculateForecastedQuarterlySalesRevenue(String SalesPerson)
        {
            DateTime currentDate = DateTime.Today;

            // Get the start and end dates for the current quarter
            DateTime startDate = new DateTime(currentDate.Year, (currentDate.Month - 1) / 3 * 3 + 1, 1);
            DateTime endDate = startDate.AddMonths(3);

            // Get the two most recent sales records for the current quarter
            var recentSalesRecords = _context.SalesPerformances
                .Where(s => s.DateAdded >= startDate && s.DateAdded < endDate && s.SalesPerson == SalesPerson)
                .OrderByDescending(s => s.DateAdded)
                .Take(2)
                .ToList();

            // Calculate the average sales revenue for the current quarter
            decimal? averageSalesRevenue = recentSalesRecords.Count > 0 ? recentSalesRecords.Average(s => s.SalesProfit) : null;

            return averageSalesRevenue;
        }

        // Calculate forecasted sales revenue per year
        public decimal? CalculateForecastedYearlySalesRevenue(String SalesPerson)
        {
            DateTime currentDate = DateTime.Today;

            // Get the start and end dates for the current year
            DateTime startDate = new DateTime(currentDate.Year, 1, 1);
            DateTime endDate = startDate.AddYears(1);

            // Get the two most recent sales records for the current year
            var recentSalesRecords = _context.SalesPerformances
                .Where(s => s.DateAdded >= startDate && s.DateAdded < endDate && s.SalesPerson == SalesPerson)
                .OrderByDescending(s => s.DateAdded)
                .Take(2)
                .ToList();

            // Calculate the average sales revenue for the current year
            decimal? averageSalesRevenue = recentSalesRecords.Count > 0 ? recentSalesRecords.Average(s => s.SalesProfit) : null;

            return averageSalesRevenue;
        }

        public IActionResult DeleteSP(int? id) //Good
        {
            if (id == null)
            {
                return RedirectToAction("SPerformanceList");
            }

            var sales = _context.SalesPerformances.Where(r => r.SalesID == id).SingleOrDefault();
            if (sales == null)
            {
                return RedirectToAction("SPerformanceList");
            }
            _context.SalesPerformances.Remove(sales);
            _context.SaveChanges();

            return RedirectToAction("SPerformanceList");
        }


        public IActionResult SalesPerformanceRanking()
        {
            // Retrieve all sales performances
            var allPerformances = _context.SalesPerformances.ToList();

            // Combine the data for sales performances with the same salesperson
            var combinedPerformances = allPerformances
                .GroupBy(s => s.SalesPerson)
                .Select(g => new SalesPerformance
                {
                    SalesPerson = g.Key,
                    UnitsSold = g.Sum(s => s.UnitsSold),
                    SalesRevenue = g.Sum(s => s.SalesRevenue),
                    SalesProfit = g.Sum(s => s.SalesProfit)
                })
                .ToList();

            // Retrieve the top three sales performances based on a criterion (e.g., UnitsSold, SalesRevenue, SalesProfit)
            var topThreePerformances = combinedPerformances
                .OrderByDescending(s => s.UnitsSold)
                .Take(3)
                .ToList();

            // Exclude the top three performances from the remaining performances
            var remainingPerformances = combinedPerformances
                .Except(topThreePerformances)
                .ToList();

            // Pass the data to the view
            var model = new SalesPerformanceRankingViewModel
            {
                TopThreePerformances = topThreePerformances,
                RemainingPerformances = remainingPerformances
            };

            return View(model);

        }
    }
}
