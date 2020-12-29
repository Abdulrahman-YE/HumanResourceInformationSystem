using DomainLayer.Models.Department;
using ServiceLayer.CommonServices;
using System;
using System.Collections.Generic;

namespace ServiceLayer.Services.DepartmentServices
{
    public class DepartmentServices : IDepartmentServices
    {
        private IDepartmentRepository departmentRepository;
        private IModelDataAnnotationCheck modelDataAnnotationCheck;

        public DepartmentServices(IDepartmentRepository departmentRepository, IModelDataAnnotationCheck modelDataAnnotationCheck)
        {
            this.departmentRepository = departmentRepository;
            this.modelDataAnnotationCheck =  modelDataAnnotationCheck;

        }

        public void Add(IDepartmentModel departmentModel)
        {
            departmentRepository.Add(departmentModel);
        }

        public IEnumerable<IDepartmentModel> GetAll()
        {
            return departmentRepository.GetAll();
        }

        public DepartmentModel GetByID(int departmentId)
        {
            return departmentRepository.GetByID(departmentId);
        }

        public void Remove(IDepartmentModel departmentModel)
        {
            departmentRepository.Remove(departmentModel);
        }

        public void Update(IDepartmentModel departmentModel)
        {
            departmentRepository.Update(departmentModel);
        }

        public void ValidateModelDataAnnotations(IDepartmentModel departmentModel)
        {
            this.modelDataAnnotationCheck.ValidateModelDataAnnotations(departmentModel);
        }
    }
}
