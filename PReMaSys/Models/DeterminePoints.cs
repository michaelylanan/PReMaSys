using PReMaSys.Data;
using System.Globalization;

namespace PReMaSys.Models
{
    public class DeterminePoints
    {
        private readonly ApplicationDbContext _context;

        public DeterminePoints(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CalculatePointsAndUpdateQuota(SalesPerformance salesPerformance)
        {
            var criteriaList = _context.PerformanceCriterias.ToList();
            var cMonth = DateTime.Today.Month;
            var cYear = DateTime.Today.Year;
            var cQuarter = (cMonth - 1) / 3 + 1;

            // Calculate and assign points for the current week
            CalculateAndAssignWeeklyPoints(salesPerformance, criteriaList);

            // Calculate and assign points for the current month
            CalculateAndAssignMonthlyPoints(salesPerformance, criteriaList, cMonth, cYear);

            // Calculate and assign points for the current quarter
            CalculateAndAssignQuarterlyPoints(salesPerformance, criteriaList, cQuarter, cYear);

            // Calculate and assign points for the current year
            CalculateAndAssignYearlyPoints(salesPerformance, criteriaList, cYear);

            _context.SaveChanges();




            foreach (var criteria in criteriaList)
            {
                // Check if points are already given for the salesperson and criteria in the current month
                var pointsAllocation = _context.PointsTracker
                    .FirstOrDefault(p => p.PerformanceCriteriaId == criteria.PerformanceCriteriaId &&
                                         p.SalesPerson == salesPerformance.SalesPerson &&
                                         p.TimeLine == salesPerformance.DateAdded.ToString("yyyy-MM"));

                if (pointsAllocation == null)
                {
                    int calculatedPoints = CalculatePoints(salesPerformance, criteria);

                    // Update the PointsTracker record for the corresponding criteria, salesperson, and time period
                    pointsAllocation = new PointsTracker
                    {
                        PerformanceCriteriaId = criteria.PerformanceCriteriaId,
                        SalesPerson = salesPerformance.SalesPerson,
                        TimeLine = salesPerformance.DateAdded.ToString("yyyy-MM"),
                        PerformancePoints = calculatedPoints,
                        DateAdded = DateTime.Today
                    };

                    _context.PointsTracker.Add(pointsAllocation);
                    _context.SaveChanges();

                    // Update the EmployeePoints in SERecord
                    var serRecord = _context.SERecord.FirstOrDefault(s => s.EmployeeNo == salesPerformance.SalesPerson);
                    if (serRecord != null)
                    {
                        serRecord.EmployeePoints += calculatedPoints;
                        _context.SaveChanges();
                    }
                }
            }
        }

        private void CalculateAndAssignQuarterlyPoints(SalesPerformance salesPerformance, List<PerformanceCriteria> criteriaList, int currentQuarter, int currentYear)
        {
            var firstDayOfQuarter = new DateTime(currentYear, (currentQuarter - 1) * 3 + 1, 1);
            var lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);

            // Check if points have already been assigned for the current date
            if (_context.PointsTracker.Any(p => p.DateAdded.Date == DateTime.Today))
            {
                return; // Points already assigned for the current date, exit the method
            }

            // Find the top three highest performers for each metric in the current quarter
            var topPerformersQuarterly = _context.SalesPerformances
                .Where(s => s.DateAdded >= firstDayOfQuarter && s.DateAdded <= lastDayOfQuarter)
                .GroupBy(s => s.SalesPerson)
                .Select(g => new
                {
                    SalesPerson = g.Key,
                    SalesRevenue = g.Sum(s => s.SalesRevenue),
                    GrossProfit = g.Sum(s => s.SellingPricePerUnit - s.CostPricePerUnit),
                    SalesVolume = g.Sum(s => s.SalesVolume),
                    ConversionRate = g.Average(s => s.ConversionR),
                    AverageDealSize = g.Average(s => s.AverageDealSize),
                    CustomerRetention = g.OrderByDescending(s => s.DateAdded)
                                         .Select(s => s.CustomerRetentionR)
                                         .FirstOrDefault()
                })
                .OrderByDescending(s => s.SalesRevenue)
                .Take(3)
                .ToList();

            // Assign points to the top performers in the current quarter
            for (int i = 0; i < topPerformersQuarterly.Count; i++)
            {
                var performer = topPerformersQuarterly[i];
                int points = 0;

                if (i == 0)
                {
                    // First Highest
                    points += 60; // Sales Revenue
                    points += 100; // Gross Profit
                    points += 60; // Sales Volume
                    points += 20; // Conversion Rate
                    points += 80; // Average Deal Size
                    points += 20; // Customer Retention
                }
                else if (i == 1)
                {
                    // Second Highest
                    points += 40; // Sales Revenue
                    points += 80; // Gross Profit
                    points += 40; // Sales Volume
                    points += 10; // Conversion Rate
                    points += 60; // Average Deal Size
                    points += 10; // Customer Retention
                }
                else if (i == 2)
                {
                    // Third Highest
                    points += 20; // Sales Revenue
                    points += 60; // Gross Profit
                    points += 20; // Sales Volume
                    points += 6; // Conversion Rate
                    points += 40; // Average Deal Size
                    points += 6; // Customer Retention
                }

                // Create a PointsTracker record for the performer and assign the points
                var pointsAllocation = new PointsTracker
                {
                    PerformanceCriteriaId = criteriaList[i].PerformanceCriteriaId,
                    SalesPerson = performer.SalesPerson,
                    TimeLine = DateTime.Today.ToString("yyyy-MM"),
                    PerformancePoints = points,
                    DateAdded = DateTime.Today
                };

                _context.PointsTracker.Add(pointsAllocation);

                // Update the EmployeePoints in SERecord
                var serRecord = _context.SERecord.FirstOrDefault(s => s.EmployeeNo == performer.SalesPerson);
                if (serRecord != null)
                {
                    serRecord.EmployeePoints += points;
                }
            }
        }

