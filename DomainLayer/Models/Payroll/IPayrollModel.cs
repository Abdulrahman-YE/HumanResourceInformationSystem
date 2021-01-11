namespace DomainLayer.Models.Payroll
{
    interface IPayrollModel
    {
        int EmployeeID { get; set; }
        int GrossPay { get; set; }
        int ID { get; set; }
        int NetPay { get; set; }
    }
}