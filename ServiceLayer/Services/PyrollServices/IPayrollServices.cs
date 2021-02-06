using DomainLayer.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.PyrollServices
{
    public interface IPayrollServices : IPayrollRepository
    {
        void ValidateModelDataAnnotations(IPayrollModel model);
    }
}
