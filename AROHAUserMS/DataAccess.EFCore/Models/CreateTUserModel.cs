namespace AROHAUserMS.DataAccess.EFCore.Models
{
    public class CreateTUserModel : CreateUserBaseModel
    {
        public string[] Roles { get; set; }
    }
}
