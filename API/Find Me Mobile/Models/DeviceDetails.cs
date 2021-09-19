using System.ComponentModel.DataAnnotations;

namespace Find_Me_Mobile.Models
{
    public class DeviceDetails
    {
        [Required]
        public string Id { get; set; }
        public string OperatingSystem { get; set; }
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
        public string Camera { get; set; }
        public string Features { get; set; }
        public string WLAN { get; set; }
        public string Bluetooth { get; set; }
        public string GPS { get; set; }
        public string USB { get; set; }
        public string NFC { get; set; }
        public string FM { get; set; }
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
        public string Ram { get; set; }
        public string Rom { get; set; }
        public string Charging { get; set; }
        public string Processor { get; set; }
        public string Screen { get; set; }
        public string Type { get; set; }
        public string TouchScreen { get; set; }
        public string Aperture { get; set; }
        public string Flash { get; set; }
        public string SceneModes { get; set; }
        public string WiFi { get; set; }
        public string OTG { get; set; }
        public string SIMSlotType { get; set; }
        public string StandbyMode { get; set; }
        public string Fingerprint { get; set; }
        public string Accelerometer { get; set; }
        public string AmbientLightSensor { get; set; }
        public string ProximitySensor { get; set; }
        public string ECompass { get; set; }
        public string GyroscopeSensor { get; set; }
        public string AudioPlayback { get; set; }
        public string VideoPlayback { get; set; }
        public string VideoRecording { get; set; }
        public string VoiceRecording { get; set; }
        public string Location { get; set; }
        public string DeviceId { get; set; }
        public virtual Devices Device { get; set; }
    }
}
