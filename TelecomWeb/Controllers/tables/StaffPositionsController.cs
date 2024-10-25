using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TelecomWeb.Models;

namespace TelecomWeb.Controllers.tables
{
    [Authorize]
    public class StaffPositionsController : Controller
    {
        private readonly TelecomDbContext _context;

        public StaffPositionsController(TelecomDbContext context)
        {
            _context = context;
        }

        // GET: StaffPositions
        public async Task<IActionResult> Index()
        {
            return View(await _context.StaffPositions.ToListAsync());
        }

        // GET: StaffPositions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffPosition = await _context.StaffPositions
                .FirstOrDefaultAsync(m => m.PositionId == id);
            if (staffPosition == null)
            {
                return NotFound();
            }

            return View(staffPosition);
        }

        // GET: StaffPositions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StaffPositions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PositionId,PositionName")] StaffPosition staffPosition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staffPosition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(staffPosition);
        }

        // GET: StaffPositions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffPosition = await _context.StaffPositions.FindAsync(id);
            if (staffPosition == null)
            {
                return NotFound();
            }
            return View(staffPosition);
        }

        // POST: StaffPositions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PositionId,PositionName")] StaffPosition staffPosition)
        {
            if (id != staffPosition.PositionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staffPosition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffPositionExists(staffPosition.PositionId))
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
            return View(staffPosition);
        }

        // GET: StaffPositions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffPosition = await _context.StaffPositions
                .FirstOrDefaultAsync(m => m.PositionId == id);
            if (staffPosition == null)
            {
                return NotFound();
            }

            return View(staffPosition);
        }

        // POST: StaffPositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staffPosition = await _context.StaffPositions.FindAsync(id);
            if (staffPosition != null)
            {
                _context.StaffPositions.Remove(staffPosition);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffPositionExists(int id)
        {
            return _context.StaffPositions.Any(e => e.PositionId == id);
        }
    }
}
