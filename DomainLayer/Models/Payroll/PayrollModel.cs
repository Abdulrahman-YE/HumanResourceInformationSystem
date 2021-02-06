using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Payroll
{
    public class PayrollModel : IPayrollModel
    {

        public int ID { get; set; }

        [Required(ErrorMessage = "Please specify salary grosspay")]
        public int GrossPay { get; set; }
        public int NetPay { get; set; }
        [Required(ErrorMessage ="Please specify an employee.")]
        public int EmployeeID { get; set; }
    }
}
