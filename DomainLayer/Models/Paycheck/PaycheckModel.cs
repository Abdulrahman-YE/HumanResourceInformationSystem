using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Paycheck
{
    class PaycheckModel : IPaycheckModel
    {

        public int ID { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public int Amount { get; set; }
        public DateTime ReceiptionDate { get; set; }
        public int EmployeeID { get; set; }
        public int PayrollID { get; set; }
    }
}
