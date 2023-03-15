using System.ComponentModel.DataAnnotations;

namespace AROHAUserMS.DataAccess.EFCore.Models
{
    public class CreateRoleModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
