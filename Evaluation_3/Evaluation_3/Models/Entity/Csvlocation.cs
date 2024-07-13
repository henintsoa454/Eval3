using System;
using System.Collections.Generic;

namespace Evaluation_3.Models.Entity
{
    public partial class Csvlocation
    {
        public int Id { get; set; }
        public string Reference { get; set; } = null!;
        public DateTime DateDebut { get; set; }
        public int DureeMois { get; set; }
        public string Client { get; set; } = null!;
    }
}
