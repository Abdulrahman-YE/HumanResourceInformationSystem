using System;

namespace DomainLayer.Models.Attendence
{
    interface IAttendenceModel
    {
        DateTime AttendenceDate { get; set; }
        int EmployeeID { get; set; }
        bool HasAttended { get; set; }
        int ID { get; set; }
    }
}