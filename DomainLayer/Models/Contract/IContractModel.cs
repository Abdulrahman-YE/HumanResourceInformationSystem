using System;

namespace DomainLayer.Models.Contract
{
    interface IContractModel
    {
        int EmployeeID { get; set; }
        DateTime ExpireDate { get; set; }
        DateTime HireDate { get; set; }
        int ID { get; set; }
    }
}