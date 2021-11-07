using System;
using System.ComponentModel.DataAnnotations;

namespace Find_Me_Mobile.Models
{
    public class Devices
    {
        [Required]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Price { get; set; }
        public string Category { get; set; }
        public string DeviceURL { get; set; }
        public string Image { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdationDate { get; set; }
        public string CompanyId { get; set; }
        public virtual Companies Company { get; set; }
        public virtual DeviceDetails DeviceDetails { get; set; }
    }
}
