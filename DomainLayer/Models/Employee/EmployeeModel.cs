using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    class EmployeeModel : IEmployeeModel
    {

        public int ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Fullname is required field")]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Name must be between 5 and 40 characters")]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z]*$", ErrorMessage = "Employee Name field can conatin only letters and space")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Phone is required field")]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Phone format is incorrect")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required field")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Date of birth is required field")]
        public DateTime DOB { get; set; }

        public int Age { get; set; }

        [Required(ErrorMessage = "Select a gender")]
        [RegularExpression(@"^male$|^female$", ErrorMessage = "Gender can only be either a male or a female.")]
        public string Gender { get; set; }

        [RegularExpression(@"^divorced|^single$|^married$", ErrorMessage = "Status can only be single, married, or divorced.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Select a country")]
        public string Country { get; set; }


        [Required(ErrorMessage = "Email is a required field")]
        [RegularExpression(@"^([a - zA - Z0 - 9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{ 1,3}\.)| (([a - zA - Z0 - 9\-] +\.)+))([a - zA - Z]{ 2,4}|[0 - 9]{ 1,3})(\]?)$", ErrorMessage = "Email format is invalid. Valid format example@example.com")]
        public string Email { get; set; }

        public Byte[] PersonalPhoto { get; set; }


        public int DepartmentID { get; set; }

    }
}
