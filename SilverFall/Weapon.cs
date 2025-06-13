namespace Game
{

    interface IAttackWeapon
    {
        void Attack(Entity attacker, Entity target);
    }

    class Weapon : Equipment, IAttackWeapon
    {
        public WeaponStats Stats { get; set; }
        public ICombatStyle CombatStyle { get; set; }
        public Weapon(string name, string description, string rarity, WeaponStats stats) : base(name, description, rarity, "Weapon", stats)
        {
            this.Stats = stats;
            this.CombatStyle = new KnightCombat();
        }

        public virtual void Attack(Entity attacker, Entity target)
        {
            // Default attack logic (can be overridden in subclasses)
            float damageDealt = Stats.Damage;
            if (GameRandom.Instance.Next(0, 100) < Stats.CritChance)
            {
                damageDealt *= Stats.CritMultiplier;
                Console.WriteLine($"{attacker.Name} landed a critical hit with {damageDealt} damage!");
            }
            else
            {
                Console.WriteLine($"{attacker.Name} attacked {target.Name} for {damageDealt} damage.");
            }
            target.TakeDamage(damageDealt, attacker);
            Console.WriteLine($"{target.Name} now has {target.Stats.Health} health remaining.");
        }

        public void GetWeaponInfo()
        {
            Console.WriteLine($"{Name} \n{Description} \nStats: {Stats} \nCrit Mult: {Stats.CritMultiplier}x");
        }
    }

    class Sword : Weapon
    {

        public Sword(string name, string description, string rarity, WeaponStats stats) : base(name, description, rarity, stats)
        {
            CombatStyle = new KnightCombat();
        }

        public override void Attack(Entity attacker, Entity target)
        {
            // Default attack logic (can be overridden in subclasses)
            // Example:
            float damageDealt = Stats.Damage;
            if (GameRandom.Instance.Next(0, 100) < Stats.CritChance)
            {
                damageDealt *= Stats.CritMultiplier;
                Console.WriteLine($"{attacker.Name} landed a critical hit with {damageDealt} damage!");
            }
            else
            {
                Console.WriteLine($"{attacker.Name} attacked {target.Name} for {damageDealt} damage.");
            }
            target.TakeDamage(damageDealt, attacker);
            Console.WriteLine($"{target.Name} now has {target.Stats.Health} health remaining.");
        }
    }
    
    class HeavyWeapon : Weapon
    {
        public HeavyWeapon(string name, string description, string rarity, WeaponStats stats) : base(name, description, rarity, stats)
        {
            CombatStyle = new KnightCombat();
        }

        public override void Attack(Entity attacker, Entity target)
        {
            // Default attack logic (can be overridden in subclasses)
            // Example:
            float damageDealt = Stats.Damage;
            if (GameRandom.Instance.Next(0, 100) < Stats.CritChance)
            {
                damageDealt *= Stats.CritMultiplier;
                Console.WriteLine($"{attacker.Name} landed a critical hit with {damageDealt} damage!");
            }
            else
            {
                Console.WriteLine($"{attacker.Name} attacked {target.Name} for {damageDealt} damage.");
            }
            target.TakeDamage(damageDealt, attacker);
            Console.WriteLine($"{target.Name} now has {target.Stats.Health} health remaining.");
        }
    }

    class Bow : Weapon
    {
        public Bow(string name, string description, string rarity, WeaponStats stats) : base(name, description, rarity, stats)
        {
            CombatStyle = new ArcherCombat();
        }

        public override void Attack(Entity attacker, Entity target)
        {
            // Default attack logic (can be overridden in subclasses)
            // Example:
            float damageDealt = Stats.Damage;
            if (GameRandom.Instance.Next(0, 100) < Stats.CritChance)
            {
                damageDealt *= Stats.CritMultiplier;
                Console.WriteLine($"{attacker.Name} landed a critical hit with {damageDealt} damage!");
            }
            else
            {
                Console.WriteLine($"{attacker.Name} attacked {target.Name} for {damageDealt} damage.");
            }
            target.TakeDamage(damageDealt, attacker);
            Console.WriteLine($"{target.Name} now has {target.Stats.Health} health remaining.");
        }

    }

    class MagicWand : Weapon
    {
        private static float _showSpellComboPct = 35; // % Of showing spell combo in UI when casting
        public MagicWand(string name, string description, string rarity, WeaponStats stats) : base(name, description, rarity, stats)
        {
            CombatStyle = new MagicianCombat();
        }
        
        public override void Attack(Entity attacker, Entity target)
        {
            // Assume attacker has a SpellBook and a selected spell
            var spellBook = attacker.Equipment.SpellBook;
            Spell? spell = spellBook?.SelectedSpell;
            
            if (spellBook == null) { throw new NullReferenceException("Can't cast a spell when SpellBook is null"); }
            if (spell == null) { spellBook.SelectSpell(); }
            else
            {
                if (CastSpell(attacker, target, spell))
                {
                    // Calculate spell damage
                    float baseDamage = spell.Stats.Damage;
                    float intBonus = attacker.Stats.SpellAmplify;
                    float totalDamage = baseDamage + intBonus;

                    Console.WriteLine($"{attacker.Name} casts {spell.Name} for {totalDamage} damage.");

                    // Apply damage to target
                    target.TakeDamage(totalDamage, attacker);
                }
            }
        }

        public bool CastSpell(Entity attacker, Entity target, Spell spell)
        {
            SpellBook? spellBook = attacker.Equipment.SpellBook;
            if (spellBook == null) { Console.WriteLine("You don't have a spell book equiped!"); return false; }
            List<ConsoleKey> input = new List<ConsoleKey>();
            bool failed = false;

            // Some AI generated stuff that i don't understand
            List<char> openKeys = new List<char>();
            int openKeysAmount = (int)Math.Round(spell.KeyCombo.Length * (_showSpellComboPct / 100));
            for (int i = 0; i < openKeysAmount;)
            {
                ConsoleKey randomKey = spell.KeyCombo[GameRandom.Instance.Next(0, spell.KeyCombo.Length)];
                char keyChar;
                if (randomKey >= ConsoleKey.A && randomKey <= ConsoleKey.Z)
                    keyChar = (char)('A' + (randomKey - ConsoleKey.A));
                else if (randomKey >= ConsoleKey.D0 && randomKey <= ConsoleKey.D9)
                    keyChar = (char)('0' + (randomKey - ConsoleKey.D0));
                else
                    keyChar = randomKey.ToString()[0];
                if (!openKeys.Contains(keyChar)) { openKeys.Add(keyChar); i++; }
            }

            string displayCombo = "";

            string spellCastingTitle = "- Spell Casting -";
            string spellCastingStatus;

            // Build the display string: show revealed keys, hide others with '-'
            void updateDisplayCombo()
            {
                for (int i = 0; i < spell.KeyCombo.Length; i++)
                {
                    char keyChar;
                    if (spell.KeyCombo[i] >= ConsoleKey.A && spell.KeyCombo[i] <= ConsoleKey.Z)
                        keyChar = (char)('A' + (spell.KeyCombo[i] - ConsoleKey.A));
                    else if (spell.KeyCombo[i] >= ConsoleKey.D0 && spell.KeyCombo[i] <= ConsoleKey.D9)
                        keyChar = (char)('0' + (spell.KeyCombo[i] - ConsoleKey.D0));
                    else
                        keyChar = spell.KeyCombo[i].ToString()[0];

                    if (openKeys.Contains(keyChar))
                        displayCombo += keyChar + " ";
                    else
                        displayCombo += "- ";
                }
                spellCastingStatus = Localization.Get(
                    "spellCastingStatus",
                    spell.Name,
                    displayCombo.Trim(),
                    target.Name,
                    target.Level
                );
            }

            updateDisplayCombo();

            Console.WriteLine(spellCastingTitle);
            Console.WriteLine(spellCastingStatus);

            for (int i = 0; i < spell.KeyCombo.Length; i++)
            {
                if (attacker.Stats.Mana < spell.Stats.ManaCost)
                {
                    Console.WriteLine("Not enough mana!");
                    return false;
                }
                var key = Console.ReadKey(true);
                ConsoleKey consoleKey = key.Key;
                input.Add(consoleKey);
                openKeys.Add(key.KeyChar);
                attacker.Stats.Mana -= spell.Stats.ManaCost;
                GameLog.Write($"{attacker.Name} spent {spell.Stats.ManaCost} mana");
                if (consoleKey != spell.KeyCombo[i])
                {
                    Console.WriteLine("Wrong character spelled! ");
                    GameLog.Write($"CastSpell() for {Name} failed: Wrong key pressed. ({consoleKey} pressed, {spell.KeyCombo[i]} expected)");
                    failed = true;
                    break;
                }
                Console.Clear();
                displayCombo = "";
                updateDisplayCombo();
                spellCastingStatus = $"You're casting \"{spell.Name}\" [{displayCombo.Trim()}] on {target.Name} ({target.Level} LVL)";
                Console.WriteLine(spellCastingTitle);
                Console.WriteLine(spellCastingStatus);
            }

            if (!failed)
            {
                spell.Cast(attacker, target);
                return true;
            }

            GameLog.Write("CastSpell() failed: No matching spell found.");
            return false;
        }
    }

   static class WeaponsList
        {

            public static List<Weapon> Weapons { get; set; } = new List<Weapon>
            {
                Items.Database.ShortSword,
                Items.Database.Katana,
                Items.Database.Claymore,
                Items.Database.Excalibur,
                Items.Database.WoodenBow,
                Items.Database.ShortBow,
                Items.Database.ElvenBow,
                Items.Database.Longbow,
                Items.Database.MagicWand,
            };


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