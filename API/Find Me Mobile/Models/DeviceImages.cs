using System.ComponentModel.DataAnnotations;

namespace Find_Me_Mobile.Models
{
    public class DeviceImages
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public string DeviceId { get; set; }
        public virtual Devices Device { get; set; }
    }
}
