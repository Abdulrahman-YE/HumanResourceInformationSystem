using DomainLayer.Models.Location;
using System.Collections.Generic;


namespace ServiceLayer.Services.LocationServices
{
    public interface ILocationRepository
    {
        void Add(ILocationModel locationModel);
        void Update(ILocationModel locationModel);
        void Remove(ILocationModel locationModell);
        IEnumerable<LocationModel> GetAll();
        LocationModel GetByID(int deaprtmentId);

    }
}
