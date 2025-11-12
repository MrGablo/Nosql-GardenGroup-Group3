using DocumentFormat.OpenXml.Wordprocessing;
using GardenGroupIncidentSystem.Models;
using GardenGroupIncidentSystem.Services;
using GardenGroupIncidentSystem.Services.Sorting;
using Microsoft.AspNetCore.Mvc;

namespace NoSQL_Project.Controllers
{
    public class TicketController : Controller
    {

        private readonly ITicketService _ticketService;
        private readonly ITicketSorter _ticketSorter;

        // Constructor with dependency injection for ticket service
        public TicketController(ITicketService ticketService, ITicketSorter ticketSorter)
        {
            _ticketService = ticketService;
            _ticketSorter = ticketSorter;
        }

        // Displays a list of all tickets
        public IActionResult Index(
       string q = "", string status = "all", string priority = "all", string type = "all",
       string sort = "priority", string dir = "desc")
        {
            try
            {


                var tickets = _ticketService.GetFilteredTickets(q, status, priority, type);

                // Sorting done in SEPARATE CLASS
                if (string.Equals(sort, "priority", StringComparison.OrdinalIgnoreCase))
                {
                    bool asc = string.Equals(dir, "asc", StringComparison.OrdinalIgnoreCase);
                    tickets = _ticketSorter.SortByPriority(tickets, asc).ToList();
                }

                // counts (keep your existing ones)
                ViewBag.TotalTickets = tickets.Count; // note: your view uses ViewBag.TotalTickets
                ViewBag.OpenCount = tickets.Count(t => t.TicketStatus == Status.Open);
                ViewBag.ResolvedCount = tickets.Count(t => t.TicketStatus == Status.Resolved);
                ViewBag.ClosedCount = tickets.Count(t => t.TicketStatus == Status.Closed);

                // keep filters and sort state in ViewBag
                ViewBag.Query = q;
                ViewBag.StatusFilter = status;
                ViewBag.PriorityFilter = priority;
                ViewBag.TypeFilter = type;
                ViewBag.Sort = sort;
                ViewBag.Dir = dir;

                return View(tickets);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading tickets: {ex.Message}";
                return View(new List<Ticket>());
            }
        }



        // Displays details for a specific ticket by ID
        [HttpGet]
        public IActionResult Details(string id)
        {
            var ticket = _ticketService.GetTicketById(id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }

        // Creates a new ticket
        [HttpPost]
        public IActionResult Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _ticketService.CreateTicket(ticket);
                TempData["Success"] = $"Ticket {ticket.Id} created successfully.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to create ticket.";
            return View(ticket);
        }


        [HttpGet]
        public IActionResult Edit(string id)
        {
            var ticket = _ticketService.GetTicketById(id);
            if (ticket == null)
            {
                TempData["Error"] = $"Ticket {id} not found.";
                return RedirectToAction("Index");
            }
            return View(ticket);
        }

        [HttpPost]
        public IActionResult Edit(string id, Ticket ticket, string HandoverTo, string HandoverReason)
        {
            try
            {
                _ticketService.UpdateTicketWithWorkflow(id, ticket, HandoverTo, HandoverReason);
                TempData["Success"] = $"Ticket {id} updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }


        // Deletes a ticket by ID
        [HttpPost]
        public IActionResult Delete(string id)
        {
            try
            {
                _ticketService.DeleteTicket(id);
                TempData["Success"] = $"Ticket {id} deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete ticket {id}: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}