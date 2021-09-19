using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Find_Me_Mobile.Models
{
    public class Companies
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdationDate { get; set; }
        public virtual ICollection<Devices> Devices { get; set; }
    }
}
