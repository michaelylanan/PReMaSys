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
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;

//using Accord.MachineLearning.VectorMachines.SupportVectorMachines;
//using Accord.MachineLearning.VectorMachines.SupportVectorMachines.Kernels;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Linear;
using Azure.ResourceManager.MachineLearning.Models;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;


namespace PReMaSys.Controllers
{
    //[Authorize(Roles = "Admin")]
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

        public IActionResult ARIMA2()
        {
            var salesPerformances = _context.SalesPerformances.ToList();
            List<decimal?> salesProfitList = new List<decimal?>();
            List<DateTime> dateAddedList = new List<DateTime>();

            // Group sales performances by month
            var salesByMonth = salesPerformances
                .GroupBy(sp => new { sp.DateAdded.Year, sp.DateAdded.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalProfit = g.Sum(sp => sp.SalesProfit)
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month);

            // Iterate through each grouped month and calculate the sum of profits
            foreach (var salesMonth in salesByMonth)
            {
                decimal? totalProfit = salesMonth.TotalProfit;

                // Add the total profit to the list
                salesProfitList.Add(totalProfit);

                // Create a DateTime object for the month
                DateTime monthDate = new DateTime(salesMonth.Year, salesMonth.Month, 1);

                // Add the month date to the list
                dateAddedList.Add(monthDate);
            }

            //Perform differencing on the salesprofit
            List<decimal?> differentiationList = new List<decimal?>();
            for (int i = 1; i < salesProfitList.Count; i++)
            {
                decimal? differentiation = salesProfitList[i] - salesProfitList[i - 1];
                differentiationList.Add(differentiation);
            }

            // - - - - - - - - - - -

            // Convert salesProfitList to a vector
            Vector<double> salesProfitVector = Vector<double>.Build.DenseOfEnumerable(salesProfitList.Select(x => (double)x));

            // Define the autoregressive order (p), differencing order (d), and moving average order (q). We are using Standard Arima Model where p, d and q are 1.
            int p = 1; // Autoregressive order
            int d = 1; // Differencing order
            int q = 1; // Moving average order

            // Apply differencing to the sales profit vector
            Vector<double> differentiationVector = Vector<double>.Build.DenseOfEnumerable(differentiationList.Select(d => (double)d));

            // Prepare the data for autoregressive and moving average modeling
            int n = differentiationVector.Count - Math.Max(p, q);
            Matrix<double> inputs = Matrix<double>.Build.Dense(n, p + q);
            Vector<double> outputs = differentiationVector.SubVector(Math.Max(p, q), n);

            for (int i = 0; i < n; i++)
            {
                inputs[i, 0] = differentiationVector[i];
                inputs[i, 1] = outputs[i];
            }

            // Fit an autoregressive and moving average model
            Vector<double> arCoefficients = MultipleRegression.QR(inputs, outputs);

            // Get the autoregressive and moving average coefficients
            double arCoefficient = arCoefficients[0];
            double maCoefficient = arCoefficients[1];
            double dCoefficient = 2;
           

            // Compute the forecast for one value
            double arComponent = arCoefficient * differentiationVector[differentiationVector.Count - 1];
            double maComponent = maCoefficient * salesProfitVector[salesProfitVector.Count - 1];

            //double forecast = salesProfitVector[salesProfitVector.Count - 1] + arComponent + maComponent;

            double forecast = (salesProfitVector[salesProfitVector.Count - 1] + arComponent + maComponent)* dCoefficient;


            ViewBag.salesProfitListByMonth = salesProfitList; //testing - remove after
            ViewBag.dateAddedByMonth = dateAddedList; //testing - remove after

            // Add the forecast value to the salesProfitList
            salesProfitList.Add((decimal?)forecast);


            var newDate = DateTime.Now.AddMonths(1);
            dateAddedList.Add(new DateTime(newDate.Year, newDate.Month, 1));



            ViewBag.SalesProfitList = salesProfitList;
            ViewBag.DateAddedList = dateAddedList;
            ViewBag.getForecast = forecast;
            return View();
        }
        public IActionResult ARIMA()
        {
            var salesPerformances = _context.SalesPerformances.ToList();
            List<decimal?> salesProfitList = new List<decimal?>();
            List<DateTime> dateAddedList = new List<DateTime>();

            foreach (var performance in salesPerformances)
            {
                var dateAdded = performance.DateAdded;
                var salesProfit = performance.SalesProfit;

                salesProfitList.Add(salesProfit);
                dateAddedList.Add(dateAdded);

                // Use the dateAdded and salesProfit variables as needed
            }

            //ForecastingModel.AutoArima fm = new ForecastingModel("bruh");
           

            decimal?[] salesArray = salesProfitList.ToArray();
            DateTime[] dateArray = dateAddedList.ToArray();

            var regression = new OrdinaryLeastSquares();
            double[][] inputs = new double[salesArray.Length - 1][];
            double[] outputs = new double[salesArray.Length - 1];

            for (int i = 1; i < salesArray.Length; i++)
            {
                inputs[i - 1] = new double[] { (double)salesArray[i - 1] };
                outputs[i - 1] = (double)salesArray[i];
            }

            MultipleLinearRegression arima = regression.Learn(inputs, outputs);

            int forecastPeriods = 10; // Adjust this value as per your requirement

            double[] futureInputs = new double[forecastPeriods];
            for (int i = 0; i < forecastPeriods; i++)
            {
                futureInputs[i] = (double)salesArray[salesArray.Length - forecastPeriods + i];
            }

            double[] futureForecasts = new double[forecastPeriods];

            for (int i = 0; i < forecastPeriods; i++)
            {
                futureForecasts[i] = arima.Transform(new double[] { futureInputs[i] });
            }

            List<decimal> forecastedRevenue = futureForecasts.Select(x => (decimal)x).ToList();
            List<DateTime> forecastedDates = new List<DateTime>();

            DateTime lastDate = dateAddedList[dateAddedList.Count - 1];
            for (int i = 1; i <= forecastPeriods; i++)
            {
                forecastedDates.Add(lastDate.AddMonths(i));
            }

            ViewBag.ForecastedRevenue = forecastedRevenue;
            ViewBag.ForecastedDates = forecastedDates;


            return View();
        }

        public IActionResult Payment()
        {

            return View();
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
            string userId = _userManager.GetUserId(HttpContext.User);

            var list = _context.SalesPerformances.Where(a => a.LoggedUser == userId).ToList();
            ViewBag.User = list;

            return View();
        }

        public IActionResult Descriptive()
        {
            string userId = _userManager.GetUserId(HttpContext.User);

            var list = _context.SalesPerformances.Where(a => a.LoggedUser == userId).ToList();
            ViewBag.User = list;

            return View();
        }

        /*View of Rewards List -----------------------------------------------------------------------------------------------------------*/
        public IActionResult Reward(string id)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            var checker = _context.ApplicationUsers.FirstOrDefault(a => a.Id == userId);
            var checker2 = checker.AddedBy;

            var supportUser = _context.ApplicationUsers.Where(u => u.AddedBy == checker2 && u.Role == "Support").Select(u => u.Id).FirstOrDefault();
            var list = _context.Rewards.Where(r => r.SupportId == supportUser).ToList();

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

            string userId = _userManager.GetUserId(HttpContext.User);

            var salesP = _context.SalesPerformances.Where(s => s.LoggedUser == userId).ToList();

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
