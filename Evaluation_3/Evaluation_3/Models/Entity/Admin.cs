using System;
using System.Collections.Generic;

namespace Evaluation_3.Models.Entity
{
    public partial class Admin
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
