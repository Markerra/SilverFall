namespace Game
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Runtime.Intrinsics.X86;

    static class GameManager
    {
        public static Player Player = new Player(new Stats(), "Player");
    }

    static class GameRandom
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

        public void Equip(Equipment equipment)
        {
            string slot = equipment.slot;
            Equipment? prevEq = Equipment[slot];
            if (prevEq != null) { Inventory.Add(prevEq); }
            Equipment[slot] = equipment;
            GameLog.Write($"{Name} equiped {equipment} in [{slot}] slot");
        }

        public void Equip(string name)
        {
            Equipment? eq = (Equipment?)Inventory.Find(item => item.Name == name);
            if (eq != null) { Equip(eq); }
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

    static class GameLog
    {
        private static bool firstEdit = true;
        private static int logNumber = 1;
        public static string Path = $"logs\\log{logNumber}.log";
        public static void Write(string text)
        {
            if (!GameOptions.isDev) { return; }

            if (!Directory.Exists("logs")) { Directory.CreateDirectory("logs"); }

            // Changing log number to not overwrite existing log
            
            while (File.Exists(Path) && firstEdit)
            {
                logNumber++;
                Path = $"logs\\log{logNumber}.log";
            }

            firstEdit = false;

            // Create a new file if log doesn't exist.
            if (!File.Exists(Path) ) { File.WriteAllText(Path, $"<{DateTime.Now}> {text}"); }
            // Add a new line to an existing log file.
            else { File.AppendAllText(Path, $"\n<{DateTime.Now}> {text}"); }
        }
        
        public static string ItemListToString(List<Item> loot)
        {
            if (loot == null || loot.Count == 0) return "(none)";
            return string.Join(", ", loot.Select(item => item?.Name ?? "Unknown Item"));
        }
    }

}