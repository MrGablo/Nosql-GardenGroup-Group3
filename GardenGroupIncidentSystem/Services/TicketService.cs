using GardenGroupIncidentSystem.Models;
using GardenGroupIncidentSystem.Services.Repositories;
using NoSQL_Project.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace GardenGroupIncidentSystem.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        // CREATE

        public Ticket CreateTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            // Example business logic: ensure status defaults to Open
            if (ticket.TicketStatus == 0)
                ticket.TicketStatus = Status.Open;

            return _ticketRepository.CreateTicket(ticket);
        }

        public void CreateTickets(List<Ticket> tickets)
        {
            if (tickets == null || tickets.Count == 0)
                throw new ArgumentException("Ticket list cannot be empty.");

            foreach (var t in tickets)
            {
                if (t.TicketStatus == 0)
                    t.TicketStatus = Status.Open;
            }

            _ticketRepository.CreateTickets(tickets);
        }

        public List<Ticket> GetAllTickets() =>
            _ticketRepository.GetAllTickets();

        public Ticket GetTicketById(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
                return null;

            return _ticketRepository.GetTicketById(ticketId);
        }

        public List<Ticket> GetTicketsByStatus(Status status) =>
            _ticketRepository.GetTicketsByStatus(status);

        public List<Ticket> GetTicketsByEmployee(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
                return new List<Ticket>();

            return _ticketRepository.GetTicketsByEmployee(employeeId);
        }

        public void UpdateTicket(string ticketId, Ticket updatedTicket)
        {
            if (string.IsNullOrEmpty(ticketId) || updatedTicket == null)
                return;

            // block updates on closed tickets
            var existing = _ticketRepository.GetTicketById(ticketId);
            if (existing == null) return;

            _ticketRepository.UpdateTicket(ticketId, updatedTicket);
        }

        public void DeleteTicket(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
                return;

            _ticketRepository.DeleteTicket(ticketId);
        }

        public Dictionary<Status, int> GetTicketCountByStatus() =>
            _ticketRepository.GetTicketCountByStatus();

        public Dictionary<string, int> GetTicketCountByPriority() =>
            _ticketRepository.GetTicketCountByPriority();
    }
}
