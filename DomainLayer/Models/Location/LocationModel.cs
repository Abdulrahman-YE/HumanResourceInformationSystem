using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Models.Location
{
    public class LocationModel : ILocationModel
    {

        public int LocationId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Location name is required field")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Location name must be between 4 and 30 characters")]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z']*$", ErrorMessage = "Location name field must contain letters only")]
        public string LocationName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Country is required field")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Country name must be between 2 and 30 characters")]
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Country field must contain letters only")]
        public string Country { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "City is required field")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "City name must be between 2 and 30 characters")]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z']*$", ErrorMessage = "City field must contain letters only")]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Street address is required field")]
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Street address must be between 5 and 40 characters")]
        public string StreetAddress { get; set; }
    }
}
