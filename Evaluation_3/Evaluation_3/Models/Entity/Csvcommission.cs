using System;
using System.Collections.Generic;

namespace Evaluation_3.Models.Entity
{
    public partial class Csvcommission
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public double Commission { get; set; }
    }
}
