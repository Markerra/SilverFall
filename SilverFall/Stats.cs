namespace Game
{

    class Stats
    {
        // Base stats
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }

        // Current and base values
        public float Health { get; set; }
        public float BaseHealth { get; set; }
        public float HealthRegen { get; set; }
        public float Mana { get; set; }
        public float BaseMana { get; set; }
        public float ManaRegen { get; set; }
        public float Defense { get; set; }
        public float MissChance { get; set; }
        public float CritChance { get; set; }
        public int SpellAmplify { get; set; }
        public float ManaCostDecrease { get; set; }

        // Calculated properties
        public float MaxHealth => BaseHealth + Strength * 3;
        public float MaxMana => BaseMana + Intelligence * 20;
        public float TotalHealthRegen => HealthRegen + Strength * 0.5f;
        public float TotalManaRegen => ManaRegen + Intelligence * 0.8f;
        public float TotalMissChance => MissChance + Math.Min(Agility * 0.01f, 80);
        public float TotalCritChance => CritChance + (Agility * 0.01f);
        public float TotalManaCostDecrease => ManaCostDecrease + (Intelligence * 0.1f);

        public Stats()
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
            CritChance = 10;
            MissChance = 5;
            Defense = 0;
        }

        public Stats(int str, int agi, int intell, float baseHp, float hpRegen, float baseMana, float manaRegen, float crit, float evasion, float def = 0)
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
            CritChance = crit;
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
            CritChance += other.CritChance;
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
            CritChance -= other.CritChance;
            MissChance -= other.MissChance;
        }

        public void PrintStats()
        {
            Console.WriteLine($"STR: {Strength}\tAGI: {Agility}\tINT: {Intelligence}");
            Console.WriteLine($"HP: {Health}/{MaxHealth}\tHP Regen: {TotalHealthRegen}");
            Console.WriteLine($"MP: {Mana}/{MaxMana}\tMP Regen: {TotalManaRegen}");
            Console.WriteLine($"DEF: {Defense}\tCRIT: {TotalCritChance:F1}%\tMISS: {TotalMissChance:F1}%");
        }

        public string StatsToString()
        {
            return $"STR:{Strength}, AGI:{Agility}, INT:{Intelligence}, HP:{Health}/{MaxHealth}, HPRegen:{TotalHealthRegen}, MP:{Mana}/{MaxMana}, MPRegen:{TotalManaRegen}, DEF:{Defense}, CRIT:{TotalCritChance:F1}%, MISS:{TotalMissChance:F1}%";
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

    class SpellStats
    {
        public int Damage { get; set; }
        public int ManaCost { get; set; }

        public SpellStats(int damage, int manaCost)
        {
            Damage = damage;
            ManaCost = manaCost;
        }
    }

}