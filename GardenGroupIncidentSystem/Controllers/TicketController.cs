using GardenGroupIncidentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using GardenGroupIncidentSystem.Services;

namespace NoSQL_Project.Controllers
{
    public class TicketController : Controller
    {

        private readonly ITicketService _ticketService;

        // Constructor with dependency injection for ticket service
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // Displays a list of all tickets
        public IActionResult Index(string q = "", string status = "all", string priority = "all", string type = "all")
        {
            try
            {
                var tickets = _ticketService.GetFilteredTickets(q, status, priority, type);

                ViewBag.TotalCount = tickets.Count;
                ViewBag.OpenCount = tickets.Count(t => t.TicketStatus == Status.Open);
                ViewBag.ResolvedCount = tickets.Count(t => t.TicketStatus == Status.Resolved);
                ViewBag.ClosedCount = tickets.Count(t => t.TicketStatus == Status.Closed);

                // Keep filters active in view
                ViewBag.Query = q;
                ViewBag.StatusFilter = status;
                ViewBag.PriorityFilter = priority;
                ViewBag.TypeFilter = type;

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

        // Updates an existing ticket by ID
        [HttpPost]
        public IActionResult Edit(string id, Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _ticketService.UpdateTicket(id, ticket);
                TempData["Success"] = $"Ticket {id} updated successfully.";
            }
            else
            {
                TempData["Error"] = $"Failed to update ticket {id}.";
            }

            return RedirectToAction("Index");
        }

        // Deletes a ticket by ID
        [HttpPost]
        public IActionResult Delete(string id)
        {
            _ticketService.DeleteTicket(id);
            TempData["Success"] = $"Ticket {id} deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}