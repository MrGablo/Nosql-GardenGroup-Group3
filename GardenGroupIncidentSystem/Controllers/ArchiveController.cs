using Microsoft.AspNetCore.Mvc;
using GardenGroupIncidentSystem.Services;
using System;

namespace GardenGroupIncidentSystem.Controllers
{
    /// <summary>
    /// Archive Controller
    /// Handles all archive-related actions
    /// Uses TicketArchivingService (Individual Functionality)
    /// DOES NOT MODIFY EXISTING TicketController
    /// </summary>
    public class ArchiveController : Controller
    {
        private readonly TicketArchivingService _archiveService;

        /// <summary>
        /// Constructor - Dependency Injection
        /// </summary>
        public ArchiveController(TicketArchivingService archiveService)
        {
            _archiveService = archiveService;
        }

        /// <summary>
        /// GET: /Archive/Index
        /// Main archive management page with statistics and archive button
        /// </summary>
        public IActionResult Index()
        {
            // Get archiving statistics
            var stats = _archiveService.GetArchivingStatistics();
            ViewBag.Statistics = stats;

            // Get count of tickets that can be archived (older than 2 years)
            var twoYearsAgo = DateTime.Now.AddYears(-2);
            ViewBag.ArchivableCount = _archiveService.GetArchivableTicketsCount(twoYearsAgo);

            return View();
        }

        /// <summary>
        /// POST: /Archive/ArchiveOldTickets
        /// MAIN ARCHIVING ACTION - Triggered by button click
        /// Archives all tickets older than 2 years
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ArchiveOldTickets()
        {
            try
            {
                // Call the archiving service
                int archivedCount = _archiveService.ArchiveOldTickets();

                if (archivedCount > 0)
                {
                    TempData["Success"] = $"Successfully archived {archivedCount} ticket(s) older than 2 years.";
                }
                else
                {
                    TempData["Info"] = "No tickets found to archive.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error occurred during archiving: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// POST: /Archive/ArchiveByDate
        /// Archive tickets by custom date
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ArchiveByDate(DateTime cutoffDate)
        {
            // Validate the date
            var validation = _archiveService.ValidateArchiving(cutoffDate);
            if (!validation.IsValid)
            {
                TempData["Error"] = validation.Message;
                return RedirectToAction("Index");
            }

            try
            {
                int archivedCount = _archiveService.ArchiveTicketsByDate(cutoffDate);
                TempData["Success"] = $"Successfully archived {archivedCount} ticket(s) before {cutoffDate:yyyy-MM-dd}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error during archiving: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: /Archive/ViewArchived
        /// View all archived tickets
        /// </summary>
        public IActionResult ViewArchived()
        {
            var archivedTickets = _archiveService.GetArchivedTickets();
            return View(archivedTickets);
        }

        /// <summary>
        /// GET: /Archive/SearchArchived
        /// Search through archived tickets
        /// </summary>
        public IActionResult SearchArchived(string searchTerm)
        {
            var results = _archiveService.SearchArchivedTickets(searchTerm);
            ViewBag.SearchTerm = searchTerm;
            return View("ViewArchived", results);
        }

        /// <summary>
        /// POST: /Archive/Restore/{id}
        /// Restore archived ticket back to active tickets
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restore(string id)
        {
            try
            {
                bool restored = _archiveService.RestoreArchivedTicket(id);
                if (restored)
                {
                    TempData["Success"] = "Ticket restored successfully.";
                    return Json(new { success = true, message = "Ticket restored" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to restore ticket" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// GET: /Archive/Preview
        /// Preview tickets that will be archived
        /// </summary>
        public IActionResult Preview(int years = 2)
        {
            DateTime cutoffDate = DateTime.Now.AddYears(-years);
            var previewTickets = _archiveService.PreviewArchivableTickets(cutoffDate);

            ViewBag.CutoffDate = cutoffDate;
            ViewBag.Years = years;

            return View(previewTickets);
        }

        /// <summary>
        /// GET: /Archive/Statistics
        /// View detailed archiving statistics
        /// </summary>
        public IActionResult Statistics()
        {
            var stats = _archiveService.GetArchivingStatistics();
            var yearlyStats = _archiveService.GetArchiveSummaryByYear();

            ViewBag.YearlyStatistics = yearlyStats;

            return View(stats);
        }
    }
}