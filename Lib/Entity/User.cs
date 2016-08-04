using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Guid Guid { get; set; }
        public bool IsAdmin { get; set; }
        public bool Deleted { get; set; }

    }
}
