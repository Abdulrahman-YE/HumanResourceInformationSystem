using Xunit;
using Xunit.Abstractions;
using InfrastructureLayer.DataAccess.Repositories.Specific.Department;
using ServiceLayer.Services.DepartmentServices;
using ServiceLayer.CommonServices;
using DomainLayer.Models.Department;
using System.Collections.Generic;
using CommonComponents;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ServicesLayer.Test.DepartmentTests
{

    [Trait("Catagory", "Data Access Validation")]
    public class DepartmentServicesDataAccessTests
    {
        private readonly ITestOutputHelper testOutputHelper;
        private string connectionString;
        private DepartmentServices departmentService;

        public DepartmentServicesDataAccessTests(ITestOutputHelper testOutputHelper)
        {
            connectionString = Properties.Settings.Default.connectionStr;
            departmentService = new DepartmentServices(new DepartmentRepository(connectionString), new ModelDataAnnotationCheck());
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldReturnListOfDepartments()
        {
            List<DepartmentModel> depatmentsList = (List<DepartmentModel>)departmentService.GetAll();

            Assert.NotEmpty(depatmentsList);

            foreach(DepartmentModel dm in depatmentsList)
            {
                testOutputHelper.WriteLine($"Name: {dm.DepartmentName}\nPhone Number: {dm.PhoneNumber}\nManager id: {dm.ManagerID}");
            }
        }

        [Fact]
        public void ShouldReturnDepartmentByID()
        {
            DepartmentModel departmentModel = null;
            int idToGet = 5;

            try
            {
                departmentModel = departmentService.GetByID(idToGet);
            }
            catch (DataAccessException e)
            {

                testOutputHelper.WriteLine(e.DataAccessStatusInfo.getFormattedValues());
            }

            Assert.True(departmentModel != null);
            Assert.True(departmentModel.DepartmentId == idToGet);

            if (departmentModel != null)
            {
                string departmentModelJsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(departmentModel);
                string formattedJsonStr = JToken.Parse(departmentModelJsonStr).ToString();
                testOutputHelper.WriteLine(formattedJsonStr);

            }
        }

        [Fact]
        public void ShouldReturnSuccessForAdd()
        {
            DepartmentModel departmentModel = new DepartmentModel();
            departmentModel.DepartmentName = "Unit 1425";
            departmentModel.PhoneNumber = "+9677777777";
            departmentModel.ManagerID = 3;

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                departmentService.Add(departmentModel);
                opeartionSucceeded = true;

            }
            catch (DataAccessException e)
            {
                e.DataAccessStatusInfo.OperationSucceeded = opeartionSucceeded;
                dataAccessJsonStr = JsonConvert.SerializeObject(e.DataAccessStatusInfo);
                formattedJsonStr = JToken.FromObject(dataAccessJsonStr).ToString();

            }

            try
            {
                Assert.True(opeartionSucceeded);
                testOutputHelper.WriteLine("The record has been succesfully added");
            }
            finally
            {
                testOutputHelper.WriteLine(formattedJsonStr);
            }
        }

        [Fact]
        public void ShouldReturnSuccessForUpdate()
        {
            DepartmentModel departmentModel = new DepartmentModel();
            departmentModel.DepartmentId = 3;
            departmentModel.DepartmentName = "Human Resource Department(Unit test 120)";
            departmentModel.PhoneNumber = "+9677727272727";
            departmentModel.ManagerID = 6;

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                departmentService.Update(departmentModel);
                opeartionSucceeded = true;

            }
            catch (DataAccessException e)
            {
                e.DataAccessStatusInfo.OperationSucceeded = opeartionSucceeded;
                dataAccessJsonStr = JsonConvert.SerializeObject(e.DataAccessStatusInfo);
                formattedJsonStr = JToken.FromObject(dataAccessJsonStr).ToString();

            }

            try
            {
                Assert.True(opeartionSucceeded);
                testOutputHelper.WriteLine("The record has been succesfully updated");
            }
            finally
            {
                testOutputHelper.WriteLine(formattedJsonStr);
            }

        }

        [Fact]
        public void ShouldReturnSuccessForDelete()
        {

            DepartmentModel departmentModel = new DepartmentModel();
            departmentModel.DepartmentId = 5;
            departmentModel.DepartmentName = "(Unit test 10)";
            departmentModel.PhoneNumber = "+9677727272727";
            departmentModel.ManagerID = 6;

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                departmentService.Update(departmentModel);
                opeartionSucceeded = true;

            }
            catch (DataAccessException e)
            {
                e.DataAccessStatusInfo.OperationSucceeded = opeartionSucceeded;
                dataAccessJsonStr = JsonConvert.SerializeObject(e.DataAccessStatusInfo);
                formattedJsonStr = JToken.FromObject(dataAccessJsonStr).ToString();

            }

            try
            {
                Assert.True(opeartionSucceeded);
                testOutputHelper.WriteLine("The record has been succesfully updated");
            }
            finally
            {
                testOutputHelper.WriteLine(formattedJsonStr);
            }


        }
    }

   
}
