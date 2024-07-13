using System;
using System.Collections.Generic;

namespace Evaluation_3.Models.Entity
{
    public partial class Proprietaire
    {
        public Proprietaire()
        {
            Biens = new HashSet<Bien>();
        }

        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public string Numerotel { get; set; } = null!;

        public virtual ICollection<Bien> Biens { get; set; }
    }
}
