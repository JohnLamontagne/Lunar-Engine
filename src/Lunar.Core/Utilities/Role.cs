using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Core.Utilities
{
    public class Role
    {
        public string Name { get; }

        public int Level { get; }

        public Role(string name, int level)
        {
            this.Name = name;
            this.Level = level;
        }

        public bool Supercedes(Role role)
        {
            return (this.Level > role.Level);
        }

        // A default, permissionless role
        public static Role Default => new Role("default", int.MinValue);
    }
}
