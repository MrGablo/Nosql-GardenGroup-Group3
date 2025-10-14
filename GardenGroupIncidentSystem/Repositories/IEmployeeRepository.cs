using GardenGroupIncidentSystem.Models;
using System.Collections.Generic;

namespace NoSQL_Project.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        List<Employee> GetAll();
        Employee? GetById(string id);
        void Create(Employee employee);
        void Update(string id, Employee employee);
        void Delete(string id);
    }
}
