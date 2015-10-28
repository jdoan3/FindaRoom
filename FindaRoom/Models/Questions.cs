using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FindaRoom.Models
{
    public class Questions
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string Own_Looking { get; set; }
        public string formattedAddress { get; set; }
        public string cityLat { get; set; }
        public string cityLong { get; set; }
        public string diameter { get; set; }
        public int priceRange { get; set; }
        public string genderInterest { get; set; }
        public string occupation { get; set; }
        public DateTime moveInDate { get; set; }
        public string aboutMe { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

    }
}