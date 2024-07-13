using System;
using System.Collections.Generic;

namespace Evaluation_3.Models.Entity
{
    public partial class Location
    {
        public int Id { get; set; }
        public string Referencebien { get; set; } = null!;
        public int Idclient { get; set; }
        public int Idbien { get; set; }
        public int Duree { get; set; }
        public DateTime Datedebut { get; set; }
        public int Ispayed { get; set; }

        public virtual Bien IdbienNavigation { get; set; } = null!;
        public virtual Client IdclientNavigation { get; set; } = null!;
        public virtual Bien ReferencebienNavigation { get; set; } = null!;
    }
}
