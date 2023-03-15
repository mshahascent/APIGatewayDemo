
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AROHAUserMS.Domain.Entities
{

    public partial class User 
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(8, ErrorMessage = "Password must be between 5 and 8 characters", MinimumLength = 5)]
        public string Password { get; set; } = null!;

        public string Companyid { get; set; } = "";

        public string Firstname { get; set; } = "";

        public string? Middlename { get; set; } = "";

        public string Lastname { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string Address { get; set; } = "";

        public string Gender { get; set; } = "";

        public DateTime? Dateofbirth { get; set; } = null;

        public string? Ssn { get; set; }= "";

        public int Createdby { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int? Updatedby { get; set; }= null!;

        public DateTime? UpdatedDate { get; set; }= null!;
    }
}
