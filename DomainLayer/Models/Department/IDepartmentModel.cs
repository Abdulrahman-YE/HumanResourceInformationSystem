using DomainLayer.Models.Location;

namespace DomainLayer.Models.Department
{
    public interface IDepartmentModel
    {
        int DepartmentId { get; set; }
        string DepartmentName { get; set; }
        int ManagerID { get; set; }
        string PhoneNumber { get; set; }
    }
}