namespace DomainLayer.Models.Location
{
    public interface ILocationModel
    {
        string City { get; set; }
        int LocationId { get; set; }
        string LocationName { get; set; }
        string StreetAddress { get; set; }
    }
}