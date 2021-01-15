using DomainLayer.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.PyrollServices
{
    public interface IPayrollRepository
    {

        void Add(IPayrollModel model);
        void Update(IPayrollModel model);
        void Remove(IPayrollModel model);
        IEnumerable<IPayrollModel> GetAll();
        PayrollModel GetByID(int id);
        PayrollModel GetByEmployee(int employeeID);
    }
}
