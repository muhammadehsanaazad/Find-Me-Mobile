using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Find_Me_Mobile.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Domain { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }
    }
}
