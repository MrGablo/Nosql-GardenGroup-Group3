using GardenGroupIncidentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Services.Interfaces;

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
        public IActionResult Index()
        {
            var tickets = _ticketService.GetAllTickets();
            return View(tickets);
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
            _ticketService.CreateTicket(ticket);
            return RedirectToAction("Index");
        }

        // Updates an existing ticket by ID
        [HttpPost]
        public IActionResult Edit(string id, Ticket ticket)
        {
            _ticketService.UpdateTicket(id, ticket);
            return RedirectToAction("Index");
        }

        // Deletes a ticket by ID
        [HttpPost]
        public IActionResult Delete(string id)
        {
            _ticketService.DeleteTicket(id);
            return RedirectToAction("Index");
        }
    }
}
