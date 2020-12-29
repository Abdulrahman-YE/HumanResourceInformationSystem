using DomainLayer.Models.Location;
using ServiceLayer.Services.LocationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.DataAccess.Repositories.Specific.Location
{
    class LocationRepository : ILocationRepository
    {
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
    }
}
