using System;
using System.Collections.Generic;

namespace Evaluation_3.Models.Entity
{
    public partial class Photobien
    {
        public int Id { get; set; }
        public int Idbien { get; set; }
        public int Idphoto { get; set; }

        public virtual Bien IdbienNavigation { get; set; } = null!;
        public virtual Photo IdphotoNavigation { get; set; } = null!;
    }
}
