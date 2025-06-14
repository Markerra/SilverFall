namespace Game
{

    class Stats
    {
        private float _health { get; set; }
        private float _mana { get; set; }
        
        // Base stats
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }

        // Current and base values
        public float Health { get { return Math.Min(_health, MaxHealth); } set { _health = value; } }
        public float BaseHealth { get; set; }
        private float HealthRegen { get; set; }
        public float Mana { get { return Math.Min(_mana, MaxMana); } set { _mana = value; } }
        public float BaseMana { get; set; }
        private float ManaRegen { get; set; }
        public float Defense { get; set; }
        public float MissChance { get; set; }
        public int SpellAmplify { get; set; }
        public float ManaCostDecrease { get; set; }

        // Calculated properties
        public float MaxHealth => BaseHealth + Strength * 3;
        public float MaxMana => BaseMana + Intelligence * 20;
        public float TotalHealthRegen => HealthRegen + Strength * 0.5f;
        public float TotalManaRegen => ManaRegen + Intelligence * 0.8f;
        public float TotalMissChance => MissChance + Math.Min(Agility * 0.01f, 80);
        public float TotalManaCostDecrease => ManaCostDecrease + (Intelligence * 0.1f);

        public Stats() {}

        public Stats(bool baseStats)
        {
            if (baseStats)
            {
                // Base player stats
                Strength = 50;
                Agility = 50;
                Intelligence = 50;
                BaseHealth = 200;
                Health = MaxHealth;
                HealthRegen = 10;
                BaseMana = 145;
                Mana = MaxMana;
                ManaRegen = 5;
                MissChance = 5;
                Defense = 0;
            }
            else
            {
                Strength = 0;
                Agility = 0;
                Intelligence = 0;
                BaseHealth = 0;
                Health = MaxHealth;
                HealthRegen = 0;
                BaseMana = 0;
                Mana = MaxMana;
                ManaRegen = 0;
                MissChance = 0;
                Defense = 0;
            }
        }

        public Stats(int str, int agi, int intell, float baseHp, float hpRegen, float baseMana, float manaRegen, float evasion, float def = 0)
        {
            Strength = str;
            Agility = agi;
            Intelligence = intell;
            BaseHealth = baseHp;
            Health = MaxHealth;
            HealthRegen = hpRegen;
            BaseMana = baseMana;
            Mana = MaxMana;
            ManaRegen = manaRegen;
            MissChance = evasion;
            Defense = def;
        }

        public void AddStats(Stats other)
        {
            Strength += other.Strength;
            Agility += other.Agility;
            Intelligence += other.Intelligence;
            BaseHealth += other.BaseHealth;
            Health += other.Health;
            BaseMana += other.BaseMana;
            Mana += other.Mana;
            HealthRegen += other.HealthRegen;
            ManaRegen += other.ManaRegen;
            Defense += other.Defense;
            SpellAmplify += other.SpellAmplify;
            ManaCostDecrease += other.ManaCostDecrease;
            MissChance += other.MissChance;
        }

        public void RemoveStats(Stats other)
        {
            Strength -= other.Strength;
            Agility -= other.Agility;
            Intelligence -= other.Intelligence;
            BaseHealth -= other.BaseHealth;
            Health -= other.Health;
            BaseMana -= other.BaseMana;
            Mana -= other.Mana;
            HealthRegen -= other.HealthRegen;
            ManaRegen -= other.ManaRegen;
            Defense -= other.Defense;
            SpellAmplify -= other.SpellAmplify;
            ManaCostDecrease -= other.ManaCostDecrease;
            MissChance -= other.MissChance;
        }

        public virtual void PrintStats()
        {
            string stats = "";
            if (Strength > 0) stats += $"STR: {Strength}\t";
            if (Agility > 0) stats += $"AGI: {Agility}\t";
            if (Intelligence > 0) stats += $"INT: {Intelligence}\n";
            if (Health > 0) stats += $"HP: {Health}\t";
            if (MaxHealth > 0) stats += $"MAXHP: {MaxHealth}\t";
            if (TotalHealthRegen > 0) stats += $"HP Regen: {TotalHealthRegen}\n";
            if (Mana > 0) stats += $"MP: {Mana}\t";
            if (Mana > 0) stats += $"MAXMP: {Mana}\t";
            if (TotalManaRegen > 0) stats += $"MP Regen: {TotalManaRegen}\n";
            if (Defense > 0) stats += $"DEF: {Defense}\t";
            if (MissChance > 0) stats += $"MISS: {MissChance:F1}%";
            Console.WriteLine(stats);
        }

        public string StatsToString()
        {
            return $"STR:{Strength}, AGI:{Agility}, INT:{Intelligence}, HP:{Health}/{MaxHealth}, HPRegen:{TotalHealthRegen}, MP:{Mana}/{MaxMana}, MPRegen:{TotalManaRegen}, DEF:{Defense}, MISS:{TotalMissChance:F1}%";
        }
    }

    class WeaponStats : Stats
    {
        public int Damage { get; set; }
        public float CritMultiplier { get; set; }
        public float CritChance { get; set; }
        public Stats? BonusStats { get; set; }
        public WeaponStats(int damage, float critChance, float critMult, Stats? bonusStats) : base(0, 0, 0, 0, 0, 0, 0, critChance, 0)
        {
            Damage = damage;
            CritChance = critChance;
            CritMultiplier = critMult;

            // Bonus stats 
            if (bonusStats != null) { BonusStats = bonusStats; }
        }

        public override void PrintStats()
        {
            Console.WriteLine($"DMG: {Damage}\tCRITMULT: {CritMultiplier}\tCRITCHANCE: {CritChance}%");
        }

        public void PrintBonusStats()
        {
            string stats = "";
            if (BonusStats?.Strength > 0) stats += $"STR: {BonusStats.Strength}\t";
            if (BonusStats?.Agility > 0) stats += $"AGI: {BonusStats.Agility}\t";
            if (BonusStats?.Intelligence > 0) stats += $"INT: {BonusStats.Intelligence}\n";
            if (BonusStats?.Health > 0) stats += $"HP: {BonusStats.Health}\t";
            if (BonusStats?.MaxHealth > 0) stats += $"MAXHP: {BonusStats.MaxHealth}\t";
            if (BonusStats?.TotalHealthRegen > 0) stats += $"HP Regen: {BonusStats.TotalHealthRegen}\n";
            if (BonusStats?.Mana > 0) stats += $"MP: {BonusStats.Mana}\t";
            if (BonusStats?.Mana > 0) stats += $"MAXMP: {BonusStats.Mana}\t";
            if (BonusStats?.TotalManaRegen > 0) stats += $"MP Regen: {BonusStats.TotalManaRegen}\n";
            if (BonusStats?.Defense > 0) stats += $"DEF: {BonusStats.Defense}\t";
            if (BonusStats?.MissChance > 0) stats += $"MISS: {BonusStats.MissChance:F1}%";
            Console.WriteLine(stats);
        }

    }

    class SpellStats
    {
        public int Damage { get; set; }
        public int ManaCost { get; set; }

        public SpellStats(int damage, int manaCost)
        {
            Damage = damage;
            ManaCost = manaCost;
        }

        public int GetManaCost(Entity? owner)
        {
            int manaCost = (int) Math.Round(ManaCost * ((100 - owner?.Stats.ManaCostDecrease ?? 0) / 100));
            return manaCost;
        }
    }

}