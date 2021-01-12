using DomainLayer.Models;
using ServiceLayer.CommonServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.EmployeeServices
{
    public class EmployeeServices : IEmployeeServices
    {

        private IEmployeeRepository employeeRepository;
        private IModelDataAnnotationCheck modelDataAnnotationCheck;

        public EmployeeServices(IEmployeeRepository employeeRepository, IModelDataAnnotationCheck modelDataAnnotationCheck)
        {
            this.employeeRepository = employeeRepository;
            this.modelDataAnnotationCheck = modelDataAnnotationCheck;

        }
        public void Add(IEmployeeModel employeeModel)
        {
            employeeRepository.Add(employeeModel);
        }

        public IEnumerable<IEmployeeModel> GetAll()
        {
            return employeeRepository.GetAll();
        }

        public EmployeeModel GetByID(int employeeId)
        {
            return employeeRepository.GetByID(employeeId);
        }

        public void Remove(IEmployeeModel employeeModel)
        {
            employeeRepository.Remove(employeeModel);
        }

        public void Update(IEmployeeModel employeeModel)
        {
            employeeRepository.Update(employeeModel);
        }

        public void ValidateModelDataAnnotations(IEmployeeModel employeeModel)
        {
            this.modelDataAnnotationCheck.ValidateModelDataAnnotations(employeeModel);
        }
    }
}
