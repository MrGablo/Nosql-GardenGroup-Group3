using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GardenGroupIncidentSystem.Models
{
    public class Employee
    {
        [BsonId]
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; }

        [BsonElement("Name")]
        public EmployeeName Name { get; set; }

        [BsonElement("ContactDetails")]
        public ContactDetails ContactDetails { get; set; }
    }

    [BsonIgnoreExtraElements]  // THIS IS IMPORTANT!
    public class EmployeeName
    {
        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }
    }

    [BsonIgnoreExtraElements]  // THIS IS IMPORTANT!
    public class ContactDetails
    {
        [BsonElement("EmailAddress")]
        public string EmailAddress { get; set; }

        [BsonElement("Location")]
        public string Location { get; set; }

        [BsonElement("PhoneNumber")]
        public string PhoneNumber { get; set; }
    }
}