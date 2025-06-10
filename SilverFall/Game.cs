namespace Game
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Runtime.Intrinsics.X86;

    public static class GameRandom
    {
        public static int Seed = new Random().Next(0, int.MaxValue);
        public static Random Instance = new Random(Seed);
        public static int GenerateSeed()
        {
            int seed = new Random().Next(0, int.MaxValue);
            return seed;
        }
        public static void SetSeed(int seed)
        {
            Seed = seed;
            Instance = new Random(seed);
        }
        public static int GetSeed()
        {
            return Seed;
        }
    }

    class Entity
    {
        public string Name { get; set; }
        public int Experience { get; set; } = 0;
        public int[] ExpTable { get; set; } = [0, 800, 2400, 5000, 8000, 13000, 19500, 27000, 34000, 43000];
        public int Level { get; set; } // Level increases all stats by 11% per level
        public Stats Stats { get; set; }
        public Dictionary<string, Equipment?> Equipment { get; set; }
        public List<Spell> SpellBook { get; set; }
        public List<Item> Inventory { get; set; }
        public event Action<Entity>? OnDeath;
        public event Action? OnLevelUp;
        public Entity(Stats stats, string name, int level)
        {
            Stats = stats;
            Name = name;
            Level = level;
            Equipment = new Dictionary<string, Equipment?>
            {
                { "Head", null }, // 0
                { "Chest", null }, // 1
                { "Legs", null }, // 2
                { "Feet", null }, // 3
                { "Hands", null }, // 4
                { "Necklace", null }, // 5
                { "Ring", null }, // 6
                { "Weapon", null }, // 7
                { "Other", null } // 8
            };
            Inventory = new List<Item>();
            SpellBook = new List<Spell>();
        }

        public void TakeDamage(float damage, Entity attacker)
        {
            Stats.Health -= Math.Max(0, damage);
            if (Stats.Health == 0)
            {
                OnDeath?.Invoke(attacker);
            }
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

        public void AddItem(Item item)
        {
            Inventory.Add(item);
            item.Owner = this;
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

    }

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

        public WeaponStats(int damage, float critChance, float critMult, Stats? bonusStats) : base(0, 0, 0, 0, 0, 0, 0, critChance, 0)
        {
            Damage = damage;
            CritChance = critChance;
            CritMultiplier = critMult;
            if (bonusStats != null)
            {
                this.AddStats(bonusStats);
            }
        }
    }

    class GameSave
    {
        public string Name { get; set; }
        public int Seed { get; set; }
        public Entity Player { get; set; }
        public int Stage { get; set; } // Current stage of the game

        public bool isDev { get; set; }
        public string? Hash { get; set; } // Integrity hash

        private static string fileName = "game_save";

        public GameSave()
        {
            Name = "Player";
            Seed = 0;
            Player = new Player(new Stats(), Name);
            Stage = GameStage.CurrentStage;
            isDev = GameOptions.isDev;
        }
        public GameSave(string name, int seed, Entity player, int stage, bool devMode)
        {
            Name = name;
            Seed = seed;
            Player = player;
            Stage = stage;
            isDev = devMode;
        }

        // Check if save file isn't corrupted and valid by hash

        private string ComputeHash(string json)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool IsValidSave(string json)
        {
            // Extract hash
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;
                string? hash = root.TryGetProperty("Hash", out var hashProp) ? hashProp.GetString() : null;
                // Remove hash for verification
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (dict != null && dict.ContainsKey("Hash"))
                    dict["Hash"] = string.Empty;
                string jsonWithoutHash = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
                string computedHash = string.Empty;
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonWithoutHash);
                    byte[] hashBytes = sha256.ComputeHash(bytes);
                    computedHash = Convert.ToBase64String(hashBytes);
                }
                if (hash == null || hash != computedHash) { return false; }
                else { return true; }
            }
        }

        public void SaveGame()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string? originalHash = Hash;
            Hash = string.Empty;
            string jsonWithoutHash = JsonSerializer.Serialize(this, options);
            Hash = ComputeHash(jsonWithoutHash);
            string jsonWithHash = JsonSerializer.Serialize(this, options);
            Hash = originalHash; // Restore
            if (!Directory.Exists("saves")) { Directory.CreateDirectory("saves"); }
            File.WriteAllText($"saves\\{fileName}.json", jsonWithHash);
        }

        public static GameSave LoadGame()
        {
            string filePath = $"saves\\{fileName}.json";
            if (!File.Exists(filePath))
            {
                new GameSave().SaveGame();
                throw new FileNotFoundException("Save file not found. Restart the game.\n", filePath);
            }
            string json = File.ReadAllText(filePath);
            GameSave? save = JsonSerializer.Deserialize<GameSave>(json);
            if (save == null) { throw new FileLoadException("Save file has not loaded succesfully"); }
            GameOptions.isDev = save.isDev;
            Console.WriteLine($"Dev Mode: {GameOptions.isDev}");
            if (IsValidSave(json) || GameOptions.isDev)
            {
                Console.WriteLine("Passed integrity check");
                return save;
            }
            else
            {
                DeleteSave(backup: true);
                new GameSave().SaveGame();
                throw new InvalidDataException("Game save file integrity check failed. The file may have been tampered with. Restart you game.");
            }
        }

        public static void StartNewGame(GameSave saveFile)
        {
            GameRandom.SetSeed(saveFile.Seed);
            Console.WriteLine("Starting new game..");
        }

        public static void DeleteSave(bool backup = false)
        {
            string filePath = $"saves\\{fileName}.json";
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Save file not found.", filePath);
            }
            else
            {
                if (backup) { File.Copy(filePath, $"saves\\{fileName}_backup.json", true); }
                File.Delete(filePath);
            }
        }
    }

    static class GameStage
    {
        public static int CurrentStage = -1;
    }

    static class GameOptions
    {
        public static bool setSeed;
        public static bool isDev;


    }

    static class GameInfo
    {
        public static string Name = "SILVERFALL"; 
    }

}