namespace Game
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Globalization;
    using System.Resources;

    static class GameManager
    {
        public static Player Player = new Player(new Stats(true), "Player");
        public static GameSave Save = new GameSave();
        public static Battle? Battle;
        public static int Stage = -1;
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

    class GameSave
    {
        public string Name { get; set; }
        public int Seed { get; set; }
        public Player Player { get; set; }
        public int Stage { get; set; } // Current stage of the game

        public bool isDev { get; set; }
        public string language { get; set; }
        public string? Hash { get; set; } // Integrity hash

        private static string fileName = "game_save";

        public GameSave()
        {
            Name = "EmptySave";
            Seed = 0;
            Player = new Player(new Stats(true), "Player");
            Stage = GameManager.Stage;
            isDev = GameOptions.isDev;
            language = GameOptions.lang;
        }
        public GameSave(string name, int seed, Player player, int stage, bool devMode)
        {
            Name = name;
            Seed = seed;
            Player = player;
            Stage = stage;
            isDev = devMode;
            language = GameOptions.lang;
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
                GameLog.Write($"Extracting hash from '{fileName}.json'");
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
                    GameLog.Write($"Computed hash for '{fileName}.json'");
                }
                if (hash == null || hash != computedHash) { return false; }
                else { return true; }
            }
        }

        public void SaveGame()
        {
            Seed = GameRandom.Seed;
            Player = GameManager.Player;
            Stage = GameManager.Stage;
            isDev = GameOptions.isDev;
            language = GameOptions.lang;
            var options = new JsonSerializerOptions { WriteIndented = true };
            string? originalHash = Hash;
            Hash = string.Empty;
            string jsonWithoutHash = JsonSerializer.Serialize(this, options);
            Hash = ComputeHash(jsonWithoutHash);
            string jsonWithHash = JsonSerializer.Serialize(this, options);
            Hash = originalHash; // Restore
            if (!Directory.Exists("saves")) { Directory.CreateDirectory("saves"); }
            File.WriteAllText($"saves\\{fileName}.json", jsonWithHash);
            GameLog.Write($"Saved game progress to '{fileName}.json' file");
        }

        public static GameSave LoadGame()
        {
            GameLog.Write("Loading game process started");
            string filePath = $"saves\\{fileName}.json";
            if (!File.Exists(filePath))
            {
                new GameSave().SaveGame();
                GameLog.Write("Loading game process failed. Save file not found");
                throw new FileNotFoundException("Save file not found. Restart the game.\n", filePath);
            }
            string json = File.ReadAllText(filePath);
            GameSave? save = JsonSerializer.Deserialize<GameSave>(json);
            if (save == null) { throw new FileLoadException("Save file has not loaded succesfully"); }
            GameOptions.isDev = save.isDev;
            GameOptions.lang = save.language;
            Console.WriteLine($"Dev Mode: {GameOptions.isDev}");
            Console.WriteLine($"Language: {GameOptions.lang}");
            if (IsValidSave(json) || GameOptions.isDev)
            {
                GameLog.Write("Loading game process finished.");
                Console.WriteLine("Passed integrity check");
                return save;
            }
            else
            {
                GameLog.Write("Loading game process failed. Save file integrity check failed");
                DeleteSave(backup: true);
                new GameSave().SaveGame();
                Thread.Sleep(10);
                throw new InvalidDataException("Game save file integrity check failed. The file may have been tampered with. Restart you game.");
            }
        }

        public void StartNewGame()
        {
            GameRandom.SetSeed(Seed);
            GameManager.Player = Player;
            GameManager.Save = this;
            GameManager.Stage = Stage;
            UI.Tutorial.Show();
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
                if (backup)
                {
                    GameLog.Write($"Creating backup of file '{fileName}.json'");
                    File.Copy(filePath, $"saves\\{fileName}_backup.json", true);
                }
                File.Delete(filePath);
                GameLog.Write("Deleted current save file");
            }
        }
    }

    static class GameOptions
    {
        public static bool setSeed;
        public static bool isDev;
        public static string lang = "en";
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

            // changing log number to not overwrite existing log

            while (File.Exists(Path) && firstEdit)
            {
                logNumber++;
                Path = $"logs\\log{logNumber}.log";
            }

            firstEdit = false;

            // create a new file if log doesn't exist.
            if (!File.Exists(Path)) { File.WriteAllText(Path, $"<{DateTime.Now}> {text}"); }
            // add a new line to an existing log file.
            else { File.AppendAllText(Path, $"\n<{DateTime.Now}> {text}"); }
        }

        public static string ItemListToString(List<Item> loot)
        {
            if (loot == null || loot.Count == 0) return "(none)";
            return string.Join(", ", loot.Select(item => item?.Name ?? "Unknown Item"));
        }
    }

    public static class Localization
    {
        private static ResourceManager _rm = new ResourceManager("SilverFall.Resources.Lang", typeof(Localization).Assembly);

        public static string Get(string key, params object[] args)
        {
            string value = _rm.GetString(key, CultureInfo.CurrentUICulture) ?? key;
            return args.Length > 0 ? string.Format(value, args) : value;
        }
        public static void UpdateLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(GameOptions.lang);
        }
    }

}