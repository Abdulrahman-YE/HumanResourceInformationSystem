using DomainLayer.Models.Role;
using ServiceLayer.CommonServices;
using ServiceLayer.Services.RoleServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Test.RoleTest
{
    public class RoleServicesFixture
    {
        private IRoleServices roleServices;
        private IRoleModel roleModel;

        public RoleServicesFixture()
        {
            this.roleServices = new RoleServices(null, new ModelDataAnnotationCheck());
            this.roleModel = new RoleModel();
        }

        public RoleModel RoleModel
        {
            get { return (RoleModel)this.roleModel; }
            set { this.roleModel = value; }
        }

        public RoleServices RoleServices
        {
            get { return (RoleServices)this.roleServices; }
            set { this.roleServices = value; }
        }

    }
}
