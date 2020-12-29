using DomainLayer.Models.Department;

namespace ServiceLayer.Services.DepartmentServices
{
    public interface IDepartmentServices : IDepartmentRepository
    {
        void ValidateModelDataAnnotations(IDepartmentModel departmentModel);
    }
}
