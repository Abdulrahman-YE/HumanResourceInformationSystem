using System;

namespace DomainLayer.Models
{
    public interface IEmployeeModel
    {
        string Address { get; set; }
        string Country { get; set; }
        int DepartmentID { get; set; }
        DateTime DOB { get; set; }
        string Email { get; set; }
        string Fullname { get; set; }
        string Gender { get; set; }
        int ID { get; set; }
        byte[] PersonalPhoto { get; set; }
        DateTime HireDate { get; set; }
        string Position { get; set; }

        string PhoneNumber { get; set; }
        string Status { get; set; }
    }
}