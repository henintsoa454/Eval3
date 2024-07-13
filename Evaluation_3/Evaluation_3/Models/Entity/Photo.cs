using System;
using System.Collections.Generic;

namespace Evaluation_3.Models.Entity
{
    public partial class Photo
    {
        public Photo()
        {
            Photobiens = new HashSet<Photobien>();
        }

        public int Id { get; set; }
        public string Path { get; set; } = null!;

        public virtual ICollection<Photobien> Photobiens { get; set; }
    }
}
