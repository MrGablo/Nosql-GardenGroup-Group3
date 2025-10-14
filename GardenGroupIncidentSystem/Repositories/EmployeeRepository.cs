using MongoDB.Driver;
using GardenGroupIncidentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GardenGroupIncidentSystem.Services.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IMongoCollection<Employee> _employees;

        public EmployeeRepository(IMongoDatabase db)
        {
            _employees = db.GetCollection<Employee>("Employee");
        }

        public string GetNextEmployeeId()
        {
            var allEmployees = _employees.Find(_ => true).ToList();
            int maxNumber = 0;

            foreach (var emp in allEmployees)
            {
                if (emp.Id.StartsWith("E") && int.TryParse(emp.Id.Substring(1), out int num))
                {
                    if (num > maxNumber)
                        maxNumber = num;
                }
            }

            return $"E{(maxNumber + 1):D4}";
        }

        public Employee CreateEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (string.IsNullOrEmpty(employee.Id))
            {
                employee.Id = GetNextEmployeeId();
            }
            else
            {
                if (EmployeeExists(employee.Id))
                    throw new InvalidOperationException($"Employee {employee.Id} already exists");
            }

            _employees.InsertOne(employee);
            return employee;
        }

        public void CreateEmployees(List<Employee> employees)
        {
            if (employees == null || employees.Count == 0)
                throw new ArgumentException("Employee list cannot be empty");

            foreach (var emp in employees)
            {
                if (string.IsNullOrEmpty(emp.Id))
                {
                    emp.Id = GetNextEmployeeId();
                }
            }

            _employees.InsertMany(employees);
        }

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

        public List<Employee> GetEmployeesByRole(Role role)
        {
            return _employees.Find(emp => emp.EmployeeRole == role).ToList();
        }

        public List<Employee> GetEmployeesByIds(List<string> employeeIds)
        {
            if (employeeIds == null || employeeIds.Count == 0)
                return new List<Employee>();

            var filter = Builders<Employee>.Filter.In(emp => emp.Id, employeeIds);
            return _employees.Find(filter).ToList();
        }

        public List<Employee> GetEmployeesByRoles(List<Role> roles)
        {
            if (roles == null || roles.Count == 0)
                return new List<Employee>();

            var filter = Builders<Employee>.Filter.In(emp => emp.EmployeeRole, roles);
            return _employees.Find(filter).ToList();
        }

        public List<Employee> GetEmployeesByLocationAndRole(string location, Role role)
        {
            var filterBuilder = Builders<Employee>.Filter;
            var filters = new List<FilterDefinition<Employee>>();

            if (!string.IsNullOrEmpty(location))
                filters.Add(filterBuilder.Eq("ContactDetails.Location", location));

            filters.Add(filterBuilder.Eq(emp => emp.EmployeeRole, role));

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

        public Dictionary<string, int> GetEmployeeCountByRole()
        {
            var pipeline = _employees.Aggregate()
                .Group(
                    emp => emp.EmployeeRole,
                    group => new
                    {
                        Role = group.Key,
                        Count = group.Count()
                    }
                )
                .ToList();

            return pipeline.ToDictionary(x => x.Role.ToString(), x => x.Count);
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

            return pipeline.ToDictionary(x => x.Location ?? "Unknown", x => x.Count);
        }

        public Employee.EmployeeStatistics GetEmployeeStatistics()
        {
            var total = _employees.CountDocuments(emp => true);
            var byRole = GetEmployeeCountByRole();
            var byLocation = GetEmployeeCountByLocation();

            return new Employee.EmployeeStatistics
            {
                TotalEmployees = (int)total,
                ByRole = byRole,
                ByLocation = byLocation
            };
        }

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