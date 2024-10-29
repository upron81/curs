using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> Index(string nameSearch, string addressSearch, string passportSearch, string tariffSearch, int pageNumber = 1)
        {
            ViewData["NameFilter"] = nameSearch;
            ViewData["AddressFilter"] = addressSearch;
            ViewData["PassportFilter"] = passportSearch;
            ViewData["TariffFilter"] = tariffSearch;

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

            int pageSize = 30;
            var totalSubscribers = await subscribers.CountAsync();
            var paginatedList = await subscribers
                .OrderBy(s => s.SubscriberId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PaginatedList<Subscriber>(paginatedList, totalSubscribers, pageNumber, pageSize);

            return View(viewModel);
        }

        // GET: Subscribers/Details/5
        public async Task<IActionResult> Details(int? id)
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

        public async Task<IActionResult> Statistic()
        {
            var currentYear = DateTime.Now.Year;

            var subscriberCount = _context.Subscribers
            .Where(s => s.Contracts.All(c => c.ContractDate.Year == currentYear) &&
                        !s.Contracts.Any(c => c.ContractDate.Year < currentYear))
            .Count();

            return View(subscriberCount);
        }
    }
}
