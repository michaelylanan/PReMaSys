using EllipticCurve;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PReMaSys.Data;
using PReMaSys.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using PReMaSys.ViewModel;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using System;

namespace PReMaSys.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DeterminePoints _determinePoints;
        private readonly UserManager<ApplicationUser> _userManager;


        public AdminController(ApplicationDbContext context, DeterminePoints determinePoints, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _determinePoints = determinePoints;
            _userManager = userManager;


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
            string userId = _userManager.GetUserId(HttpContext.User);

            var list = _context.SalesPerformances.Where(a => a.LoggedUser == userId).ToList();
            ViewBag.User = list;

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
            ApplicationUser u = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));
            string au = u?.Id.ToString();
            var list = _context.SalesPerformances.Where(a => a.LoggedUser == au).ToList();
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

            SEmployees.EmployeePoints = Convert.ToInt32(temp);
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

            SEmployees.EmployeePoints = Convert.ToInt32(temp);
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
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User));
            string au = user?.Id.ToString();

            var sales = new SalesPerformance()
            {
                LoggedUser = au,
                SalesPerson = record.SalesPerson,
                UnitsSold = record.UnitsSold,
                CostPricePerUnit = record.CostPricePerUnit,
                SellingPricePerUnit = record.SellingPricePerUnit,
                UnitType = record.UnitType,
                Particulars = record.Particulars,
                SalesRevenue = (record.UnitsSold * record.SellingPricePerUnit),
                SalesProfit = (record.UnitsSold * record.SellingPricePerUnit) - (record.UnitsSold * record.CostPricePerUnit),
                SalesVolume = Convert.ToInt32(record.UnitsSold),
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
            var salesP = _context.SalesPerformances.ToList();

            var combP = salesP.GroupBy(s => s.SalesPerson).Select(g => new SalesPerformance
                {
                    SalesPerson = g.Key,
                    UnitsSold = g.Sum(s => s.UnitsSold),
                    SalesRevenue = g.Sum(s => s.SalesRevenue),
                    SalesProfit = g.Sum(s => s.SalesProfit)
                }).ToList();

            var topThree = combP.OrderByDescending(s => s.UnitsSold).Take(3).ToList();
            var remaining = combP.Except(topThree).ToList();

            var model = new SalesPerformanceRankingViewModel
            {
                TopThreeP = topThree,
                RemainingP = remaining
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
            var CMonth = DateTime.Now.Month;

            var sPersons = _context.SalesPerformances.Where(sp => sp.DateAdded.Month == CMonth).Select(sp => sp.SalesPerson).Distinct().ToList();

            foreach (var salesPerson in sPersons)
            {
                var eotm = _context.EmployeeofThes.FirstOrDefault(e => e.SalesPerson == salesPerson);
                if (eotm != null)
                {
                    if (eotm.DateModified?.Month != CMonth)
                    {
                        var highestSalesPerson = _context.SalesPerformances.Where(sp => sp.DateAdded.Month == CMonth && sp.SalesPerson == salesPerson).OrderByDescending(sp => sp.SalesProfit).FirstOrDefault();

                        if (highestSalesPerson != null)
                        {
                            eotm.EmployeeOfTheMonth += 1;
                            eotm.DateModified = DateTime.Now;

                            if (eotm.EmployeeOfTheMonth % 7 == 0)
                            {
                                // Update the EmployeeofTheYear
                                eotm.EmployeeOfTheYear += 1;
                            }
                        }
                    }
                }
                else
                {
                    var newEmployeeOfTheMonth = new EmployeeofThe
                    {
                        SalesPerson = salesPerson,
                        EmployeeOfTheMonth = 1,
                        EmployeeOfTheYear = 0,
                        DateModified = DateTime.Now
                    };

                    _context.EmployeeofThes.Add(newEmployeeOfTheMonth);
                }
            }
            _context.SaveChanges();
        }

        public IActionResult SalesCriteria()
        {
            var list = _context.PointsAllocation.ToList();
            return View(list);
        }

        public IActionResult DeterminePoints()
        {
            var cMonth = DateTime.Today.Month;
            var cYear = DateTime.Today.Year;

            var salesPerformances = _context.SalesPerformances.Where(s => s.DateAdded.Month == cMonth && s.DateAdded.Year == cYear).ToList();

            foreach (var salesPerformance in salesPerformances)
            {
                _determinePoints.CPointsAndUQuota(salesPerformance);
            }

            return RedirectToAction("ESalesProfitPoints", "Admin");
        }
    }


}
