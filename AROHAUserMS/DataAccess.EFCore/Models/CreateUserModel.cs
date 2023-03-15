using System.ComponentModel.DataAnnotations;

namespace AROHAUserMS.DataAccess.EFCore.Models
{
    public class CreateUserModel : CreateUserBaseModel
    {
        [Required]
        public override string Password { get; set; }
        [Required]
        public  string Role { get; set; }

    }
}
