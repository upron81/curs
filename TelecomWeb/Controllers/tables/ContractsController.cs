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
    public class ContractsController : Controller
    {
        private readonly TelecomDbContext _context;

        public ContractsController(TelecomDbContext context)
        {
            _context = context;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(int pageNumber = 1, string searchSubscriber = "", string searchTariff = "", string searchStaff = "", string searchPhone = "")
        {
            int pageSize = 30;

            var telecomDbContext = _context.Contracts
                .Include(c => c.Staff)
                .Include(c => c.Subscriber)
                .Include(c => c.TariffPlan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchSubscriber))
            {
                telecomDbContext = telecomDbContext.Where(c => c.Subscriber.FullName.Contains(searchSubscriber));
            }

            if (!string.IsNullOrEmpty(searchTariff))
            {
                telecomDbContext = telecomDbContext.Where(c => c.TariffPlan.TariffName.Contains(searchTariff));
            }

            if (!string.IsNullOrEmpty(searchStaff))
            {
                telecomDbContext = telecomDbContext.Where(c => c.Staff.FullName.Contains(searchStaff));
            }

            if (!string.IsNullOrEmpty(searchPhone))
            {
                telecomDbContext = telecomDbContext.Where(c => c.PhoneNumber.Contains(searchPhone));
            }

            var totalContracts = await telecomDbContext.CountAsync();
            var contracts = await telecomDbContext
                .OrderBy(c => c.ContractId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PaginatedList<Contract>(contracts, totalContracts, pageNumber, pageSize);

            ViewData["searchSubscriber"] = searchSubscriber;
            ViewData["searchTariff"] = searchTariff;
            ViewData["searchStaff"] = searchStaff;
            ViewData["searchPhone"] = searchPhone;

            return View(viewModel);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Staff)
                .Include(c => c.Subscriber)
                .Include(c => c.TariffPlan)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["StaffId"] = new SelectList(_context.Staff, "StaffId", "FullName");
            ViewData["SubscriberId"] = new SelectList(_context.Subscribers, "SubscriberId", "FullName");
            ViewData["TariffPlanId"] = new SelectList(_context.TariffPlans, "TariffPlanId", "TariffName");
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,SubscriberId,TariffPlanId,ContractDate,ContractEndDate,PhoneNumber,StaffId")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StaffId"] = new SelectList(_context.Staff, "StaffId", "StaffId", contract.StaffId);
            ViewData["SubscriberId"] = new SelectList(_context.Subscribers, "SubscriberId", "SubscriberId", contract.SubscriberId);
            ViewData["TariffPlanId"] = new SelectList(_context.TariffPlans, "TariffPlanId", "TariffPlanId", contract.TariffPlanId);
            return View(contract);
        }
        
        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["StaffName"] = new SelectList(_context.Staff, "StaffId", "FullName", contract.StaffId);
            ViewData["SubscriberName"] = new SelectList(_context.Subscribers, "SubscriberId", "FullName", contract.SubscriberId);
            ViewData["TariffPlanName"] = new SelectList(_context.TariffPlans, "TariffPlanId", "TariffName", contract.TariffPlanId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,SubscriberId,TariffPlanId,ContractDate,ContractEndDate,PhoneNumber,StaffId")] Contract contract)
        {
            if (id != contract.ContractId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.ContractId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["StaffName"] = new SelectList(_context.Staff, "StaffId", "FullName", contract.StaffId);
            ViewData["SubscriberName"] = new SelectList(_context.Subscribers, "SubscriberId", "FullName", contract.SubscriberId);
            ViewData["TariffPlanName"] = new SelectList(_context.TariffPlans, "TariffPlanId", "TariffName", contract.TariffPlanId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Staff)
                .Include(c => c.Subscriber)
                .Include(c => c.TariffPlan)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }
    }
}
