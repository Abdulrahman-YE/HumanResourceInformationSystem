using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Role
{
    public class RoleModel : IRoleModel
    {
        public int ID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Role name is required field")]
        public string Name { get; set; }
    }
}
