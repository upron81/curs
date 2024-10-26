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
    public class MessagesController : Controller
    {
        private readonly TelecomDbContext _context;

        public MessagesController(TelecomDbContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index(string dateSearch, string phoneNumberSearch, string isMmsSearch, int pageNumber = 1)
        {
            ViewData["DateFilter"] = dateSearch;
            ViewData["PhoneNumberFilter"] = phoneNumberSearch;
            ViewData["IsMmsFilter"] = isMmsSearch;

            var messages = from m in _context.Messages.Include(m => m.Contract) select m;

            if (!string.IsNullOrEmpty(dateSearch) && DateTime.TryParse(dateSearch, out var date))
            {
                messages = messages.Where(c => c.MessageDate.Date == date.Date);
            }

            if (!string.IsNullOrEmpty(phoneNumberSearch))
            {
                messages = messages.Where(m => m.Contract.PhoneNumber.Contains(phoneNumberSearch));
            }

            if (isMmsSearch == "true")
            {
                messages = messages.Where(m => m.IsMms);
            }
            else if (isMmsSearch == "false")
            {
                messages = messages.Where(m => !m.IsMms);
            }

            int pageSize = 30;
            var totalMessages = await messages.CountAsync();
            var paginatedList = await messages
                .OrderBy(m => m.MessageDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PaginatedList<Message>(paginatedList, totalMessages, pageNumber, pageSize);

            return View(viewModel);
        }


        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Contract)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["ContractName"] = new SelectList(_context.Contracts, "ContractId", "PhoneNumber");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,ContractId,MessageDate,IsMms")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractName"] = new SelectList(_context.Contracts, "ContractId", "PhoneNumber");
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["ContractName"] = new SelectList(_context.Contracts, "ContractId", "PhoneNumber");
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,ContractId,MessageDate,IsMms")] Message message)
        {
            if (id != message.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageId))
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
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Contract)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
