namespace Game
{
    class Stats
    {
        // Base stats
        //-----------------------------

        // Increases health by 10 per point, max health by 3 and health regen by 0.5 per point
        private int strength = 50;
        public int Strength
        {
            get { return strength; }
            set
            {
                strength = value;
                cachedHealth = strength * 10 + health;
            }
        }
        // Increases crit chance by 1% per point and miss chance by 1% per point
        public int Agility { get; set; } = 50;
        // Increases mana by 20 and mana regen by 0.8 per point
        public int Intelligence { get; set; } = 50;

        private float health = 70; // Base health value
        private float cachedHealth; // Cached health value
        public float Health // Base health, can be increased by Strength
        {
            get { return cachedHealth; }
            set
            {
                health = Math.Clamp(value, 0, MaxHealth);
            }
        }
        private float maxHealth = 200;
        public float MaxHealth
        {
            get { return maxHealth + (Strength * 3); }
            set { maxHealth = Math.Max(0, value); }
        }
        private float healthRegen = 3.8F; // Backing field for HealthRegen
        public float HealthRegen
        {
            get { return healthRegen + Strength * 0.5F; }
            set { healthRegen = Math.Max(0, value); }
        }

        private float mana = 250; // Base mana value

        // Mana, can be increased by Intelligence
        public float Mana
        {
            get { return Intelligence * 20 + mana; }
            set { mana = Math.Max(0, value); }
        }
        private float manaRegen = 4.2F;
        public float ManaRegen
        {
            get { return manaRegen + Intelligence * 0.8F; }
            set { manaRegen = Math.Max(0, value); }
        }

        // Base defense, can be increased by items. Absorbs a percentage of incoming damage

        private float defense { get; set; }
        public float Defense
        {
            get { return Math.Clamp(defense, 0, 100); }
            set { defense = value; }
        }

        // Additional stats

        private float missChance;
        public float MissChance
        {
            get { return missChance + Math.Min(Agility * 0.01F, 80); }
            set { missChance = Math.Clamp(value, 0, 100); }
        }

        private float critChance;
        public float CritChance
        {
            get { return critChance + (Agility * 0.01F); }
            set { critChance = Math.Clamp(value, 0, 100); }
        }

        public Stats() { }

        public Stats(int str, int agi, int intell, int hp, int hpRegen, int mana, int manaRegen, float crit, float miss)
        {
            Strength = str;
            Agility = agi;
            Intelligence = intell;
            Health = hp;
            HealthRegen = hpRegen;
            Mana = mana;
            ManaRegen = manaRegen;
            CritChance = crit;
            MissChance = miss;
        }

        public void AddStats(Stats other)
        {
            Strength += other.Strength;
            Agility += other.Agility;
            Intelligence += other.Intelligence;
            maxHealth += other.maxHealth;
            health += other.health;
            mana += other.mana;
            healthRegen += other.healthRegen;
            manaRegen += other.manaRegen;
            defense += other.defense;
        }

        public void RemoveStats(Stats other)
        {
            Strength -= other.Strength;
            Agility -= other.Agility;
            Intelligence -= other.Intelligence;
            maxHealth -= other.maxHealth;
            health -= other.health;
            mana -= other.mana;
            healthRegen -= other.healthRegen;
            manaRegen -= other.manaRegen;
            defense -= other.defense;
        }

    }

    class WeaponStats : Stats
    {
        public int Damage { get; set; }
        public float CritMultiplier { get; set; }
        public Stats? BonusStats { get; set; }

        public WeaponStats(int damage, float critChance, float critMult, Stats? bonusStats) : base(0, 0, 0, 0, 0, 0, 0, critChance, 0)
        {
            Damage = damage;
            CritChance = critChance;
            CritMultiplier = critMult;

            // Bonus stats 
            if (bonusStats != null) { BonusStats = bonusStats; }
        }
    }

    class SpellStats : Stats
    {
        public int Damage { get; set; }
        public int ManaCost { get; set; }

        public SpellStats(int damage, int manaCost) : base(0, 0, 0, 0, 0, 0, 0, 0, 0)
        {
            Damage = damage;
            ManaCost = manaCost;
        }
    }
}