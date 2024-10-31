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
    public class InternetUsagesController : Controller
    {
        private readonly TelecomDbContext _context;

        public InternetUsagesController(TelecomDbContext context)
        {
            _context = context;
        }

        // GET: InternetUsages
        public async Task<IActionResult> Index(DateTime? dateSearch, decimal? dataSentSearch, decimal? dataReceivedSearch, string phoneSearch, int pageNumber = 1)
        {
            ViewData["DateFilter"] = dateSearch?.ToString("yyyy-MM-dd");
            ViewData["DataSentFilter"] = dataSentSearch;
            ViewData["DataReceivedFilter"] = dataReceivedSearch;
            ViewData["PhoneFilter"] = phoneSearch;

            var internetUsages = _context.InternetUsages.Include(i => i.Contract).AsQueryable();

            if (dateSearch.HasValue)
            {
                internetUsages = internetUsages.Where(i => i.UsageDate.Date == dateSearch.Value.Date);
            }

            if (dataSentSearch.HasValue)
            {
                internetUsages = internetUsages.Where(i => i.DataSentMb >= dataSentSearch.Value);
            }

            if (dataReceivedSearch.HasValue)
            {
                internetUsages = internetUsages.Where(i => i.DataReceivedMb >= dataReceivedSearch.Value);
            }

            if (!string.IsNullOrEmpty(phoneSearch))
            {
                internetUsages = internetUsages.Where(i => i.Contract.PhoneNumber.Contains(phoneSearch));
            }

            int pageSize = 30;
            var totalItems = await internetUsages.CountAsync();
            var paginatedList = await internetUsages
                .OrderBy(i => i.UsageDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PaginatedList<InternetUsage>(paginatedList, totalItems, pageNumber, pageSize);

            return View(viewModel);
        }


        // GET: InternetUsages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var internetUsage = await _context.InternetUsages
                .Include(i => i.Contract)
                .FirstOrDefaultAsync(m => m.UsageId == id);
            if (internetUsage == null)
            {
                return NotFound();
            }

            return View(internetUsage);
        }

        // GET: InternetUsages/Create
        public IActionResult Create()
        {
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "PhoneNumber");
            return View();
        }

        // POST: InternetUsages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsageId,ContractId,UsageDate,DataSentMb,DataReceivedMb")] InternetUsage internetUsage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(internetUsage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "PhoneNumber");
            return View(internetUsage);
        }

        // GET: InternetUsages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var internetUsage = await _context.InternetUsages.FindAsync(id);
            if (internetUsage == null)
            {
                return NotFound();
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "PhoneNumber");
            return View(internetUsage);
        }

        // POST: InternetUsages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsageId,ContractId,UsageDate,DataSentMb,DataReceivedMb")] InternetUsage internetUsage)
        {
            if (id != internetUsage.UsageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(internetUsage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InternetUsageExists(internetUsage.UsageId))
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
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "PhoneNumber");
            return View(internetUsage);
        }

        // GET: InternetUsages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var internetUsage = await _context.InternetUsages
                .Include(i => i.Contract)
                .FirstOrDefaultAsync(m => m.UsageId == id);
            if (internetUsage == null)
            {
                return NotFound();
            }

            return View(internetUsage);
        }

        // POST: InternetUsages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var internetUsage = await _context.InternetUsages.FindAsync(id);
            if (internetUsage != null)
            {
                _context.InternetUsages.Remove(internetUsage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InternetUsageExists(int id)
        {
            return _context.InternetUsages.Any(e => e.UsageId == id);
        }
    }
}