        private void CalculateAndAssignYearlyPoints(SalesPerformance salesPerformance, List<PerformanceCriteria> criteriaList, int currentYear)
        {
            var firstDayOfYear = new DateTime(currentYear, 1, 1);
            var lastDayOfYear = new DateTime(currentYear, 12, 31);

            // Check if points have already been assigned for the current date
            if (_context.PointsTracker.Any(p => p.DateAdded.Date == DateTime.Today))
            {
                return; // Points already assigned for the current date, exit the method
            }

            // Find the top three highest performers for each metric in the current year
            var topPerformersYearly = _context.SalesPerformances
                .Where(s => s.DateAdded >= firstDayOfYear && s.DateAdded <= lastDayOfYear)
                .GroupBy(s => s.SalesPerson)
                .Select(g => new
                {
                    SalesPerson = g.Key,
                    SalesRevenue = g.Sum(s => s.SalesRevenue),
                    GrossProfit = g.Sum(s => s.SellingPricePerUnit - s.CostPricePerUnit),
                    SalesVolume = g.Sum(s => s.SalesVolume),
                    ConversionRate = g.Average(s => s.ConversionR),
                    AverageDealSize = g.Average(s => s.AverageDealSize),
                    CustomerRetention = g.OrderByDescending(s => s.DateAdded)
                                         .Select(s => s.CustomerRetentionR)
                                         .FirstOrDefault()
                })
                .OrderByDescending(s => s.SalesRevenue)
                .Take(3)
                .ToList();

            // Assign points to the top performers in the current year
            for (int i = 0; i < topPerformersYearly.Count; i++)
            {
                var performer = topPerformersYearly[i];
                int points = 0;

                if (i == 0)
                {
                    // First Highest
                    points += 120; // Sales Revenue
                    points += 200; // Gross Profit
                    points += 120; // Sales Volume
                    points += 40; // Conversion Rate
                    points += 160; // Average Deal Size
                    points += 40; // Customer Retention
                }
                else if (i == 1)
                {
                    // Second Highest
                    points += 80; // Sales Revenue
                    points += 160; // Gross Profit
                    points += 80; // Sales Volume
                    points += 20; // Conversion Rate
                    points += 120; // Average Deal Size
                    points += 20; // Customer Retention
                }
                else if (i == 2)
                {
                    // Third Highest
                    points += 40; // Sales Revenue
                    points += 120; // Gross Profit
                    points += 40; // Sales Volume
                    points += 12; // Conversion Rate
                    points += 80; // Average Deal Size
                    points += 16; // Customer Retention
                }

                // Create a PointsTracker record for the performer and assign the points
                var pointsAllocation = new PointsTracker
                {
                    PerformanceCriteriaId = criteriaList[i].PerformanceCriteriaId,
                    SalesPerson = performer.SalesPerson,
                    TimeLine = currentYear.ToString(),
                    PerformancePoints = points,
                    DateAdded = DateTime.Today
                };

                _context.PointsTracker.Add(pointsAllocation);

                // Update the EmployeePoints in SERecord
                var serRecord = _context.SERecord.FirstOrDefault(s => s.EmployeeNo == performer.SalesPerson);
                if (serRecord != null)
                {
                    serRecord.EmployeePoints += points;
                }
            }
        }

