using DomainLayer.Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.RoleServices
{
    public interface IRoleServices : IRoleRepository
    {
        void ValidateModelDataAnnotations(IRoleModel roleModel);

    }
}
