using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GardenGroupIncidentSystem.Models
{
    public enum Role
    {
        supervisor,
        servicedesk_1,
        servicedesk_2,
        servicedesk_3,
        regular
    }
    public class Employee
    {
        [BsonId]
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("Role")]
        public Role EmployeeRole { get; set; }

        [BsonElement("EmailAddress")]
        public string EmailAddress { get; set; }

        [BsonElement("Location")]
        public string Location { get; set; }

        [BsonElement("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }
        public Employee() { }
        public Employee(string id, string password, Role employeeRole, string emailAddress, string location, string phoneNumber, string firstName, string lastName)
        {
            Id = id;
            Password = password;
            EmployeeRole = employeeRole;
            EmailAddress = emailAddress;
            Location = location;
            PhoneNumber = phoneNumber;
            FirstName = firstName;
            LastName = lastName;
        }
    }
    }