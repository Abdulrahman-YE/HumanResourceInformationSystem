using DomainLayer.Models.Paycheck;
using ServiceLayer.CommonServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.PaycheckServices
{
    public class PaycheckServices : IPaycheckServices
    {
        private IPaycheckRepository repository;
        private IModelDataAnnotationCheck modelCheck;

        public PaycheckServices(IPaycheckRepository repository, IModelDataAnnotationCheck modelCheck)
        {
            this.repository = repository;
            this.modelCheck = modelCheck;
        }

        public void Add(IPaycheckModel model)
        {
            ValidateModelDataAnnotations(model);
            repository.Add(model);
        }


        public IEnumerable<IPaycheckModel> GetAll()
        {
            return repository.GetAll();
        }

        public IEnumerable<IPaycheckModel> GetByEmployee(int employeeID)
        {
            return repository.GetByEmployee(employeeID);
        }

        public IEnumerable<IPaycheckModel> GetByMonth(DateTime date)
        {
            return repository.GetByMonth(date);
        }

        public PaycheckModel GetByID(int id)
        {
            return repository.GetByID(id);
        }

        public void Remove(IPaycheckModel model)
        {
            repository.Remove(model);
        }

        public void Update(IPaycheckModel model)
        {
            ValidateModelDataAnnotations(model);
            repository.Update(model);
        }

        public void ValidateModelDataAnnotations(IPaycheckModel model)
        {
            modelCheck.ValidateModelDataAnnotations(model);
        }
    }
}
