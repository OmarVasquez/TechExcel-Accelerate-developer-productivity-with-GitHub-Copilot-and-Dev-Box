﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesTestSample.Data;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace RazorPagesTestSample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Message Message { get; set; }

        public IList<Message> Messages { get; private set; }

        [TempData]
        public string MessageAnalysisResult { get; set; }

        #region snippet1
        public async Task OnGetAsync()
        {
            Messages = await _db.GetMessagesAsync();
        }
        #endregion

        public async Task<IActionResult> OnPostAddMessageAsync()
        {
            if (!ModelState.IsValid)
            {
                Messages = await _db.GetMessagesAsync();

                return Page();
            }

            await _db.AddMessageAsync(Message);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAllMessagesAsync()
        {
            await _db.DeleteAllMessagesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteMessageAsync(int id)
        {
            await _db.DeleteMessageAsync(id);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAnalyzeMessagesAsync()
        {
            Messages = await _db.GetMessagesAsync();

            if (Messages.Count == 0)
            {
                MessageAnalysisResult = "There are no messages to analyze.";
            }
            else
            {
                var wordCount = Messages.Sum(message => message.Text.Split(' ').Length);
                var avgWordCount = Decimal.Divide(wordCount, Messages.Count);
                MessageAnalysisResult = $"The average message length is {avgWordCount:0.##} words.";
            }

            return RedirectToPage();
        }

        public static void WriteToDirectory(ZipArchiveEntry entry, string destDirectory)
        {
            string destFileName = Path.Combine(destDirectory, entry.FullName);
            string fullPath = Path.GetFullPath(destFileName);

            if (!fullPath.StartsWith(Path.GetFullPath(destDirectory), StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Entry is outside the target directory.");
            }

            entry.ExtractToFile(fullPath);
        }
    }
}
