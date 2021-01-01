using DomainLayer.Models.Role;
using ServiceLayer.CommonServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.RoleServices
{
    public class RoleServices : IRoleServices
    {
        private IRoleRepository roleRepository;
        private IModelDataAnnotationCheck modelDataAnnotationCheck;

        public RoleServices()
        {
                
        }

        public RoleServices(IRoleRepository roleRepository, IModelDataAnnotationCheck modelDataAnnotationCheck )
        {
            this.roleRepository = roleRepository;
            this.modelDataAnnotationCheck = modelDataAnnotationCheck;

        }
        public void Add(IRoleModel roleModel)
        {
            roleRepository.Add(roleModel);
        }

        public IEnumerable<IRoleModel> GetAll()
        {
            return roleRepository.GetAll();
        }

        public RoleModel GetByID(int id)
        {
            return roleRepository.GetByID(id);
        }

        public void Remove(IRoleModel roleModel)
        {
            roleRepository.Remove(roleModel);
        }

        public void Update(IRoleModel roleModel)
        {
            roleRepository.Update(roleModel);
        }

        public void ValidateModelDataAnnotations(IRoleModel roleModel)
        {
            this.modelDataAnnotationCheck.ValidateModelDataAnnotations(roleModel);
        }
    }
}
