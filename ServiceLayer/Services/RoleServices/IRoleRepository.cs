using DomainLayer.Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.RoleServices
{
    public interface IRoleRepository
    {
        void Add(IRoleModel roleModel);
        void Update(IRoleModel roleModel);
        void Remove(IRoleModel roleModel);
        RoleModel GetByID(int id);
        IEnumerable<IRoleModel> GetAll();
    }
}
