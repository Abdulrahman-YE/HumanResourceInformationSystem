using DomainLayer.Models.Department;
using System.Collections.Generic;


namespace ServiceLayer.Services
{
    public interface IDepartmentRepository
    {
        void Add(IDepartmentModel departmentModel);
        void Update(IDepartmentModel departmentModel);
        void Remove(IDepartmentModel departmentModel);
        IEnumerable<IDepartmentModel> GetAll();
        DepartmentModel GetByID(int departmentId);

    }
}
