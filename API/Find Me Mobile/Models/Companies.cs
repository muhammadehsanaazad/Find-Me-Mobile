using System.ComponentModel.DataAnnotations;

namespace Find_Me_Mobile.Models
{
    public class Companies
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public long? ContactNumber { get; set; }
    }
}
