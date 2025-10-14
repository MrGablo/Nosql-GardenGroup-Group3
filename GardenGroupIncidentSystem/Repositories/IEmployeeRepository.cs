using GardenGroupIncidentSystem.Models;
using System.Collections.Generic;

namespace GardenGroupIncidentSystem.Services.Repositories
{
    /// <summary>
    /// Repository interface for Employee data access
    /// Defines all CRUD operations for Employee entity
    /// Author: [YOUR NAME]
    /// </summary>
    public interface IEmployeeRepository
    {
        // ID GENERATION
        string GetNextEmployeeId();

        // CREATE
        Employee CreateEmployee(Employee employee);
        void CreateEmployees(List<Employee> employees);

        // READ
        List<Employee> GetAllEmployees();
        Employee GetEmployeeById(string employeeId);
        List<Employee> GetEmployeesByRole(string role);
        List<Employee> GetEmployeesByIds(List<string> employeeIds);
        List<Employee> GetEmployeesByRoles(List<string> roles);
        List<Employee> GetEmployeesByLocationAndRole(string location, string role);
        List<Employee> SearchEmployeesByName(string searchTerm);

        // HELPER METHODS
        bool EmployeeExists(string employeeId);
        long CountAllEmployees();

        // AGGREGATION
        Dictionary<string, int> GetEmployeeCountByRole();
        Dictionary<string, int> GetEmployeeCountByLocation();
        EmployeeStatistics GetEmployeeStatistics();
    }
}