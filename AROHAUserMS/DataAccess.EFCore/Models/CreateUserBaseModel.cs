using System.ComponentModel.DataAnnotations;

namespace AROHAUserMS.DataAccess.EFCore.Models
{
    public class CreateUserBaseModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        //[Required]
        //[Url]
        //public string ConfirmUrl { get; set; }

        [Required]
        public virtual string Password { get; set; }
    }
}
