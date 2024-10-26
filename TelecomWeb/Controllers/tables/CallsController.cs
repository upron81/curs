using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TelecomWeb.Models;

namespace TelecomWeb.Controllers.tables
{
    public class CallsController : Controller
    {
        private readonly TelecomDbContext _context;

        public CallsController(TelecomDbContext context)
        {
            _context = context;
        }

        // GET: Calls
        public async Task<IActionResult> Index(string dateSearch, string durationSearch, string phoneSearch, int pageNumber = 1)
        {
            ViewData["DateFilter"] = dateSearch;
            ViewData["DurationFilter"] = durationSearch;
            ViewData["PhoneFilter"] = phoneSearch;

            var calls = from c in _context.Calls.Include(c => c.Contract) select c;

            if (!string.IsNullOrEmpty(dateSearch) && DateTime.TryParse(dateSearch, out var date))
            {
                calls = calls.Where(c => c.CallDate.Date == date.Date);
            }

            if (!string.IsNullOrEmpty(durationSearch) && int.TryParse(durationSearch, out var duration))
            {
                calls = calls.Where(c => c.CallDuration == duration);
            }

            if (!string.IsNullOrEmpty(phoneSearch))
            {
                calls = calls.Where(c => c.Contract.PhoneNumber.Contains(phoneSearch));
            }

            int pageSize = 30;
            var totalCalls = await calls.CountAsync();
            var paginatedList = await calls
                .OrderBy(c => c.CallId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PaginatedList<Call>(paginatedList, totalCalls, pageNumber, pageSize);

            return View(viewModel);
        }

        // GET: Calls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var call = await _context.Calls
                .Include(c => c.Contract)
                .FirstOrDefaultAsync(m => m.CallId == id);
            if (call == null)
            {
                return NotFound();
            }

            return View(call);
        }

        // GET: Calls/Create
        public IActionResult Create()
        {
            ViewData["ContractName"] = new SelectList(_context.Contracts, "ContractId", "PhoneNumber");
            return View();
        }

        // POST: Calls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CallId,ContractId,CallDate,CallDuration")] Call call)
        {
            if (ModelState.IsValid)
            {
                _context.Add(call);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractName"] = new SelectList(_context.Contracts, "ContractId", "PhoneNumber");
            return View(call);
        }

        // GET: Calls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var call = await _context.Calls.FindAsync(id);
            if (call == null)
            {
                return NotFound();
            }
            ViewData["ContractName"] = new SelectList(_context.Contracts, "ContractId", "PhoneNumber");
            return View(call);
        }

        // POST: Calls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CallId,ContractId,CallDate,CallDuration")] Call call)
        {
            if (id != call.CallId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(call);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CallExists(call.CallId))
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
            ViewData["ContractName"] = new SelectList(_context.Contracts, "ContractId", "PhoneNumber");
            return View(call);
        }

        // GET: Calls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var call = await _context.Calls
                .Include(c => c.Contract)
                .FirstOrDefaultAsync(m => m.CallId == id);
            if (call == null)
            {
                return NotFound();
            }

            return View(call);
        }

        // POST: Calls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var call = await _context.Calls.FindAsync(id);
            if (call != null)
            {
                _context.Calls.Remove(call);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CallExists(int id)
        {
            return _context.Calls.Any(e => e.CallId == id);
        }
    }
}
