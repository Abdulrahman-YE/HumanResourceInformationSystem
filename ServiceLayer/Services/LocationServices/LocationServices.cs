using DomainLayer.Models.Location;
using ServiceLayer.CommonServices;
using System;
using System.Collections.Generic;


namespace ServiceLayer.Services.LocationServices
{
    public class LocationServices : ILocationServices
    {
        private ILocationRepository locationRepository;
        private IModelDataAnnotationCheck modelDataAnnotationCheck;

        public LocationServices(ILocationRepository locationRepository, IModelDataAnnotationCheck modelDataAnnotationCheck)
        {
            this.locationRepository = locationRepository;
            this.modelDataAnnotationCheck = modelDataAnnotationCheck;

        }

        public void Add(ILocationModel locationModel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LocationModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public LocationModel GetByID(int locationId)
        {
            throw new NotImplementedException();
        }

        public void Remove(ILocationModel locationModell)
        {
            throw new NotImplementedException();
        }

        public void Update(ILocationModel locationModel)
        {
            throw new NotImplementedException();
        }

        public void ValidateModelDataAnnotations(ILocationModel locationModel)
        {
            this.modelDataAnnotationCheck.ValidateModelDataAnnotations(locationModel);
        }
    }
}
