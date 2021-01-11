using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.JobDescription
{
    class JobModel : IJobModel
    {

        public int ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Job title is required")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Job title should be between 2 and 10 characters.")]
        public string JobTitle { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Position is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Position length should be between 2 and 20 characters.")]
        public string Position { get; set; }

        public bool IsActive { get; set; }
        public int EmployeeID { get; set; }
    }
}
