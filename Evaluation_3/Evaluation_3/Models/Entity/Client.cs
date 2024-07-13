using System;
using System.Collections.Generic;

namespace Evaluation_3.Models.Entity
{
    public partial class Client
    {
        public Client()
        {
            Locations = new HashSet<Location>();
        }

        public int Id { get; set; }
        public string? Nom { get; set; }
        public string Email { get; set; } = null!;

        public virtual ICollection<Location> Locations { get; set; }
    }
}
