using System.ComponentModel.DataAnnotations;

namespace Find_Me_Mobile.ViewModels
{
    public class SignInBindingModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public SignInBindingModel()
        {
            Email = "user@findmobile.com";
            Password = "user@find";
        }
    }

    public class SignUpBindingModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }
        public long? Phone { get; set; }
        public string Address { get; set; }
    }

    public class CompaniesBindingModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ContactNumber { get; set; }
    }
}
