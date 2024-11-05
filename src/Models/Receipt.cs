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
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime BoughtAt { get; set; }
        public int TotalPrice { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string County { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}