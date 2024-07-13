using System;
using System.Collections.Generic;

namespace Evaluation_3.Models.Entity
{
    public partial class Typebien
    {
        public Typebien()
        {
            Biens = new HashSet<Bien>();
        }

        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public double Commission { get; set; }

        public virtual ICollection<Bien> Biens { get; set; }
    }
}
