using CommonComponents;
using DomainLayer.Models.Role;
using InfrastructureLayer.DataAccess.Repositories.Specific;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceLayer.CommonServices;
using ServiceLayer.Services.RoleServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ServicesLayer.Test.RoleTest
{
    [Trait("Catagory", "Data Access Validation")]
    public class RoleServicesDataAccessTests
    {

        private readonly ITestOutputHelper testOutputHelper;
        private RoleServices roleServices;

        public RoleServicesDataAccessTests(ITestOutputHelper testOutputHelper)
        {
            roleServices = new RoleServices(new RoleRepository(), new ModelDataAnnotationCheck());
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldReturnListOfRoles()
        {
            List<RoleModel> rolesList = (List<RoleModel>)roleServices.GetAll();

            Assert.NotEmpty(rolesList);

            foreach (RoleModel rm in rolesList)
            {
                testOutputHelper.WriteLine($"Name: {rm.Name}.");
            }
        }

        [Fact]
        public void ShouldReturnRoleByID()
        {
            RoleModel roleModel = null;
            int idToGet = 2;

            try
            {
                roleModel = roleServices.GetByID(idToGet);
            }
            catch (DataAccessException e)
            {

                testOutputHelper.WriteLine(e.DataAccessStatusInfo.getFormattedValues());
            }

            Assert.True(roleModel != null);
            Assert.True(roleModel.ID == idToGet);

            if (roleModel != null)
            {
                string roleModelJsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(roleModel);
                string formattedJsonStr = JToken.Parse(roleModelJsonStr).ToString();
                testOutputHelper.WriteLine(formattedJsonStr);

            }
        }

        [Fact]
        public void ShouldReturnSuccessForAdd()
        {
            RoleModel roleModel = new RoleModel();
            roleModel.Name = "No one";

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                roleServices.Add(roleModel);
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
            RoleModel roleModel = new RoleModel();
            roleModel.ID = 4;
            roleModel.Name= "Unit test updated";

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                roleServices.Update(roleModel);
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

            RoleModel roleModel = new RoleModel();
            roleModel.ID = 4;

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                roleServices.Remove(roleModel);
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
                testOutputHelper.WriteLine("The record has been succesfully deleted");
            }
            finally
            {
                testOutputHelper.WriteLine(formattedJsonStr);
            }


        }
    }
}
