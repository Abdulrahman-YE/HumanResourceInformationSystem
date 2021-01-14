using CommonComponents;
using DomainLayer.Models.Account;
using InfrastructureLayer.DataAccess.Repositories.Account;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceLayer.CommonServices;
using ServiceLayer.Services.AccountServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ServicesLayer.Test.AccountTests
{
    [Trait("Catagory", "Data Access Validation")]
    public class AccountServicesDataAccessTests
    {
        private readonly ITestOutputHelper testOutputHelper;
        private string connectionString;
        private AccountServices accountService;
        private int randomNumber = new Random().Next();

        public AccountServicesDataAccessTests(ITestOutputHelper testOutputHelper)
        {
            connectionString = Properties.Settings.Default.connectionStr;
            accountService = new AccountServices(new AccountRepository(connectionString), new ModelDataAnnotationCheck());
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldReturnListOfDepartments()
        {
            List<AccountModel> accounts = (List<AccountModel>)accountService.GetAll();

            Assert.NotEmpty(accounts);

            foreach (AccountModel account in accounts)
            {
                testOutputHelper.WriteLine($"ID: {account.ID} \nUsername: {account.Username}\nPassword: {account.Password}" +
                    $"\nEmployee id: {account.EmployeeID}\nRole id: {account.RoleID}");
                testOutputHelper.WriteLine("==========================");
            }
        }

        [Fact]
        public void ShouldReturnDepartmentByID()
        {
            AccountModel account = null;
            int idToGet = 1;

            try
            {
                account = accountService.GetByID(idToGet);
            }
            catch (DataAccessException e)
            {

                testOutputHelper.WriteLine(e.DataAccessStatusInfo.getFormattedValues());
            }

            Assert.True(account != null);
            Assert.True(account.ID == idToGet);

            if (account != null)
            {
                string ModelJsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(account);
                string formattedJsonStr = JToken.Parse(ModelJsonStr).ToString();
                testOutputHelper.WriteLine(formattedJsonStr);

            }
        }

        [Fact]
        public void ShouldReturnDepartmentByUsername()
        {
            AccountModel account = null;
            string username = "UnitTestUpdate";

            try
            {
                account = accountService.GetByUsername(username);
            }
            catch (DataAccessException e)
            {

                testOutputHelper.WriteLine(e.DataAccessStatusInfo.getFormattedValues());
            }

            Assert.True(account != null);
            Assert.True(account.Username == username);

            if (account != null)
            {
                string ModelJsonStr = JsonConvert.SerializeObject(account);
                string formattedJsonStr = JToken.Parse(ModelJsonStr).ToString();
                testOutputHelper.WriteLine(formattedJsonStr);

            }
        }

        [Fact]
        public void ShouldReturnSuccessForAdd()
        {
            AccountModel account = new AccountModel();
            account.Username = "UnitTest" + randomNumber;
            account.Password = "abdulrahman";
            account.EmployeeID = 1;
            account.RoleID = 1;


            var opeartionSucceeded = false;
            string formattedJsonStr = string.Empty;
            string dataAccessJsonStr;

            try
            {
                accountService.Add(account);
                opeartionSucceeded = true;

            }
            catch (ArgumentException e)
            {
                dataAccessJsonStr = JsonConvert.SerializeObject(e);
                formattedJsonStr = JToken.FromObject(dataAccessJsonStr).ToString();
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
            AccountModel account = new AccountModel();
            account.ID = 1;
            account.Username = "UnitTestUpdate";
            account.Password = "+9677727272727";
            account.EmployeeID = 1;
            account.RoleID = 1;

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                accountService.Update(account);
                opeartionSucceeded = true;

            }
            catch(ArgumentException e)
            {
                dataAccessJsonStr = JsonConvert.SerializeObject(e);
                formattedJsonStr = JToken.FromObject(dataAccessJsonStr).ToString();
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

            AccountModel account = new AccountModel();
            account.ID = 2;

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                accountService.Remove(account);
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
