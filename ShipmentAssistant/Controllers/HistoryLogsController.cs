using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipmentAssistant.Models;

namespace ShipmentAssistant.Controllers
{
    [Authorize]
    public class HistoryLogsController : Controller
    {
        private readonly HistoryContext _context;

        public HistoryLogsController(HistoryContext context)
        {
            _context = context;
        }

        // GET: HistoryLogs
        public async Task<IActionResult> Index()
        {
              return _context.HistoryLogs != null ? 
                          View(await _context.HistoryLogs.OrderByDescending(o=>o.Date).ToListAsync()) :
                          Problem("Entity set 'HistoryContext.HistoryLogs'  is null.");
        }

        // GET: HistoryLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.HistoryLogs == null)
            {
                return NotFound();
            }

            var historyLog = await _context.HistoryLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historyLog == null)
            {
                return NotFound();
            }

            return View(historyLog);
        }

        // GET: HistoryLogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HistoryLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,User,Code,UnitsRecived")] HistoryLog historyLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historyLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(historyLog);
        }

        // GET: HistoryLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.HistoryLogs == null)
            {
                return NotFound();
            }

            var historyLog = await _context.HistoryLogs.FindAsync(id);
            if (historyLog == null)
            {
                return NotFound();
            }
            return View(historyLog);
        }

        // POST: HistoryLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,User,Code,UnitsRecived")] HistoryLog historyLog)
        {
            if (id != historyLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historyLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoryLogExists(historyLog.Id))
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
            return View(historyLog);
        }

        // GET: HistoryLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.HistoryLogs == null)
            {
                return NotFound();
            }

            var historyLog = await _context.HistoryLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historyLog == null)
            {
                return NotFound();
            }

            return View(historyLog);
        }

        // POST: HistoryLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.HistoryLogs == null)
            {
                return Problem("Entity set 'HistoryContext.HistoryLogs'  is null.");
            }
            var historyLog = await _context.HistoryLogs.FindAsync(id);
            if (historyLog != null)
            {
                _context.HistoryLogs.Remove(historyLog);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Clear()
        {
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [HistoryLogs]");
            return Redirect(nameof(Index));
        }

        private bool HistoryLogExists(int id)
        {
          return (_context.HistoryLogs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
