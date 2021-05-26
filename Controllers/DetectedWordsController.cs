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

        public async Task<IActionResult> WordsByDateTime(DateTime date, int time)
        {
            List<DetectedWord> dataByDate = new List<DetectedWord>();
            List<DetectedWord> dataByTime = new List<DetectedWord>();
            List<KeyData> result = new List<KeyData>();

            dataByDate = await _context.DetectedWords.Where(x => x.CreationDate.Date == date.Date).ToListAsync();

            switch (time)
            {
                case 1:
                    dataByTime = dataByDate.Where(x => x.CreationDate.Hour > 0 && x.CreationDate.Hour <= 6).ToList();
                    break;
                case 2:
                    dataByTime = dataByDate.Where(x => x.CreationDate.Hour > 6 && x.CreationDate.Hour <= 12).ToList();
                    break;
                case 3:
                    dataByTime = dataByDate.Where(x => x.CreationDate.Hour > 12 && x.CreationDate.Hour <= 18).ToList();
                    break;
                case 4:
                    dataByTime = dataByDate.Where(x => x.CreationDate.Hour > 18 && x.CreationDate.Hour <= 24).ToList();

                    break;
            }
            if (dataByTime.Count > 0)
            {
                foreach (var data in dataByTime)
                {
                    KeyData resultModel = new KeyData();
                    resultModel.CreationDate = data.CreationDate.ToString("dd/MM/yyyy hh:mm:ss tt");
                    resultModel.Id = data.Id;
                    resultModel.Keystroke = data.Description;
                    result.Add(resultModel);
                }
            }

            return Json(result);
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

        public async Task<IActionResult> PrintAll()
        {
            try
            {
                List<DetectedWord> allData = new List<DetectedWord>();

                allData = await _context.DetectedWords.ToListAsync();

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Detected_Words_" + DateTime.Now + ".xlsx";

                var workbook = new XLWorkbook();
                IXLWorksheet worksheet = workbook.Worksheets.Add("Detected_Words");
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
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [DetectedWords]");
            return RedirectToAction("Index");
        }

        private bool DetectedWordExists(int id)
        {
            return _context.DetectedWords.Any(e => e.Id == id);
        }
    }
}
