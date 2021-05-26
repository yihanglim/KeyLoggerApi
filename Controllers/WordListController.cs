using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KeyLoggerApi.Models;
using ClosedXML.Excel;
using System.IO;

namespace KeyLoggerApi.Controllers
{
    public class WordListController : Controller
    {
        private readonly KeyloggerContext _context;

        public WordListController(KeyloggerContext context)
        {
            _context = context;
        }

        // GET: WordList
        public async Task<IActionResult> Index()
        {
            return View(await _context.WordLists.ToListAsync());
        }

        // GET: WordList/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wordList = await _context.WordLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wordList == null)
            {
                return NotFound();
            }

            return View(wordList);
        }

        // GET: WordList/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WordList/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,CreationDate")] WordList wordList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wordList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wordList);
        }

        // GET: WordList/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wordList = await _context.WordLists.FindAsync(id);
            if (wordList == null)
            {
                return NotFound();
            }
            return View(wordList);
        }

        // POST: WordList/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,CreationDate")] WordList wordList)
        {
            if (id != wordList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wordList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WordListExists(wordList.Id))
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
            return View(wordList);
        }

        // GET: WordList/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wordList = await _context.WordLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wordList == null)
            {
                return NotFound();
            }

            return View(wordList);
        }

        // POST: WordList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wordList = await _context.WordLists.FindAsync(id);
            _context.WordLists.Remove(wordList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PrintAll()
        {
            try
            {
                List<WordList> allData = new List<WordList>();

                allData = await _context.WordLists.ToListAsync();

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Word_Lists_" + DateTime.Now + ".xlsx";

                var workbook = new XLWorkbook();
                IXLWorksheet worksheet = workbook.Worksheets.Add("Word_List");
                worksheet.Cell(1, 1).Value = "Description";
                worksheet.Cell(1, 2).Value = "Date";
                worksheet.Cell(1, 3).Value = "Time";

                for (int index = 1; index <= allData.Count; index++)
                {
                    worksheet.Cell(index + 1, 1).Value = allData[index - 1].Description;
                    worksheet.Cell(index + 1, 2).Value = allData[index - 1].CreationDate.ToString("MM/dd/yyyy");
                    worksheet.Cell(index + 1, 3).Value = allData[index - 1].CreationDate.ToString("HH:mm:ss");
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        private IActionResult Error()
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> DeleteAll()
        {
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [WordLists]");
            return RedirectToAction("Index");

        }

        private bool WordListExists(int id)
        {
            return _context.WordLists.Any(e => e.Id == id);
        }
    }
}
