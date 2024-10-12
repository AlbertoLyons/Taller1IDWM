using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Bogus.DataSets;

namespace Taller_1_IDWM.src.Models
{
    public class Receipt
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public DateTime BoughtAt { get; set; }
        [Range(1, 100000000)]
        public int TotalPrice { get; set; }
        [StringLength(256, MinimumLength = 3)]
        public string Country { get; set; }
        [StringLength(256, MinimumLength = 3)]
        public string City { get; set; }
        [StringLength(256, MinimumLength = 3)]
        public string County { get; set; }
        [StringLength(256, MinimumLength = 3)]
        public string Address { get; set; }
    }
}