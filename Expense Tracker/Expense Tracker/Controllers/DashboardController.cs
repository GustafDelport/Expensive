using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Spreadsheet;
using System.Globalization;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDBContext _context;
        public DashboardController(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Index()
        {
            var now = DateTime.Now;
            var DaysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

            DateTime EndDate = new DateTime(now.Year, now.Month, DaysInMonth);
            //DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime StartDate = new DateTime(now.Year, now.Month, 1);

            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-ZA");
            culture.NumberFormat.CurrencyNegativePattern = 1;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            //TotalIncome
            int TotalIncome = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);

            ViewBag.TotalIncome = String.Format(culture, "{0:C0}", TotalIncome);

            //TotalExpense
            int TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);

            ViewBag.TotalExpense = String.Format(culture, "{0:C0}",TotalExpense);

            //Balance
            int Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = String.Format(culture, "{0:C0}",Balance);

            //TotalExpense
            int TotalInvestment = SelectedTransactions
                .Where(i => i.Category.Type == "Investments")
                .Sum(j => j.Amount);

            ViewBag.TotalInvestment = String.Format(culture, "{0:C0}", TotalInvestment);

            //Chart Data

            ViewBag.DougnutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formatedAmount = k.Sum(j => j.Amount).ToString("C0")
                })
                .OrderByDescending(l => l.amount)
                .ToList();

            //Spline Chart

            //Income Spline Data
            List<SplineChartData> IncomeSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                })
                .ToList();

            //Expense Spline Data
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                })
                .ToList();

            List<SplineChartData> InvestmentSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Investments")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    investment = k.Sum(l => l.Amount)
                })
                .ToList();

            //Combining SplineData
            string[] LastDays = Enumerable.Range(0, DaysInMonth)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from day in LastDays
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      join investment in InvestmentSummary on day equals investment.day into investmentJoined
                                      from investment in investmentJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income==null ? 0 : income.income,
                                          expense = expense==null ? 0 : expense.expense,
                                          investment = investment == null ? 0 : investment.investment,
                                      };


            //Recent transactions
            ViewBag.RecentTransactions = await _context.Transactions
                .Include(i => i.Category)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }

    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense;
        public int investment;
    }
}
