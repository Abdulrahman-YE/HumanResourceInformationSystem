using DomainLayer.Models.Paycheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.PaycheckServices
{
    public interface IPaycheckRepository
    {
        void Add(IPaycheckModel model);
        void Update(IPaycheckModel model);
        void Remove(IPaycheckModel model);
        IEnumerable<IPaycheckModel> GetAll();
        PaycheckModel GetByID(int id);
        IEnumerable<IPaycheckModel> GetByEmployee(int employeeID);
        IEnumerable<IPaycheckModel> GetByMonth(DateTime date);


    }
}