        private void CalculateAndAssignMonthlyPoints(SalesPerformance salesPerformance, List<PerformanceCriteria> criteriaList, int cYear, int cYear1)
        {
            var cMonth = DateTime.Today.Month;
            var currentYear = DateTime.Today.Year;

            // Check if points have already been assigned for the current date
            if (_context.PointsTracker.Any(p => p.DateAdded.Date == DateTime.Today))
            {
                return; // Points already assigned for the current date, exit the method
            }


            // Find the top three highest performers for each metric in the current month
            var topPerformersMonthly = _context.SalesPerformances
                .Where(s => s.DateAdded.Month == cMonth && s.DateAdded.Year == currentYear)
                .GroupBy(s => s.SalesPerson)
                .Select(g => new
                {
                    SalesPerson = g.Key,
                    SalesRevenue = g.Sum(s => s.SalesRevenue),
                    GrossProfit = g.Sum(s => s.SellingPricePerUnit - s.CostPricePerUnit),
                    SalesVolume = g.Sum(s => s.SalesVolume),
                    ConversionRate = g.Average(s => s.ConversionR),
                    AverageDealSize = g.Average(s => s.AverageDealSize),
                    CustomerRetention = g.OrderByDescending(s => s.DateAdded)
                                         .Select(s => s.CustomerRetentionR)
                                         .FirstOrDefault()
                })
                .OrderByDescending(s => s.SalesRevenue)
                .Take(3)
                .ToList();

            // Assign points to the top performers in the current month
            for (int i = 0; i < topPerformersMonthly.Count; i++)
            {
                var performer = topPerformersMonthly[i];
                int points = 0;

                if (i == 0)
                {
                    // First Highest
                    points += 70; // Sales Revenue
                    points += 100; // Gross Profit
                    points += 50; // Sales Volume
                    points += 20; // Conversion Rate
                    points += 70; // Average Deal Size
                    points += 20; // Customer Retention
                }
                else if (i == 1)
                {
                    // Second Highest
                    points += 50; // Sales Revenue
                    points += 80; // Gross Profit
                    points += 40; // Sales Volume
                    points += 15; // Conversion Rate
                    points += 50; // Average Deal Size
                    points += 15; // Customer Retention
                }
                else if (i == 2)
                {
                    // Third Highest
                    points += 30; // Sales Revenue
                    points += 50; // Gross Profit
                    points += 30; // Sales Volume
                    points += 10; // Conversion Rate
                    points += 40; // Average Deal Size
                    points += 10; // Customer Retention
                }


                // Create a PointsTracker record for the performer and assign the points
                var pointsAllocation = new PointsTracker
                {
                    PerformanceCriteriaId = criteriaList[i].PerformanceCriteriaId,
                    SalesPerson = performer.SalesPerson,
                    TimeLine = cYear.ToString(),
                    PerformancePoints = points,
                    DateAdded = DateTime.Today
                };

                _context.PointsTracker.Add(pointsAllocation);

                // Update the EmployeePoints in SERecord
                var serRecord = _context.SERecord.FirstOrDefault(s => s.EmployeeNo == performer.SalesPerson);
                if (serRecord != null)
                {
                    serRecord.EmployeePoints += points;
                }
            }
        }

        private void CalculateAndAssignWeeklyPoints(SalesPerformance salesPerformance, List<PerformanceCriteria> criteriaList)
        {
            var cWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            // Check if points have already been assigned for the current date
            if (_context.PointsTracker.Any(p => p.DateAdded.Date == DateTime.Today))
            {
                return; // Points already assigned for the current date, exit the method
            }

            var topPerformersWeekly = _context.SalesPerformances
            .AsEnumerable() // Switch to client evaluation
            .Where(s => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(s.DateAdded, CalendarWeekRule.FirstDay, DayOfWeek.Monday) == cWeek && s.DateAdded.Year == DateTime.Today.Year)
            .GroupBy(s => s.SalesPerson)
            .Select(g => new
            {
                SalesPerson = g.Key,
                SalesRevenue = g.Sum(s => s.SalesRevenue),
                GrossProfit = g.Sum(s => s.SellingPricePerUnit - s.CostPricePerUnit),
                SalesVolume = g.Sum(s => s.SalesVolume),
                ConversionRate = g.Average(s => s.ConversionR),
                AverageDealSize = g.Average(s => s.AverageDealSize),
                CustomerRetention = g.OrderByDescending(s => s.DateAdded)
                                     .Select(s => s.CustomerRetentionR)
                                     .FirstOrDefault()
            })
            .OrderByDescending(s => s.SalesRevenue)
            .Take(3)
            .ToList();

            // Assign points to the top performers in the current week
            for (int i = 0; i < topPerformersWeekly.Count; i++)
            {
                var performer = topPerformersWeekly[i];
                int points = 0;

                if (i == 0)
                {
                    // First Highest
                    points += 30; // Sales Revenue
                    points += 50; // Gross Profit
                    points += 30; // Sales Volume
                    points += 10; // Conversion Rate
                    points += 40; // Average Deal Size
                    points += 10; // Customer Retention
                }
                else if (i == 1)
                {
                    // Second Highest
                    points += 20; // Sales Revenue
                    points += 40; // Gross Profit
                    points += 20; // Sales Volume
                    points += 5; // Conversion Rate
                    points += 30; // Average Deal Size
                    points += 5; // Customer Retention
                }
                else if (i == 2)
                {
                    // Third Highest
                    points += 10; // Sales Revenue
                    points += 30; // Gross Profit
                    points += 10; // Sales Volume
                    points += 3; // Conversion Rate
                    points += 20; // Average Deal Size
                    points += 3; // Customer Retention
                }

                // Create a PointsTracker record for the performer and assign the points
                var pointsAllocation = new PointsTracker
                {
                    PerformanceCriteriaId = criteriaList[i].PerformanceCriteriaId,
                    SalesPerson = performer.SalesPerson,
                    TimeLine = DateTime.Today.ToString("yyyy-MM"),
                    PerformancePoints = points,
                    DateAdded = DateTime.Today
                };

                _context.PointsTracker.Add(pointsAllocation);

                // Update the EmployeePoints in SERecord
                var serRecord = _context.SERecord.FirstOrDefault(s => s.EmployeeNo == performer.SalesPerson);
                if (serRecord != null)
                {
                    serRecord.EmployeePoints += points;
                }
            }
        }


