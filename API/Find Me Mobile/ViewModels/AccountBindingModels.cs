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

    public class DeviceBindingModel
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public string Price { get; set; }
        public string PrimaryImage { get; set; }
    }

    public class DeviceDetailsBindingModel
    {
        public string OS { get; set; }
        public string UI { get; set; }
        public string Dimensions { get; set; }
        public string Weight { get; set; }
        public string SIM { get; set; }
        public string Colors { get; set; }
        public string G2Band { get; set; }
        public string G3Band { get; set; }
        public string G4Band { get; set; }
        public string G5Band { get; set; }
        public string CPU { get; set; }
        public string Chipset { get; set; }
        public string GPU { get; set; }
        public string Technology { get; set; }
        public string Size { get; set; }
        public string Resolution { get; set; }
        public string Protection { get; set; }
        public string ExtraFeatures { get; set; }
        public string BuiltIn { get; set; }
        public string Card { get; set; }
        public string Main { get; set; }
        public string Features { get; set; }
        public string Front { get; set; }
        public string WLAN { get; set; }
        public string Bluetooth { get; set; }
        public string GPS { get; set; }
        public string USB { get; set; }
        public string NFC { get; set; }
        public string Data { get; set; }
        public string Sensors { get; set; }
        public string Audio { get; set; }
        public string Browser { get; set; }
        public string Messaging { get; set; }
        public string Games { get; set; }
        public string Torch { get; set; }
        public string Extra { get; set; }
        public string Capacity { get; set; }
        public string Price { get; set; }
        public string AverageRating { get; set; }
    }
    public class Item
    {
        public string ItemName { get; set; }
    }
}
