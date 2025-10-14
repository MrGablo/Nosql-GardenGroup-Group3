using GardenGroupIncidentSystem.Models;
using System.Collections.Generic;

namespace GardenGroupIncidentSystem.Services
{
    public interface ITicketService
    {
        Ticket CreateTicket(Ticket ticket);
        void CreateTickets(List<Ticket> tickets);
        List<Ticket> GetAllTickets();
        Ticket GetTicketById(string ticketId);
        List<Ticket> GetTicketsByStatus(Status status);
        List<Ticket> GetTicketsByEmployee(string employeeId);
        void UpdateTicket(string ticketId, Ticket updatedTicket);
        void DeleteTicket(string ticketId);
        Dictionary<Status, int> GetTicketCountByStatus();
        Dictionary<string, int> GetTicketCountByPriority();
    }
}
