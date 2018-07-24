namespace Lunar.Server.Utilities
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

        public bool Supercedes(string roleName)
        {
            if (Settings.Roles.ContainsKey(roleName))
            {
                return (this.Level >= Settings.Roles[roleName].Level);
            }
            else
            {
                return false;
            }
        }

        public bool Supercedes(Role role)
        {
            return (this.Level > role.Level);
        }

        // A default, permissionless role
        public static Role Default => new Role("default", int.MinValue);
    }
}
