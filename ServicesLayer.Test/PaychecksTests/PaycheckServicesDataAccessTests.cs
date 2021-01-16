using CommonComponents;
using DomainLayer.Models.Paycheck;
using InfrastructureLayer.DataAccess.Repositories.Employee;
using InfrastructureLayer.DataAccess.Repositories.Paycheck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceLayer.CommonServices;
using ServiceLayer.Services.EmployeeServices;
using ServiceLayer.Services.PaycheckServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ServicesLayer.Test.PaychecksTests
{
    [Trait("Catagory", "Data Access Validation")]
    public class PaycheckServicesDataAccessTests
    {
        private readonly ITestOutputHelper testOutputHelper;
        private string connectionString;
        private PaycheckServices paycheckServices;

        public PaycheckServicesDataAccessTests(ITestOutputHelper testOutputHelper)
        {
            connectionString = Properties.Settings.Default.connectionStr;
            paycheckServices = new PaycheckServices(new PaycheckRepository(connectionString), new ModelDataAnnotationCheck());
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldReturnListOfPaychecks()
        {
            List<PaycheckModel> paychecks = (List<PaycheckModel>)paycheckServices.GetAll();

            Assert.NotEmpty(paychecks);

            foreach (PaycheckModel paycheck in paychecks)
            {
                testOutputHelper.WriteLine($"ID: {paycheck.ID} \nAmount: {paycheck.Amount}\nEmplpoyeeID: {paycheck.EmployeeID}" +
                    $"\nPayrollID: {paycheck.PayrollID}\nReceiption Date: {paycheck.ReceiptionDate}");
                testOutputHelper.WriteLine("==========================");
            }
        }

        [Fact]
        public void ShouldReturnPaycheckByID()
        {
            PaycheckModel paycheck = null;
            int idToGet = 6;

            try
            {
                paycheck = paycheckServices.GetByID(idToGet);
            }
            catch (DataAccessException e)
            {

                testOutputHelper.WriteLine(e.DataAccessStatusInfo.getFormattedValues());
            }

            Assert.True(paycheck != null);
            Assert.True(paycheck.ID == idToGet);

            if (paycheck != null)
            {
                string ModelJsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(paycheck);
                string formattedJsonStr = JToken.Parse(ModelJsonStr).ToString();
                testOutputHelper.WriteLine(formattedJsonStr);

            }
        }

        [Fact]
        public void ShouldReturnPayrollByEmployee()
        {
            int employeeID = 3;
            List<PaycheckModel> paychecks =(List<PaycheckModel>) paycheckServices.GetByEmployee(employeeID) ;

            Assert.NotEmpty(paychecks);

            foreach (PaycheckModel paycheck in paychecks)
            {
                testOutputHelper.WriteLine($"ID: {paycheck.ID} \nAmount: {paycheck.Amount}\nEmplpoyeeID: {paycheck.EmployeeID}" +
                    $"\nPayrollID: {paycheck.PayrollID}\nReceiption Date: {paycheck.ReceiptionDate}");
                testOutputHelper.WriteLine("==========================");
            }
        }

        [Fact]
        public void ShouldReturnPayrollByMonth()
        {
            int employeeID = 1;
            DateTime dataTime = DateTime.Now;
            List<PaycheckModel> paychecks = (List<PaycheckModel>)paycheckServices.GetByMonth(dataTime);

            Assert.NotEmpty(paychecks);

            foreach (PaycheckModel paycheck in paychecks)
            {
                testOutputHelper.WriteLine($"ID: {paycheck.ID} \nAmount: {paycheck.Amount}\nEmplpoyeeID: {paycheck.EmployeeID}" +
                    $"\nPayrollID: {paycheck.PayrollID}\nReceiption Date: {paycheck.ReceiptionDate}");
                testOutputHelper.WriteLine("==========================");
            }
        }

        [Fact]
        public void ShouldThrowExceptionIfPaycheckAmountIsBiggerThanTheNetpay()
        {
            PaycheckModel paycheck = new PaycheckModel();
            paycheck.Amount = 1000000000;
            paycheck.EmployeeID = 4;
            paycheck.PayrollID = 7;



               Exception ex =  Assert.Throws<DataAccessException>(testCode: () => this.paycheckServices.Add(paycheck));

            testOutputHelper.WriteLine(ex.Message);
        }



        [Fact]
        public void ShouldReturnSuccessForAdd()
        {
            PaycheckModel paycheck = new PaycheckModel();
            paycheck.Amount = 10000;
            paycheck.EmployeeID = 4;
            paycheck.PayrollID = 7;

            var opeartionSucceeded = false;
            string formattedJsonStr = string.Empty;
            string dataAccessJsonStr;

            try
            {
                paycheckServices.Add(paycheck);
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
            PaycheckModel paycheck = new PaycheckModel();
            paycheck.ID = 20;
            paycheck.Amount = 20000;
            paycheck.EmployeeID = 3;
            paycheck.PayrollID = 6;

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                paycheckServices.Update(paycheck);
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

            PaycheckModel paycheck = new PaycheckModel();
            paycheck.ID = 7;

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                paycheckServices.Remove(paycheck);
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
