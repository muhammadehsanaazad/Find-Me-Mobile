using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Find_Me_Mobile.Models
{
    public class Devices
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string CompanyId { get; set; }
        public virtual Companies Company { get; set; }
        public virtual DeviceDetails DeviceDetails { get; set; }
        public virtual ICollection<DeviceImages> DeviceImages { get; set; }
    }
}
