using Newtonsoft.Json.Linq;
using System;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ServicesLayer.Test.LocationTests
{
    [Trait("Catagory", "Model Validation")]
    public class LocationServicesValidationTests : IClassFixture<LocationServicesFixture>
    {
        private readonly ITestOutputHelper testOutputHelper;
        private LocationServicesFixture locationServicesFixture;

        public LocationServicesValidationTests(LocationServicesFixture locationServicesFixture
                                                , ITestOutputHelper testOutputHelper)
        {
            this.locationServicesFixture = locationServicesFixture;
            this.testOutputHelper = testOutputHelper;

            SetValidSampleValues();

        }

        [Fact]
        public void ShouldNotThrowExceptionForDefaultTestValuesOnAnnotations()
        {
            var exception =
                Record.Exception(() => this.locationServicesFixture.LocationServices.ValidateModelDataAnnotations(
                    this.locationServicesFixture.LocationModel));

            Assert.Null(exception);

            WriteExceptionTestResult(exception);
        }



        [Fact]
        public void ShouldThrowExceptionIfAnyDataAnnotationCheckFails()
        {
            this.locationServicesFixture.LocationModel.LocationName = "1276";
            this.locationServicesFixture.LocationModel.Country = "Yem2en";

            Exception exception =
                Assert.Throws<ArgumentException>(testCode: () => this.locationServicesFixture.LocationServices.ValidateModelDataAnnotations(
                    this.locationServicesFixture.LocationModel));
        }


        [Fact]
        public void ShouldThrowExceptionForLocationNameIsEmpty()
        {
            this.locationServicesFixture.LocationModel.LocationName = "";

            Exception exception =
                Assert.Throws<ArgumentException>(testCode: () => this.locationServicesFixture.LocationServices.ValidateModelDataAnnotations(
                     this.locationServicesFixture.LocationModel));

            WriteExceptionTestResult(exception);

        }

        [Fact]
        public void ShouldThrowExceptionForCountryIsEmpty()
        {
            this.locationServicesFixture.LocationModel.Country = "";

            Exception exception =
                Assert.Throws<ArgumentException>(testCode: () => this.locationServicesFixture.LocationServices.ValidateModelDataAnnotations(
                     this.locationServicesFixture.LocationModel));

            WriteExceptionTestResult(exception);

        }

        [Fact]
        public void ShouldThrowExceptionForCityIsEmpty()
        {
            this.locationServicesFixture.LocationModel.City = "";

            Exception exception =
                Assert.Throws<ArgumentException>(testCode: () => this.locationServicesFixture.LocationServices.ValidateModelDataAnnotations(
                     this.locationServicesFixture.LocationModel));

            WriteExceptionTestResult(exception);

        }

        [Fact]
        public void ShouldThrowExceptionForStreetAddressIsEmpty()
        {
            this.locationServicesFixture.LocationModel.StreetAddress = "";

            Exception exception =
                Assert.Throws<ArgumentException>(testCode: () => this.locationServicesFixture.LocationServices.ValidateModelDataAnnotations(
                     this.locationServicesFixture.LocationModel));

            WriteExceptionTestResult(exception);

        }

        private void SetValidSampleValues()
        {
            this.locationServicesFixture.LocationModel.LocationName = "Sana'a Pranch";
            this.locationServicesFixture.LocationModel.Country = "Yemen";
            this.locationServicesFixture.LocationModel.City = "Sana'a";
            this.locationServicesFixture.LocationModel.StreetAddress = "Haddah.St";
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
                JObject json = JObject.FromObject(this.locationServicesFixture.LocationModel);
                stringBuilder.Append("========== No Exception Was Thrown ==========").AppendLine();

                foreach(JProperty jProperty in json.Properties())
                {
                    stringBuilder.Append(jProperty.Name).Append(" ---> ").Append(jProperty.Value).AppendLine();
                }

                this.testOutputHelper.WriteLine(stringBuilder.ToString());
            }
        }
    }
}
