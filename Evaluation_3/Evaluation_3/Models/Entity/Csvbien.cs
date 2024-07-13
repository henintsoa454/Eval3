using System;
using System.Collections.Generic;

namespace Evaluation_3.Models.Entity
{
    public partial class Csvbien
    {
        public int Id { get; set; }
        public string Reference { get; set; } = null!;
        public string Nom { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Region { get; set; } = null!;
        public double LoyerMensuel { get; set; }
        public string Proprietaire { get; set; } = null!;
    }
}
