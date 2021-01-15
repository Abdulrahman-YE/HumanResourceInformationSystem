using DomainLayer.Models.Payroll;
using ServiceLayer.CommonServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.PyrollServices 
{
    public class PayrollServices : IPayrollServices
    {
        private IPayrollRepository repository;
        private IModelDataAnnotationCheck modelData;

        public PayrollServices(IPayrollRepository repository, IModelDataAnnotationCheck modelData)
        {
            this.repository = repository;
            this.modelData = modelData;
        }

        public void Add(IPayrollModel model)
        {
            ValidateModelDataAnnotations(model);
            repository.Add(model);
        }


        public IEnumerable<IPayrollModel> GetAll()
        {
            return repository.GetAll();
        }

        public PayrollModel GetByEmployee(int employeeID)
        {
            return repository.GetByEmployee(employeeID);
        }

        public PayrollModel GetByID(int id)
        {
            return repository.GetByID(id);
        }

        public void Remove(IPayrollModel model)
        {
            repository.Remove(model);
        }

        public void Update(IPayrollModel model)
        {
            ValidateModelDataAnnotations(model);
            repository.Update(model);
        }

        public void ValidateModelDataAnnotations(IPayrollModel model)
        {
            modelData.ValidateModelDataAnnotations(model);
        }
    }
}
