namespace Game
{
    class Entity
    {
        public string Name { get; set; }
        public int Experience { get; set; } = 0;
        public int[] ExpTable { get; set; } = [0, 800, 2400, 5000, 8000, 13000, 19500, 27000, 34000, 43000];
        public int Level { get; set; } // Level increases all stats by 11% per level

        private Stats _stats { get; set; }
        private Stats _bonusStats { get; set; }
        public Stats Stats
        {
            get
            {
                // Creating copy of base stats
                var totalStats = new Stats();
                totalStats.AddStats(_stats); // Add current base stats
                totalStats.AddStats(_bonusStats); // Add current bonus stats
                return totalStats; // Return total stats
            }
            set { _stats = value; }
        }

        public EquipmentSlots Equipment { get; set; }
        public List<Item> Inventory { get; set; }
        public event Action<Entity>? OnDeath;
        public event Action? OnLevelUp;
        public bool IsAlive = true;

        public Entity()
        {
            _stats = new Stats();
            _bonusStats = new Stats();

            Stats = new Stats();
            Name = "Entity";
            Level = 1;
            Equipment = new EquipmentSlots();
            Inventory = new List<Item>();
        }
        public Entity(Stats baseStats, string name, int level)
        {
            _stats = new Stats();
            _bonusStats = new Stats();

            Stats = baseStats;
            Name = name;
            Level = level;
            Equipment = new EquipmentSlots();
            Inventory = new List<Item>();

            OnDeath += (attacker) => { IsAlive = false; };
        }

        public void TakeDamage(float damage, Entity attacker)
        {
            Stats.Health -= Math.Max(0, damage);
            if (Stats.Health == 0)
            {
                OnDeath?.Invoke(attacker);
            }
        }

        public void Equip(Equipment equipment)
        {
            switch (equipment)
            {
                case Helmet helmet:
                    if (Equipment.Head != null) { AddItem(Equipment.Head); }
                    Equipment.Head = helmet; break;
                case Chestplate chest:
                    if (Equipment.Chest != null) { AddItem(Equipment.Chest); }
                    Equipment.Chest = chest; break;
                case Leggings legs:
                    if (Equipment.Legs != null) { AddItem(Equipment.Legs); }
                    Equipment.Legs = legs; break;
                case Boots boots:
                    if (Equipment.Feet != null) { AddItem(Equipment.Feet); }
                    Equipment.Feet = boots; break;
                case Gloves gloves:
                    if (Equipment.Hands != null) { AddItem(Equipment.Hands); }
                    Equipment.Hands = gloves; break;
                case Necklace necklace:
                    if (Equipment.Necklace != null) { AddItem(Equipment.Necklace); }
                    Equipment.Necklace = necklace; break;
                case Shield shield:
                    if (Equipment.Offhand != null) { AddItem(Equipment.Offhand); }
                    Equipment.Offhand = shield; break;
                case Weapon weapon:
                    if (Equipment.Weapon != null) { AddItem(Equipment.Weapon); }
                    Equipment.Weapon = weapon; break;
                case SpellBook spellBook:
                    if (Equipment.SpellBook != null) { AddItem(Equipment.SpellBook); }
                    Equipment.SpellBook = spellBook; break;
                default: throw new ArgumentException("Unknown equipment type");
            }
        }

        public void Equip(string name)
        {
            Equipment? eq = (Equipment?)Inventory.Find(item => item.Name == name);
            if (eq != null) { Equip(eq); }
        }

        public bool EquipFirst<T>() where T : Equipment
        {
            var item = Inventory.OfType<T>().FirstOrDefault();
            if (item != null)
            {
                Equip(item);
                return true;
            }
            return false;
        }

        public void Attack(Entity target, Weapon weapon)
        {
            if (target == null)
            {
                Console.WriteLine("Target is null. Cannot attack.");
                return;
            }

            weapon.OnAttack(this, target);

            if (GameRandom.Instance.Next(0, 100) < target.Stats.MissChance)
            {
                Console.WriteLine($"{Name} missed the attack on {target.Name}!");
                return;
            }

            float damageDealt = weapon.Stats.Damage * (1 - target.Stats.Defense / 100);
            if (GameRandom.Instance.Next(0, 100) < weapon.Stats.CritChance)
            {
                damageDealt *= weapon.Stats.CritMultiplier;
                Console.WriteLine($"{Name} landed a critical hit with {damageDealt} damage!");
            }
            else
            {
                Console.WriteLine($"{Name} attacked {target.Name} for {damageDealt} damage.");
            }

            target.TakeDamage(damageDealt, this);
            Console.WriteLine($"{target.Name} now has {target.Stats.Health} health remaining.");
        }

