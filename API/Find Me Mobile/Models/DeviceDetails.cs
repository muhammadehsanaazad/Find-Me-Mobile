using System.ComponentModel.DataAnnotations;

namespace Find_Me_Mobile.Models
{
    public class DeviceDetails
    {
        [Required]
        public string Id { get; set; }
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
        [Required]
        public string DeviceId { get; set; }
        public virtual Devices Device { get; set; }
    }
}
