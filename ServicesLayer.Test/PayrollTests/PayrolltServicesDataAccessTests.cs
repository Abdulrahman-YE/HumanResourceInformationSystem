using CommonComponents;
using DomainLayer.Models.Payroll;
using InfrastructureLayer.DataAccess.Repositories.Payroll;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceLayer.CommonServices;
using ServiceLayer.Services.PyrollServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ServicesLayer.Test.PayrollTests
{
    public class PayrolltServicesDataAccessTests
    {

        private readonly ITestOutputHelper testOutputHelper;
        private string connectionString;
        private PayrollServices payrollService;

        public PayrolltServicesDataAccessTests(ITestOutputHelper testOutputHelper)
        {
            connectionString = Properties.Settings.Default.connectionStr;
            payrollService = new PayrollServices(new PayrollRepository(connectionString), new ModelDataAnnotationCheck());
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldReturnListOfPayrolls()
        {
            List<PayrollModel> payrolls = (List<PayrollModel>)payrollService.GetAll();

            Assert.NotEmpty(payrolls);

            foreach (PayrollModel payroll in payrolls)
            {
                testOutputHelper.WriteLine($"ID: {payroll.ID} \nGross Pay: {payroll.GrossPay}\nNet pay: {payroll.NetPay}" +
                    $"\nEmployee id: {payroll.EmployeeID}");
                testOutputHelper.WriteLine("==========================");
            }
        }

        [Fact]
        public void ShouldReturnPayrollByID()
        {
            PayrollModel payroll = null;
            int idToGet = 4;

            try
            {
                payroll = payrollService.GetByID(idToGet);
            }
            catch (DataAccessException e)
            {

                testOutputHelper.WriteLine(e.DataAccessStatusInfo.getFormattedValues());
            }

            Assert.True(payroll != null);
            Assert.True(payroll.ID == idToGet);

            if (payroll != null)
            {
                string ModelJsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(payroll);
                string formattedJsonStr = JToken.Parse(ModelJsonStr).ToString();
                testOutputHelper.WriteLine(formattedJsonStr);

            }
        }

        [Fact]
        public void ShouldReturnPayrollByEmployee()
        {
            PayrollModel account = null;
            int employeeID = 1;

            try
            {
                account = payrollService.GetByEmployee(employeeID);
            }
            catch (DataAccessException e)
            {

                testOutputHelper.WriteLine(e.DataAccessStatusInfo.getFormattedValues());
            }

            Assert.True(account != null);
            Assert.True(account.EmployeeID == employeeID);

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
            PayrollModel payroll = new PayrollModel();
            payroll.GrossPay = 22000000;
            payroll.NetPay = 22000000;
            payroll.EmployeeID = 4;

            var opeartionSucceeded = false;
            string formattedJsonStr = string.Empty;
            string dataAccessJsonStr;

            try
            {
                payrollService.Add(payroll);
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
            PayrollModel payroll = new PayrollModel();
            payroll.ID = 6;
            payroll.GrossPay = 1500000;
            payroll.NetPay = 1200000;
            payroll.EmployeeID = 3;

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                payrollService.Update(payroll);
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

            PayrollModel account = new PayrollModel();
            account.ID = 6;

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                payrollService.Remove(account);
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