        private int CalculatePoints(SalesPerformance salesPerformance, PerformanceCriteria criteria)
        {
            switch (criteria.RewardsCriteria)
            {
                case "Sales Revenue":
                    return CalculateSalesRevenuePoints(salesPerformance);
                case "Gross Profit":
                    return CalculateGrossProfitPoints(salesPerformance);
                case "Sales Volume":
                    return CalculateSalesVolumePoints(salesPerformance);
                case "Conversion Rate":
                    return CalculateConversionRatePoints(salesPerformance);
                case "Average Deal Size":
                    return CalculateAverageDealSizePoints(salesPerformance);
                case "Customer Acquisition":
                    return CalculateCustomerAcquisitionPoints(salesPerformance);
                case "Customer Retention":
                    return CalculateCustomerRetentionPoints(salesPerformance);
                default:
                    return 0;
            }
        }

        private int CalculateSalesRevenuePoints(SalesPerformance salesPerformance)
        {
            decimal salesRevenue = salesPerformance.SalesRevenue ?? 0;

            if (salesRevenue >= 1000000)
                return 10000;
            else if (salesRevenue >= 100000)
                return 1000;
            else if (salesRevenue >= 1000)
                return 100;
            else
                return 10;
        }

        private int CalculateGrossProfitPoints(SalesPerformance salesPerformance)
        {
            decimal grossProfit = salesPerformance.SellingPricePerUnit - salesPerformance.CostPricePerUnit;

            if (grossProfit >= 1000000)
                return 10000;
            else if (grossProfit >= 100000)
                return 1000;
            else if (grossProfit >= 10000)
                return 100;
            else
                return 0;
        }

        private int CalculateSalesVolumePoints(SalesPerformance salesPerformance)
        {
            int salesVolume = salesPerformance.SalesVolume;

            if (salesVolume >= 1000000)
                return 2000;
            else if (salesVolume >= 10000)
                return 400;
            else if (salesVolume >= 1000)
                return 100;
            else
                return 0;
        }

        private int CalculateConversionRatePoints(SalesPerformance salesPerformance)
        {
            decimal conversionRate = salesPerformance.ConversionR;
            decimal previousConversionRate = 0; // Get the previous conversion rate from the database for comparison

            if (conversionRate > previousConversionRate)
                return 10;
            else
                return 0;
        }

        private int CalculateAverageDealSizePoints(SalesPerformance salesPerformance)
        {
            decimal averageDealSize = salesPerformance.AverageDealSize;
            decimal previousAverageDealSize = 0; // Get the previous average deal size from the database for comparison

            if (averageDealSize - previousAverageDealSize >= 10000)
                return 100;
            else
                return 0;
        }

        private int CalculateCustomerAcquisitionPoints(SalesPerformance salesPerformance)
        {
            int customerAcquisition = salesPerformance.CustomerAcquisition;

            return 50;
        }

        private int CalculateCustomerRetentionPoints(SalesPerformance salesPerformance)
        {
            decimal customerRetentionRate = salesPerformance.CustomerRetentionR;

            // Get the previous customer retention rate from the database for comparison
            decimal previousCustomerRetentionRate = _context.SalesPerformances
                .Where(s => s.SalesPerson == salesPerformance.SalesPerson &&
                            s.DateAdded < salesPerformance.DateAdded)
                .OrderByDescending(s => s.DateAdded)
                .Select(s => s.CustomerRetentionR)
                .FirstOrDefault();

            if (customerRetentionRate > previousCustomerRetentionRate)
                return 60;
            else
                return 0;
        }

    }
}
