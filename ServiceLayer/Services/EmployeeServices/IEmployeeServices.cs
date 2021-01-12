using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.EmployeeServices
{
    public interface IEmployeeServices : IEmployeeRepository
    {
        void ValidateModelDataAnnotations(IEmployeeModel employeeModel);

    }
}
