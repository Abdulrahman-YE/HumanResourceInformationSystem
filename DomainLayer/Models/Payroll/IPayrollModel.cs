namespace DomainLayer.Models.Payroll
{
    public interface IPayrollModel
    {
        int EmployeeID { get; set; }
        int GrossPay { get; set; }
        int ID { get; set; }
        int NetPay { get; set; }
    }
}