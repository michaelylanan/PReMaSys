using EllipticCurve;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PReMaSys.Data;
using PReMaSys.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using PReMaSys.ViewModel;
using System.Globalization;

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
            UpdateSalesPersonOfTheMonthAndYear();

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

            return RedirectToAction("SPerformanceList");
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

        public IActionResult SalesProfitForecast()
        {
            var salesData = _context.SalesPerformances.ToList();
            var chartData = salesData.Select(x => new SalesPerformance { DateAdded = x.DateAdded, SalesProfit = x.SalesProfit }).ToList();
            return View(chartData);
        }

        public void UpdateSalesPersonOfTheMonthAndYear()
        {
            // Get the current month
            var currentMonth = DateTime.Now.Month;

            // Get all distinct SalesPerson values from SalesPerformances table
            var salesPersons = _context.SalesPerformances
                .Where(sp => sp.DateAdded.Month == currentMonth)
                .Select(sp => sp.SalesPerson)
                .Distinct()
                .ToList();

            foreach (var salesPerson in salesPersons)
            {
                // Check if the EmployeeofThe record exists for the sales person
                var employeeOfTheMonth = _context.EmployeeofThes.FirstOrDefault(e => e.SalesPerson == salesPerson);


                if (employeeOfTheMonth != null)
                {
                    // Check if the DateModified is not in the current month
                    if (employeeOfTheMonth.DateModified?.Month != currentMonth)
                    {
                        // Check if the sales person has the highest sales profit in the current month
                        var highestSalesPerson = _context.SalesPerformances
                            .Where(sp => sp.DateAdded.Month == currentMonth && sp.SalesPerson == salesPerson)
                            .OrderByDescending(sp => sp.SalesProfit)
                            .FirstOrDefault();

                        if (highestSalesPerson != null)
                        {
                            // Update the existing EmployeeofThe record
                            employeeOfTheMonth.EmployeeOfTheMonth += 1;
                            employeeOfTheMonth.DateModified = DateTime.Now;

                            // Check if the sales person has accumulated points for SalesPerson of the Month
                            if (employeeOfTheMonth.EmployeeOfTheMonth % 7 == 0)
                            {
                                // Update the EmployeeofTheYear
                                employeeOfTheMonth.EmployeeOfTheYear += 1;
                            }
                        }
                    }
                }
                else
                {
                    // Create a new EmployeeofThe record
                    var newEmployeeOfTheMonth = new EmployeeofThe
                    {
                        SalesPerson = salesPerson,
                        EmployeeOfTheMonth = 1,
                        EmployeeOfTheYear = 0,
                        DateModified = DateTime.Now
                    };

                    // Add the new record to the context
                    _context.EmployeeofThes.Add(newEmployeeOfTheMonth);
                }
            }

            // Save changes to the database
            _context.SaveChanges();
        }

    }
}
