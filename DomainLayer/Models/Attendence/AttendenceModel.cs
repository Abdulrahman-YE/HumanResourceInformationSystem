using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Attendence
{
    class AttendenceModel : IAttendenceModel
    {

        public int ID { get; set; }
        public DateTime AttendenceDate { get; set; }
        public bool HasAttended { get; set; }
        public int EmployeeID { get; set; }
    }
}
