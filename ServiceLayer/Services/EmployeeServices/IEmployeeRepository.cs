using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.EmployeeServices
{
    public interface IEmployeeRepository
    {

        void Add(IEmployeeModel employeeModel);
        void Update(IEmployeeModel employeeModel);
        void Remove(IEmployeeModel employeeModel);
        IEnumerable<IEmployeeModel> GetAll();
        EmployeeModel GetByID(int employeeId);
    }
}
