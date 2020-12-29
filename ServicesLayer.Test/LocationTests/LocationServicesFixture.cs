using DomainLayer.Models.Location;
using ServiceLayer.CommonServices;
using ServiceLayer.Services.LocationServices;
namespace ServicesLayer.Test.LocationTests
{
    public class LocationServicesFixture
    {
        private ILocationServices locationServices;
        private ILocationModel locationModel;

        public LocationServicesFixture()
        {
            this.locationServices = new LocationServices(null, new ModelDataAnnotationCheck());
            this.locationModel = new LocationModel();
        }

        public LocationServices LocationServices
        {
            get { return (LocationServices)this.locationServices; }
            set { this.locationServices = value; }
        }

        public LocationModel LocationModel
        {
            get { return (LocationModel)this.locationModel; }
            set { this.locationModel = value; }
        }
    }
}
