namespace Game
{
    class Entity
    {
        public string Name { get; set; }
        public int Experience { get; set; } = 0;
        public int[] ExpTable { get; set; } = [0, 800, 2400, 5000, 8000, 13000, 19500, 27000, 34000, 43000];
        public int Level { get; set; } // Level increases all stats by 11% per level
        public bool IsAlive = true;

        public Stats Stats { get; set; }
        public EquipmentSlots Equipment { get; set; }
        public List<Item> Inventory { get; set; }

        public event Action<Entity>? OnDeath;
        public event Action? OnLevelUp;

        public Entity()
        {
            Name = "Entity";
            Level = 1;
            Equipment = new EquipmentSlots();
            Inventory = new List<Item>();
            Stats = new Stats(true);
        }
        public Entity(Stats baseStats, string name, int level)
        {
            Name = name;
            Level = level;
            Equipment = new EquipmentSlots();
            Inventory = new List<Item>();
            Stats = baseStats;

            OnDeath += (attacker) => { IsAlive = false; };
        }

        public void TakeDamage(float damage, Entity attacker)
        {
             // Apply defense reduction
            float effectiveDamage = damage * (100 - Stats.Defense) / 100;

            // Reduce health
            Stats.Health -= effectiveDamage;
        
            // Log the damage
            GameLog.Write($"{Name} takes {effectiveDamage} damage from {attacker.Name}. (HP: {Stats.Health}/{Stats.MaxHealth})");
        
            // Check for death
            if (Stats.Health <= 0 && IsAlive)
            {
                IsAlive = false;
                GameLog.Write(Localization.Get("EntityDefeated"));
                OnDeath?.Invoke(attacker);
            }
        }

        public void Attack(Entity target)
        {
            if (Equipment.Weapon is IAttackWeapon attackWeapon) { attackWeapon.Attack(this, target); }
            else { Console.WriteLine($"{Name} has no weapon to attack with!"); }
        }

        public void Equip(Equipment equipment)
        {
            switch (equipment)
            {
                case Helmet helmet:
                    if (Equipment.Head != null) { Unequip(Equipment.Head); }
                    Equipment.Head = helmet; break;
                case Chestplate chest:
                    if (Equipment.Chest != null) { Unequip(Equipment.Chest); }
                    Equipment.Chest = chest; break;
                case Leggings legs:
                    if (Equipment.Legs != null) { Unequip(Equipment.Legs); }
                    Equipment.Legs = legs; break;
                case Boots boots:
                    if (Equipment.Feet != null) { Unequip(Equipment.Feet); }
                    Equipment.Feet = boots; break;
                case Gloves gloves:
                    if (Equipment.Hands != null) { Unequip(Equipment.Hands); }
                    Equipment.Hands = gloves; break;
                case Necklace necklace:
                    if (Equipment.Necklace != null) { Unequip(Equipment.Necklace); }
                    Equipment.Necklace = necklace; break;
                case Shield shield:
                    if (Equipment.Offhand != null) { Unequip(Equipment.Offhand); }
                    Equipment.Offhand = shield; break;
                case Weapon weapon:
                    if (Equipment.Weapon != null) { Unequip(Equipment.Weapon); }
                    Equipment.Weapon = weapon; break;
                case SpellBook spellBook:
                    if (Equipment.SpellBook != null) { Unequip(Equipment.SpellBook); }
                    Equipment.SpellBook = spellBook; break;
                default: throw new ArgumentException("Unknown equipment type");
            }
            Stats.AddStats(equipment.BonusStats);
        }

        public void Unequip(Equipment equipment)
        {
            switch (equipment)
            {
                case Helmet helmet: if(Equipment.Head == equipment) 
                { Equipment.Head = null; AddItem(equipment); Stats.RemoveStats(equipment.BonusStats); }  break;
                case Chestplate chest: if(Equipment.Chest == equipment) 
                { Equipment.Chest = null;  AddItem(equipment); Stats.RemoveStats(equipment.BonusStats); }  break;
                case Leggings legs: if(Equipment.Legs == equipment) 
                { Equipment.Legs = null; AddItem(equipment); Stats.RemoveStats(equipment.BonusStats); }  break;
                case Boots boots: if(Equipment.Feet == equipment) 
                { Equipment.Feet = null; AddItem(equipment); Stats.RemoveStats(equipment.BonusStats); }  break;
                case Gloves gloves: if(Equipment.Hands == equipment) 
                { Equipment.Hands = null;  AddItem(equipment); Stats.RemoveStats(equipment.BonusStats); }  break;
                case Necklace necklace: if(Equipment.Hands == equipment) 
                { Equipment.Hands = null;  AddItem(equipment); Stats.RemoveStats(equipment.BonusStats); }  break;
                case Shield shield: if(Equipment.Offhand == equipment) 
                { Equipment.Offhand = null;    AddItem(equipment); Stats.RemoveStats(equipment.BonusStats); }  break;
                case Weapon weapon: if(Equipment.Weapon == equipment) 
                { Equipment.Weapon = null;   AddItem(equipment); Stats.RemoveStats(equipment.BonusStats); }  break;
                case SpellBook spellBook: if(Equipment.SpellBook == equipment) 
                { Equipment.SpellBook = null;      AddItem(equipment); Stats.RemoveStats(equipment.BonusStats); }  break;
                default: throw new ArgumentException("Unknown equipment type");
            }
        }

        public void Equip(string name)
        {
            Equipment? eq = (Equipment?)Inventory.Find(item => item.Name == name);
            if (eq != null) { Equip(eq); }
        
        }

        public void Unequip(string name)
        {
            Equipment? eq = (Equipment?)Inventory.Find(item => item.Name == name);
            if (eq != null) { Unequip(eq); }
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

        public void Heal(float amount)
        {
            Stats.Health = Math.Min(Stats.Health + amount, Stats.MaxHealth);
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