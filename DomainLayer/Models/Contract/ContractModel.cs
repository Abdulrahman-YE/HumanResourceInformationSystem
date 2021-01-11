using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Contract
{
    class ContractModel : IContractModel
    {
        public int ID { get; set; }
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Contract expire date is a required field")]
        public DateTime ExpireDate { get; set; }
        public int EmployeeID { get; set; }
    }
}
