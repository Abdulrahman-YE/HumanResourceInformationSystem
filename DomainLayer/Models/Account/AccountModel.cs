using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Account
{
    class AccountModel : IAccountModel
    {

        public int ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is a required field")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Username should be between 3 and 40 characters.")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is a required field")]
        [StringLength(60, MinimumLength = 8, ErrorMessage = "Password must contain more than 8 characters.")]
        public string Password { get; set; }
        public int RoleID { get; set; }
        public int EmployeeID { get; set; }
    }
}
