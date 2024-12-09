using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelecomWeb.Models;

namespace TelecomWeb.Controllers.tables
{
    [Authorize]
    public class SubscribersController : Controller
    {
        private readonly TelecomDbContext _context;

        public SubscribersController(TelecomDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string nameSearch,
            string addressSearch,
            string passportSearch,
            string tariffSearch,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber = 1,
            string sortOrder = "",
            string currentSort = "")
        {
            ViewBag.Tariffs = await _context.TariffPlans
                                             .Select(t => t.TariffName)
                                             .Distinct()
                                             .ToListAsync();

            ViewData["NameFilter"] = nameSearch;
            ViewData["AddressFilter"] = addressSearch;
            ViewData["PassportFilter"] = passportSearch;
            ViewData["TariffFilter"] = tariffSearch;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AddressSortParm"] = sortOrder == "address" ? "address_desc" : "address";
            ViewData["PassportSortParm"] = sortOrder == "passport" ? "passport_desc" : "passport";
            ViewData["TariffSortParm"] = sortOrder == "tariff" ? "tariff_desc" : "tariff";

            var subscribers = from s in _context.Subscribers
                              .Include(s => s.Contracts)
                              .ThenInclude(c => c.TariffPlan)
                              select s;

            if (!string.IsNullOrEmpty(nameSearch))
            {
                subscribers = subscribers.Where(s => s.FullName.Contains(nameSearch));
            }

            if (!string.IsNullOrEmpty(addressSearch))
            {
                subscribers = subscribers.Where(s => s.HomeAddress.Contains(addressSearch));
            }

            if (!string.IsNullOrEmpty(passportSearch))
            {
                subscribers = subscribers.Where(s => s.PassportData.Contains(passportSearch));
            }

            if (!string.IsNullOrEmpty(tariffSearch))
            {
                subscribers = subscribers.Where(s => s.Contracts.Any(c => c.TariffPlan.TariffName.Contains(tariffSearch)));
            }

            DateOnly? startDateOnly = startDate.HasValue ? DateOnly.FromDateTime(startDate.Value) : null;
            DateOnly? endDateOnly = endDate.HasValue ? DateOnly.FromDateTime(endDate.Value) : null;

            if (startDateOnly.HasValue && endDateOnly.HasValue)
            {
                subscribers = subscribers.Where(s =>
                    s.Contracts.Any() &&
                    s.Contracts.All(c => c.ContractDate >= startDateOnly && c.ContractDate <= endDateOnly));
            }

            subscribers = sortOrder switch
            {
                "name_desc" => subscribers.OrderByDescending(s => s.FullName),
                "address" => subscribers.OrderBy(s => s.HomeAddress),
                "address_desc" => subscribers.OrderByDescending(s => s.HomeAddress),
                "passport" => subscribers.OrderBy(s => s.PassportData),
                "passport_desc" => subscribers.OrderByDescending(s => s.PassportData),
                _ => subscribers.OrderBy(s => s.FullName),
            };

            int pageSize = 30;
            var totalSubscribers = await subscribers.CountAsync();
            var paginatedList = await subscribers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PaginatedList<Subscriber>(paginatedList, totalSubscribers, pageNumber, pageSize);

            return View(viewModel);
        }

        // GET: Subscribers/Details/5
        public async Task<IActionResult> Details(int? id, DateTime? startDate, DateTime? endDate)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriber = await _context.Subscribers
                .Include(s => s.Contracts)
                    .ThenInclude(c => c.Calls)
                .Include(s => s.Contracts)
                    .ThenInclude(c => c.Messages)
                .Include(s => s.Contracts)
                    .ThenInclude(c => c.InternetUsages)
                .Include(s => s.Contracts)
                    .ThenInclude(c => c.TariffPlan)
                .FirstOrDefaultAsync(m => m.SubscriberId == id);

            if (subscriber == null)
            {
                return NotFound();
            }

            decimal grandTotalCost = 0;

            if (startDate.HasValue && endDate.HasValue)
            {
                foreach (var contract in subscriber.Contracts)
                {
                    var tariff = contract.TariffPlan;
                    if (tariff != null)
                    {
                        decimal totalCallCost = contract.Calls
                            .Where(call => call.CallDate >= startDate && call.CallDate <= endDate)
                            .Sum(call => call.CallDuration * (tariff.IsPerSecond ? tariff.LocalCallRate / 60 : tariff.LocalCallRate));

                        decimal totalSmsCost = contract.Messages
                            .Where(m => m.MessageDate >= startDate && m.MessageDate <= endDate)
                            .Count(m => !m.IsMms) * tariff.SmsRate;

                        decimal totalMmsCost = contract.Messages
                            .Where(m => m.MessageDate >= startDate && m.MessageDate <= endDate)
                            .Count(m => m.IsMms) * tariff.MmsRate;

                        decimal totalInternetCost = contract.InternetUsages
                            .Where(u => u.UsageDate >= startDate && u.UsageDate <= endDate)
                            .Sum(u => (u.DataSentMb + u.DataReceivedMb) * tariff.DataRatePerMb);

                        grandTotalCost += totalCallCost + totalSmsCost + totalMmsCost + totalInternetCost;
                    }
                }
            }

            ViewBag.GrandTotalCost = grandTotalCost;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(subscriber);
        }

        // GET: Subscribers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subscribers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubscriberId,FullName,HomeAddress,PassportData")] Subscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subscriber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subscriber);
        }

        // GET: Subscribers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriber = await _context.Subscribers.FindAsync(id);
            if (subscriber == null)
            {
                return NotFound();
            }
            return View(subscriber);
        }

        // POST: Subscribers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubscriberId,FullName,HomeAddress,PassportData")] Subscriber subscriber)
        {
            if (id != subscriber.SubscriberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscriber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriberExists(subscriber.SubscriberId))
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
            return View(subscriber);
        }

        // GET: Subscribers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriber = await _context.Subscribers
                .FirstOrDefaultAsync(m => m.SubscriberId == id);
            if (subscriber == null)
            {
                return NotFound();
            }

            return View(subscriber);
        }

        // POST: Subscribers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscriber = await _context.Subscribers.FindAsync(id);
            if (subscriber != null)
            {
                _context.Subscribers.Remove(subscriber);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriberExists(int id)
        {
            return _context.Subscribers.Any(e => e.SubscriberId == id);
        }
    }
}
