using CommonComponents;
using DomainLayer.Models;
using InfrastructureLayer.DataAccess.Repositories.Employee;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceLayer.CommonServices;
using ServiceLayer.Services.EmployeeServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace ServicesLayer.Test.EmployeeTests
{
    [Trait("Catagory", "Data Access Validation")]
    public class EmployeeServicesDataAccessTests
    {

        private readonly ITestOutputHelper testOutputHelper;
        private string connectionString;
        private EmployeeServices employeeServices;
        private int testNum = 0;
        public EmployeeServicesDataAccessTests(ITestOutputHelper testOutputHelper)
        {
            connectionString = Properties.Settings.Default.connectionStr;
            employeeServices = new EmployeeServices(new EmployeeRepository(connectionString), new ModelDataAnnotationCheck());
            this.testOutputHelper = testOutputHelper;
            testNum = new Random().Next();
            
        }

        [Fact]
        public void ShouldReturnListOfEmployees()
        {
            List<EmployeeModel> employees = (List<EmployeeModel>)employeeServices.GetAll();

            Assert.NotEmpty(employees);

            foreach (EmployeeModel em in employees)
            {
                testOutputHelper.WriteLine($"ID: {em.ID}\nName: {em.Fullname}\nPhone Number: {em.PhoneNumber}\nAddress: {em.Address}" +
                    $"\nGender: {em.Gender}\nStatus: {em.Status}\nDate of Birth: {em.DOB.Date}\nEmail: {em.Email}\nCountry: {em.Country}\nPhoto: {em.PersonalPhoto.Length}" +
                    $"\nHireDate: {em.HireDate.Date}\nPosition: {em.Position}\nDepartment ID: {em.DepartmentID}");
            }
        }

        [Fact]
        public void ShouldReturnEmployeeByID()
        {
            EmployeeModel employee = null;
            int idToGet = 3;

            try
            {
                employee = employeeServices.GetByID(idToGet);
            }
            catch (DataAccessException e)
            {

                testOutputHelper.WriteLine(e.DataAccessStatusInfo.getFormattedValues());
            }

            Assert.True(employee != null);
            Assert.True(employee.ID == idToGet);

            if (employee != null)
            {
                string employeeModelJsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(employee);
                string formattedJsonStr = JToken.Parse(employeeModelJsonStr).ToString();
                testOutputHelper.WriteLine(formattedJsonStr);

            }
        }

        [Fact]
        public void ShouldReturnSuccessForAdd()
        {
            EmployeeModel employee = new EmployeeModel();
            employee.Fullname = "Unit 2";
            employee.PhoneNumber = "+9677777777";
            employee.Address = "Al-Asbahi St.";
            employee.DOB = DateTime.Now;
            employee.Country = "Yemen";
            employee.Email = "Abdulrahman"+ testNum.ToString() +"@gmail.com";
            employee.Gender = "male";
            employee.Position = "Worker";
            employee.Status = "divorced";

            FileInfo fileinfo = new FileInfo("C:\\Users\\alwani\\Downloads\\Wallpaper\\18397.jpg");
            byte[] imageByte = new byte[fileinfo.Length];

            using (FileStream fs = fileinfo.OpenRead())
            {
                fs.Read(imageByte, 0, imageByte.Length);
            }

            employee.PersonalPhoto = imageByte;
           
            


            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                employeeServices.Add(employee);
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
            EmployeeModel employee = new EmployeeModel();
            employee.ID = 3 ;
            employee.Fullname = "Unit Test Update";
            employee.PhoneNumber = "+96777272727";
            employee.Address = "Al-Asbahi St.";
            employee.DOB = DateTime.Now;
            employee.Country = "Yemen";
            employee.Email = "Abdulrahman893111334@gmail.com";
            employee.Gender = "male";
            employee.Status = "divorced";
            employee.Position = "Not Worker";

            FileInfo fileinfo = new FileInfo("C:\\Users\\alwani\\Downloads\\Wallpaper\\18397.jpg");
            byte[] imageByte = new byte[fileinfo.Length];

            using (FileStream fs = fileinfo.OpenRead())
            {
                fs.Read(imageByte, 0, imageByte.Length);
            }

            employee.PersonalPhoto = imageByte;


            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                employeeServices.Update(employee);
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

            EmployeeModel employee = new EmployeeModel();
            employee.ID = 2;

            bool opeartionSucceeded = false;
            string dataAccessJsonStr = string.Empty;
            string formattedJsonStr = string.Empty;

            try
            {
                employeeServices.Remove(employee);
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
