using GardenGroupIncidentSystem.Models;
using GardenGroupIncidentSystem.Services.Repositories;
using System;
using System.Collections.Generic;

namespace GardenGroupIncidentSystem.Services
{
    /// <summary>
    /// Service layer for Ticket operations.
    /// Encapsulates business logic and validation before database access.
    /// </summary>
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        // CRUD

        public List<Ticket> GetAllTickets()
        {
            return _ticketRepository.GetAllTickets();
        }

        public Ticket GetTicketById(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
                throw new ArgumentException("Ticket ID cannot be null or empty.");

            return _ticketRepository.GetTicketById(ticketId);
        }

        public Ticket CreateTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            return _ticketRepository.CreateTicket(ticket);
        }

        public void UpdateTicket(string ticketId, Ticket updatedTicket)
        {
            if (string.IsNullOrEmpty(ticketId))
                throw new ArgumentException("Ticket ID cannot be null or empty.");

            if (updatedTicket == null)
                throw new ArgumentNullException(nameof(updatedTicket));

            _ticketRepository.UpdateTicket(ticketId, updatedTicket);
        }

        public void DeleteTicket(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
                throw new ArgumentException("Ticket ID cannot be null or empty.");

            _ticketRepository.DeleteTicket(ticketId);
        }

        // FILTERS

        public List<Ticket> GetFilteredTickets(string q, string status, string priority, string type)
        {
            // Pass filters to repository
            return _ticketRepository.GetFilteredTickets(q, status, priority, type);
        }

        // ANALYTICS

        public Dictionary<Status, int> GetTicketCountByStatus()
        {
            return _ticketRepository.GetTicketCountByStatus();
        }

        public Dictionary<string, int> GetTicketCountByPriority()
        {
            return _ticketRepository.GetTicketCountByPriority();
        }
    }
}
