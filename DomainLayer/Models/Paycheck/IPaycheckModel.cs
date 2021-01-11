using System;

namespace DomainLayer.Models.Paycheck
{
    interface IPaycheckModel
    {
        int Amount { get; set; }
        int EmployeeID { get; set; }
        int ID { get; set; }
        int PayrollID { get; set; }
        DateTime ReceiptionDate { get; set; }
    }
}