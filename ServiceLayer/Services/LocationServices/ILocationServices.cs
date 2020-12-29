using DomainLayer.Models.Location;


namespace ServiceLayer.Services.LocationServices
{
    public interface ILocationServices : ILocationRepository
    {
        void ValidateModelDataAnnotations(ILocationModel locationModel);
    }
}
