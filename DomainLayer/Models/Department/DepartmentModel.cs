using System.ComponentModel.DataAnnotations;
using DomainLayer.Models.Location;

namespace DomainLayer.Models.Department
{
    public class DepartmentModel : IDepartmentModel
    {

        public int DepartmentId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Department name is required field")]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Department name must be between 5 and 40 characters")]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z]*$", ErrorMessage = "Department Name field can conatin only letters and space")]
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Phone is required field")]
        [RegularExpression(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", ErrorMessage = "Phone format is incorrect")]
        public string PhoneNumber { get; set; }

        public int ManagerID { get; set; }

    }
}
