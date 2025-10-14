using MongoDB.Driver;
using GardenGroupIncidentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GardenGroupIncidentSystem.Services.Repositories
{
    /// <summary>
    /// Repository implementation for Employee data access
    /// Handles all database operations for Employee entity
    /// Author: [YOUR NAME]
    /// 
    /// Design Choice: Repository Pattern
    /// Reason: Separates data access logic from business logic
    /// Alternative: Direct database access in services
    /// Why chosen: Better testability, maintainability, and separation of concerns
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IMongoCollection<Employee> _employees;

        public EmployeeRepository(IMongoDatabase db)
        {
            _employees = db.GetCollection<Employee>("Employee");
        }

        // ========================================================================
        // AUTO-INCREMENT ID GENERATION
        // ========================================================================

        /// <summary>
        /// Generates next Employee ID automatically
        /// Format: E0001, E0002, E0003, etc.
        /// Design Choice: Calculate from existing employees (no counter collection)
        /// Alternative: Separate counter collection with atomic increment
        /// Why chosen: Simpler implementation, adequate for project scale
        /// Trade-off: Slightly slower, but code is cleaner and easier to maintain
        /// </summary>
        public string GetNextEmployeeId()
        {
            // Get all employees to find max ID
            var allEmployees = _employees.Find(_ => true).ToList();

            int maxNumber = 0;

            // Find the highest existing employee number
            foreach (var emp in allEmployees)
            {
                // Check if ID starts with 'E' and extract number
                if (emp.Id.StartsWith("E") && int.TryParse(emp.Id.Substring(1), out int num))
                {
                    if (num > maxNumber)
                        maxNumber = num;
                }
            }

            // Return next number formatted as E0001, E0002, etc.
            return $"E{(maxNumber + 1):D4}";
        }

        // ========================================================================
        // CREATE OPERATIONS
        // ========================================================================

        /// <summary>
        /// Creates a new employee with auto-generated ID
        /// </summary>
        public Employee CreateEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            // Auto-generate ID if not provided
            if (string.IsNullOrEmpty(employee.Id))
            {
                employee.Id = GetNextEmployeeId();
            }
            else
            {
                // If ID is provided, check if it exists
                if (EmployeeExists(employee.Id))
                    throw new InvalidOperationException($"Employee {employee.Id} already exists");
            }

            _employees.InsertOne(employee);
            return employee;
        }

        /// <summary>
        /// Creates multiple employees at once (bulk operation)
        /// Design Choice: Grouped operation for efficiency
        /// </summary>
        public void CreateEmployees(List<Employee> employees)
        {
            if (employees == null || employees.Count == 0)
                throw new ArgumentException("Employee list cannot be empty");

            // Auto-generate IDs for all employees
            foreach (var emp in employees)
            {
                if (string.IsNullOrEmpty(emp.Id))
                {
                    emp.Id = GetNextEmployeeId();
                }
            }

            _employees.InsertMany(employees);
        }

        // ========================================================================
        // READ OPERATIONS
        // ========================================================================

        public List<Employee> GetAllEmployees()
        {
            return _employees.Find(emp => true).ToList();
        }

        public Employee GetEmployeeById(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
                return null;

            return _employees.Find(emp => emp.Id == employeeId).FirstOrDefault();
        }

        public List<Employee> GetEmployeesByRole(string role)
        {
            if (string.IsNullOrEmpty(role))
                return new List<Employee>();

            return _employees.Find(emp => emp.Role == role).ToList();
        }

        public List<Employee> GetEmployeesByIds(List<string> employeeIds)
        {
            if (employeeIds == null || employeeIds.Count == 0)
                return new List<Employee>();

            var filter = Builders<Employee>.Filter.In(emp => emp.Id, employeeIds);
            return _employees.Find(filter).ToList();
        }

        public List<Employee> GetEmployeesByRoles(List<string> roles)
        {
            if (roles == null || roles.Count == 0)
                return new List<Employee>();

            var filter = Builders<Employee>.Filter.In(emp => emp.Role, roles);
            return _employees.Find(filter).ToList();
        }

        public List<Employee> GetEmployeesByLocationAndRole(string location, string role)
        {
            var filterBuilder = Builders<Employee>.Filter;
            var filters = new List<FilterDefinition<Employee>>();

            if (!string.IsNullOrEmpty(location))
                filters.Add(filterBuilder.Eq("ContactDetails.Location", location));

            if (!string.IsNullOrEmpty(role))
                filters.Add(filterBuilder.Eq(emp => emp.Role, role));

            if (filters.Count == 0)
                return new List<Employee>();

            var combinedFilter = filterBuilder.And(filters);
            return _employees.Find(combinedFilter).ToList();
        }

        public List<Employee> SearchEmployeesByName(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return new List<Employee>();

            var filter = Builders<Employee>.Filter.Or(
                Builders<Employee>.Filter.Regex("Name.FirstName",
                    new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<Employee>.Filter.Regex("Name.LastName",
                    new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );

            return _employees.Find(filter).ToList();
        }

        // ========================================================================
        // AGGREGATION PIPELINES
        // ========================================================================

        public Dictionary<string, int> GetEmployeeCountByRole()
        {
            var pipeline = _employees.Aggregate()
                .Group(
                    emp => emp.Role,
                    group => new
                    {
                        Role = group.Key,
                        Count = group.Count()
                    }
                )
                .ToList();

            return pipeline.ToDictionary(x => x.Role, x => x.Count);
        }

        public Dictionary<string, int> GetEmployeeCountByLocation()
        {
            var pipeline = _employees.Aggregate()
                .Group(
                    emp => emp.ContactDetails.Location,
                    group => new
                    {
                        Location = group.Key,
                        Count = group.Count()
                    }
                )
                .ToList();

            return pipeline.ToDictionary(x => x.Location, x => x.Count);
        }

        public EmployeeStatistics GetEmployeeStatistics()
        {
            var total = _employees.CountDocuments(emp => true);
            var byRole = GetEmployeeCountByRole();
            var byLocation = GetEmployeeCountByLocation();

            return new EmployeeStatistics
            {
                TotalEmployees = (int)total,
                ByRole = byRole,
                ByLocation = byLocation
            };
        }

        // ========================================================================
        // HELPER METHODS
        // ========================================================================

        public bool EmployeeExists(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
                return false;

            return _employees.CountDocuments(emp => emp.Id == employeeId) > 0;
        }

        public long CountAllEmployees()
        {
            return _employees.CountDocuments(emp => true);
        }
    }
}