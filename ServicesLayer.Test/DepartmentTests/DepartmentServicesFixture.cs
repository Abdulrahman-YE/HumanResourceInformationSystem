using DomainLayer.Models.Department;
using ServiceLayer.CommonServices;
using ServiceLayer.Services.DepartmentServices;


namespace ServicesLayer.Test.DepartmentTests
{
    /// <summary>
    /// This fixture is used to act as a resources instantiatet once instead of intsiate it for each function
    /// </summary>
    public class DepartmentServicesFixture
    {
        private IDepartmentServices departmentServices;
        private IDepartmentModel departmentModel;

        public DepartmentServicesFixture()
        {
            this.departmentServices = new DepartmentServices(null, new ModelDataAnnotationCheck());
            this.departmentModel = new DepartmentModel();
        }

        public DepartmentModel DepartmentModel
        {
            get { return (DepartmentModel)this.departmentModel;  }
            set { this.departmentModel = value; }
        }

        public DepartmentServices DepartmentServices
        {
            get { return (DepartmentServices)this.departmentServices;  }
            set { this.departmentServices = value;  }
        }
    }
}
