using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KeyLoggerApi.Models;

namespace KeyLoggerApi.Controllers
{
    public class DetectedWordsController : Controller
    {
        private readonly KeyloggerContext _context;

        public DetectedWordsController(KeyloggerContext context)
        {
            _context = context;
        }

        // GET: DetectedWords
        public async Task<IActionResult> Index()
        {
            return View(await _context.DetectedWords.ToListAsync());
        }

        // GET: DetectedWords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detectedWord = await _context.DetectedWords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detectedWord == null)
            {
                return NotFound();
            }

            return View(detectedWord);
        }

        // GET: DetectedWords/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DetectedWords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,CreationDate")] DetectedWord detectedWord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detectedWord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(detectedWord);
        }

        // GET: DetectedWords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detectedWord = await _context.DetectedWords.FindAsync(id);
            if (detectedWord == null)
            {
                return NotFound();
            }
            return View(detectedWord);
        }

        // POST: DetectedWords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,CreationDate")] DetectedWord detectedWord)
        {
            if (id != detectedWord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detectedWord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetectedWordExists(detectedWord.Id))
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
            return View(detectedWord);
        }

        // GET: DetectedWords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detectedWord = await _context.DetectedWords
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detectedWord == null)
            {
                return NotFound();
            }

            return View(detectedWord);
        }

        // POST: DetectedWords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detectedWord = await _context.DetectedWords.FindAsync(id);
            _context.DetectedWords.Remove(detectedWord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAll()
        {
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [DetectedWords]");
            return RedirectToAction("Index");
        }

        private bool DetectedWordExists(int id)
        {
            return _context.DetectedWords.Any(e => e.Id == id);
        }
    }
}
