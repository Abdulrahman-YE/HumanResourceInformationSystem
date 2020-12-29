using DomainLayer.Models.Location;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ServicesLayer.Test.DepartmentTests
{
    [Trait("Catagory", "Model Validation")]
    /// <summary>
    /// <code>DepartmentServicesValidationTests</code> is used to perform unit test to services layer specifically fofr <code>DepartmentServices</code>
    /// </summary>
    public class DepartmentsServicesValidationTests : IClassFixture<DepartmentServicesFixture>
    {
        private readonly ITestOutputHelper testOutputHelper;
        private DepartmentServicesFixture departmentServicesFixture;

        public DepartmentsServicesValidationTests(DepartmentServicesFixture departmentServicesFixture, ITestOutputHelper testOutputHelper)
        {
            this.departmentServicesFixture = departmentServicesFixture;
            this.testOutputHelper = testOutputHelper;

            SetValidSampleValues();

        }

        [Fact]
        public void ShouldNotThrowsExceptionForDefualtTestValuesOnAnnotations()
        {
            var exception =
                Record.Exception(() => this.departmentServicesFixture.DepartmentServices.ValidateModelDataAnnotations(
                    this.departmentServicesFixture.DepartmentModel));

            Assert.Null(exception);

            WriteExceptionTestResult(exception);
        }

        [Fact]
        public void ShouldThrowsExceptionIfAnyDataAnnotationCheckFails()
        {

            this.departmentServicesFixture.DepartmentModel.DepartmentName = "Human 121Res";
            this.departmentServicesFixture.DepartmentModel.PhoneNumber = "99-9273-283ss12";

            Exception exception =
                Assert.Throws<ArgumentException>(testCode: () => this.departmentServicesFixture.DepartmentServices.ValidateModelDataAnnotations(
                    this.departmentServicesFixture.DepartmentModel));

            WriteExceptionTestResult(exception);

        }

        [Fact]
        public void ShouldThrowsExceptionIfDepartmentNameIsEmpty()
        {
            this.departmentServicesFixture.DepartmentModel.DepartmentName = "";


            Exception exception =
                Assert.Throws<ArgumentException>(testCode: () => this.departmentServicesFixture.DepartmentServices.ValidateModelDataAnnotations(
                    this.departmentServicesFixture.DepartmentModel));

            WriteExceptionTestResult(exception);
        }

        [Fact]
        public void ShouldThrowsExceptionIfPhoneNumberIsEmpty()
        {
            this.departmentServicesFixture.DepartmentModel.PhoneNumber = "";


            Exception exception =
                Assert.Throws<ArgumentException>(testCode: () => this.departmentServicesFixture.DepartmentServices.ValidateModelDataAnnotations(
                    this.departmentServicesFixture.DepartmentModel));

            WriteExceptionTestResult(exception);
        }

       


        private void WriteExceptionTestResult(Exception exception)
        {

            if(exception != null)
            {
                this.testOutputHelper.WriteLine(exception.Message);
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                JObject json = JObject.FromObject(this.departmentServicesFixture.DepartmentModel);

                stringBuilder.Append("========== No Exception Was Thrown ==========").AppendLine();

                foreach (JProperty jProperty in json.Properties())
                {
                    stringBuilder.Append(jProperty.Name).Append(" ---> ").Append(jProperty.Value).AppendLine();
                }

                this.testOutputHelper.WriteLine(stringBuilder.ToString());

            }
        }

        private void SetValidSampleValues()
        {
            this.departmentServicesFixture.DepartmentModel.DepartmentName = "Human Resources Department";
            this.departmentServicesFixture.DepartmentModel.PhoneNumber = "+12-772-313-088";
            this.departmentServicesFixture.DepartmentModel.ManagerID = 10;
        }

       
    }
}
