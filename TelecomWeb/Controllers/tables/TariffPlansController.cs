using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelecomWeb.Models;

namespace TelecomWeb.Controllers.tables
{
    [Authorize]
    public class TariffPlansController : Controller
    {
        private readonly TelecomDbContext _context;

        public TariffPlansController(TelecomDbContext context)
        {
            _context = context;
        }

        // GET: TariffPlans
        public async Task<IActionResult> Index(
            int pageNumber = 1,
            string searchTariffName = "",
            string searchSubscriptionFee = "",
            string searchLocalCallRate = "",
            string searchLongDistanceCallRate = "",
            string searchInternationalCallRate = "",
            string sortOrder = "",
            string currentSort = "")
        {
            int pageSize = 30;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["TariffNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "tariffName_desc" : "";
            ViewData["SubscriptionFeeSortParm"] = sortOrder == "subscriptionFee" ? "subscriptionFee_desc" : "subscriptionFee";
            ViewData["LocalCallRateSortParm"] = sortOrder == "localCallRate" ? "localCallRate_desc" : "localCallRate";

            var telecomDbContext = _context.TariffPlans.AsQueryable();

            if (!string.IsNullOrEmpty(searchTariffName))
            {
                telecomDbContext = telecomDbContext.Where(t => t.TariffName.Contains(searchTariffName));
            }

            if (decimal.TryParse(searchSubscriptionFee, out decimal subscriptionFee))
            {
                telecomDbContext = telecomDbContext.Where(t => t.SubscriptionFee == subscriptionFee);
            }

            if (decimal.TryParse(searchLocalCallRate, out decimal localCallRate))
            {
                telecomDbContext = telecomDbContext.Where(t => t.LocalCallRate == localCallRate);
            }

            if (decimal.TryParse(searchLongDistanceCallRate, out decimal longDistanceCallRate))
            {
                telecomDbContext = telecomDbContext.Where(t => t.LongDistanceCallRate == longDistanceCallRate);
            }

            if (decimal.TryParse(searchInternationalCallRate, out decimal internationalCallRate))
            {
                telecomDbContext = telecomDbContext.Where(t => t.InternationalCallRate == internationalCallRate);
            }

            telecomDbContext = sortOrder switch
            {
                "tariffName_desc" => telecomDbContext.OrderByDescending(t => t.TariffName),
                "subscriptionFee" => telecomDbContext.OrderBy(t => t.SubscriptionFee),
                "subscriptionFee_desc" => telecomDbContext.OrderByDescending(t => t.SubscriptionFee),
                "localCallRate" => telecomDbContext.OrderBy(t => t.LocalCallRate),
                "localCallRate_desc" => telecomDbContext.OrderByDescending(t => t.LocalCallRate),
                _ => telecomDbContext.OrderBy(t => t.TariffName)
            };

            var totalTariffPlans = await telecomDbContext.CountAsync();

            var tariffPlans = await telecomDbContext
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PaginatedList<TariffPlan>(tariffPlans, totalTariffPlans, pageNumber, pageSize);

            ViewData["searchTariffName"] = searchTariffName;
            ViewData["searchSubscriptionFee"] = searchSubscriptionFee;
            ViewData["searchLocalCallRate"] = searchLocalCallRate;
            ViewData["searchLongDistanceCallRate"] = searchLongDistanceCallRate;
            ViewData["searchInternationalCallRate"] = searchInternationalCallRate;

            return View(viewModel);
        }

        // GET: TariffPlans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tariffPlan = await _context.TariffPlans
                .FirstOrDefaultAsync(m => m.TariffPlanId == id);
            if (tariffPlan == null)
            {
                return NotFound();
            }

            return View(tariffPlan);
        }

        // GET: TariffPlans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TariffPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TariffPlanId,TariffName,SubscriptionFee,LocalCallRate,LongDistanceCallRate,InternationalCallRate,IsPerSecond,SmsRate,MmsRate,DataRatePerMb")] TariffPlan tariffPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tariffPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tariffPlan);
        }

        // GET: TariffPlans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tariffPlan = await _context.TariffPlans.FindAsync(id);
            if (tariffPlan == null)
            {
                return NotFound();
            }
            return View(tariffPlan);
        }

        // POST: TariffPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TariffPlanId,TariffName,SubscriptionFee,LocalCallRate,LongDistanceCallRate,InternationalCallRate,IsPerSecond,SmsRate,MmsRate,DataRatePerMb")] TariffPlan tariffPlan)
        {
            if (id != tariffPlan.TariffPlanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tariffPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TariffPlanExists(tariffPlan.TariffPlanId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tariffPlan);
        }

        // GET: TariffPlans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tariffPlan = await _context.TariffPlans
                .FirstOrDefaultAsync(m => m.TariffPlanId == id);
            if (tariffPlan == null)
            {
                return NotFound();
            }

            return View(tariffPlan);
        }

        // POST: TariffPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tariffPlan = await _context.TariffPlans.FindAsync(id);
            if (tariffPlan != null)
            {
                _context.TariffPlans.Remove(tariffPlan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TariffPlanExists(int id)
        {
            return _context.TariffPlans.Any(e => e.TariffPlanId == id);
        }
    }
}
