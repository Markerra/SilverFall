namespace Game
{

    using Game.Items;

    class Weapon : Equipment
    {
        public WeaponStats Stats { get; set; }
        public Weapon(string name, string description, string rarity, WeaponStats stats) : base(name, description, rarity, "Weapon", stats)
        {
            this.Stats = stats;
        }

        public virtual void OnAttack(Entity attacker, Entity target)
        {
            // Default: no special effect
        }

        public void GetWeaponInfo()
        {
            Console.WriteLine($"{Name} \n{Description} \nStats: {Stats} \nCrit Mult: {Stats.CritMultiplier}x");
        }
    }

    class Sword : Weapon
    {
        public Sword(string name, string description, string rarity, WeaponStats stats) : base(name, description, rarity, stats) {}

        public override void OnAttack(Entity attacker, Entity target)
        {
            // Default: no special effect
        }
    }

    class Bow : Weapon
    {
        public Bow(string name, string description, string rarity, WeaponStats stats) : base(name, description, rarity, stats) {}

        public override void OnAttack(Entity attacker, Entity target)
        {
            // Default: no special effect
        }
        
    }
    
    class HeavyWeapon : Weapon
    {
        public HeavyWeapon(string name, string description, string rarity, WeaponStats stats) : base(name, description, rarity, stats) {}

        public override void OnAttack(Entity attacker, Entity target)
        {
            // Default: no special effect
        }
    }

    class MagicWand : Weapon
    {
        public MagicWand(string name, string description, string rarity, WeaponStats stats) : base(name, description, rarity, stats) { }

        public override void OnAttack(Entity attacker, Entity target)
        {
            // Default: no special effect
        }

        public void CastSpell(Spell spell)
        {
            
        }
    }

   static class WeaponsList
    {

        public static List<Weapon> Weapons { get; set; } = new List<Weapon>
        {
            Database.ShortSword,
            Database.Katana,
            Database.Claymore,
            Database.Excalibur,
            Database.WoodenBow,
            Database.ShortBow,
            Database.ElvenBow,
            Database.Longbow
Zz        };


        public static Weapon GetRandomWeapon(bool RarityCheck = true)
        {
            if (RarityCheck)
            {
                while (true)
                {
                    Weapon weapon = Weapons[GameRandom.Instance.Next(0, Weapons.Count)];
                    if (GameRandom.Instance.Next(0, 100) < Item.RarityChances[weapon.Rarity])
                    {
                        return weapon;
                    }
                }
            }
            else
            {
                return Weapons[GameRandom.Instance.Next(0, Weapons.Count)];
            }
        }

        public static Weapon GetRandomWeaponByRarity(string rarity)
        {
            List<Weapon> filteredWeapons = Weapons.FindAll(item => item.Rarity == rarity);
            if (filteredWeapons.Count == 0)
            {
                throw new ArgumentException($"No weapons found with rarity: {rarity}");
            }
            return filteredWeapons[GameRandom.Instance.Next(0, filteredWeapons.Count)];
        }

        public static List<Weapon> GetRandomWeapons(int amount, bool RarityCheck = true)
        {
            if (amount <= 0 || amount > Weapons.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be between 1 and the total number of items.");
            }

            List<Weapon> randomWeapons = new List<Weapon>();


            for (int i = 0; i < amount;)
            {
                if (RarityCheck)
                {
                    var weapon = Weapons[GameRandom.Instance.Next(0, Weapons.Count)];
                    if (GameRandom.Instance.Next(0, 100) < Item.RarityChances[weapon.Rarity])
                    {
                        randomWeapons.Add(weapon);
                        i++;
                    }
                }
                else
                {
                    Weapon weapon = Weapons[GameRandom.Instance.Next(0, Weapons.Count)];
                    randomWeapons.Add(weapon);
                    i++;
                }
            }

            return randomWeapons;
        }

        public static Weapon? GetWeaponByIndex(int index)
        {
            if (index < 0 || index >= Weapons.Count)
            {
                return null;
            }
            return Weapons[index];
        }

        public static Weapon? GetWeaponByName(string name)
        {
            return Weapons.Find(item => item.Name == name);
        }
    }
}