using System;

namespace AROHAUserMS.DataAccess.EFCore.Models
{
    public class BaseModel
    {
        public Guid CreatedUserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedUserId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