        public void Heal(float amount)
        {
            Stats.Health += amount;
        }

        public void GiveExp(int amount)
        {
            Experience += amount;
            Console.WriteLine($"{Name} gained {amount} experience points. Total: {Experience}");

            int newLevel = Level;
            while (newLevel < ExpTable.Length && Experience >= ExpTable[newLevel])
            {
                newLevel++;
            }

            if (newLevel > Level)
            {
                for (int i = Level; i < newLevel; i++)
                {
                    LevelUp();
                }
            }
        }

        public void LevelUp()
        {
            if (Level >= ExpTable.Length - 1)
            {
                Console.WriteLine($"{Name} has reached the maximum level.");
                return;
            }

            float lvlMult = 1.11F; // Level multiplier for stats increase

            Level++;
            Stats.Strength = (int)(Stats.Strength * lvlMult);
            Stats.Agility = (int)(Stats.Agility * lvlMult);
            Stats.Intelligence = (int)(Stats.Intelligence * lvlMult);
            Stats.Health = (int)(Stats.Health * lvlMult);
            Stats.Mana = (int)(Stats.Mana * lvlMult);
            Stats.Defense = (int)(Stats.Defense * lvlMult);

            Console.WriteLine($"{Name} leveled up to level {Level}!");
            OnLevelUp?.Invoke();
        }

        public Item AddItem(Item item)
        {
            Item newItem = item;
            Inventory.Add(newItem);
            newItem.Owner = this;
            return newItem;
        }

        public void RemoveItem(Item item)
        {
            Inventory.Remove(item);
        }

        public void ShowInventory()
        {
            if (Inventory.Count == 0)
            {
                Console.WriteLine($"{Name}'s inventory is empty.");
                return;
            }

            Console.WriteLine($"{Name}'s inventory contains:");
            foreach (var item in Inventory)
            {
                if (item is Weapon weapon)
                {
                    weapon.GetWeaponInfo();
                }
                else
                {
                    Console.WriteLine($"{item.Name} \n{item.Description}");
                }
            }
        }

        public void CalculateBonusStats()
        {
            // Reset current bonus stats
            _bonusStats = new Stats();

            // Add bonus stats from each equipped item if present
            if (Equipment.Head != null) _bonusStats.AddStats(Equipment.Head.BonusStats);
            if (Equipment.Chest != null) _bonusStats.AddStats(Equipment.Chest.BonusStats);
            if (Equipment.Legs != null) _bonusStats.AddStats(Equipment.Legs.BonusStats);
            if (Equipment.Feet != null) _bonusStats.AddStats(Equipment.Feet.BonusStats);
            if (Equipment.Hands != null) _bonusStats.AddStats(Equipment.Hands.BonusStats);
            if (Equipment.Necklace != null) _bonusStats.AddStats(Equipment.Necklace.BonusStats);
            if (Equipment.Offhand != null) _bonusStats.AddStats(Equipment.Offhand.BonusStats);
            if (Equipment.Weapon != null) _bonusStats.AddStats(Equipment.Weapon.BonusStats);
            if (Equipment.SpellBook != null) _bonusStats.AddStats(Equipment.SpellBook.BonusStats);
        }

        public bool IsInBattle()
        {
            if (GameManager.Battle != null)
            {
                if (this == GameManager.Battle.Attacker || this == GameManager.Battle.Target)
                { return true; }
            }
            return false;
        }

    }

    class EquipmentSlots
    {
        public Helmet? Head { get; set; }
        public Chestplate? Chest { get; set; }
        public Leggings? Legs { get; set; }
        public Boots? Feet { get; set; }
        public Gloves? Hands { get; set; }
        public Necklace? Necklace { get; set; }
        public Shield? Offhand { get; set; }
        public Weapon? Weapon { get; set; }
        public SpellBook? SpellBook { get; set; }

        public List<Equipment>? GetAsList()
        {
            List<Equipment>? list = new List<Equipment>();
            if (Head != null) list.Add(Head);
            if (Chest != null) list.Add(Chest);
            if (Legs != null) list.Add(Legs);
            if (Feet != null) list.Add(Feet);
            if (Hands != null) list.Add(Hands);
            if (Necklace != null) list.Add(Necklace);
            if (Offhand != null) list.Add(Offhand);
            if (Weapon != null) list.Add(Weapon);
            if (SpellBook != null) list.Add(SpellBook);
            return list;
        }

    }
}