namespace Lunar.Core.World.Actor
{
    public class Stats
    {
        private int _health;
        private int _strength;
        private int _intelligence;
        private int _dexterity;
        private int _defense;
        private int _maximumHealth;

        public int Health
        {
            get => _health;
            set => _health = value;
        }

        public int MaximumHealth
        {
            get => _maximumHealth;
            set => _maximumHealth = value;
        }

        public int Strength
        {
            get => _strength;
            set => _strength = value;
        }

        public int Intelligence
        {
            get => _intelligence;
            set => _intelligence = value;
        }

        public int Dexterity
        {
            get => _dexterity;
            set => _dexterity = value;
        }

        public int Defense
        {
            get => _defense;
            set => _defense = value;
        }
    }
}
