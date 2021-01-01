using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ServicesLayer.Test.RoleTest
{
    [Trait("Catagory", "Model Validation")]
    public class RoleServicesValidationTests : IClassFixture<RoleServicesFixture>
    {

        private readonly ITestOutputHelper testOutputHelper;
        private RoleServicesFixture roleServicesFixture;

        public RoleServicesValidationTests(RoleServicesFixture roleServicesFixture, ITestOutputHelper testOutputHelper)
        {
            this.roleServicesFixture = roleServicesFixture;
            this.testOutputHelper = testOutputHelper;

            SetValidSampleValues();

        }

        [Fact]
        public void ShouldNotThrowsExceptionForDefualtTestValuesOnAnnotations()
        {
            var exception =
                Record.Exception(() => this.roleServicesFixture.RoleServices.ValidateModelDataAnnotations(
                    this.roleServicesFixture.RoleModel));

            Assert.Null(exception);

            WriteExceptionTestResult(exception);
        }

        [Fact]
        public void ShouldThrowsExceptionIfAnyDataAnnotationCheckFails()
        {


            this.roleServicesFixture.RoleModel.Name = string.Empty;

            Exception exception =
                Assert.Throws<ArgumentException>(testCode: () => this.roleServicesFixture.RoleServices.ValidateModelDataAnnotations(
                    this.roleServicesFixture.RoleModel));

            WriteExceptionTestResult(exception);

        }

  

        private void WriteExceptionTestResult(Exception exception)
        {

            if (exception != null)
            {
                this.testOutputHelper.WriteLine(exception.Message);
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                JObject json = JObject.FromObject(this.roleServicesFixture.RoleModel);

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
            this.roleServicesFixture.RoleModel.Name = "Admin";
        }


    }
}
