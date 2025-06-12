namespace Game.UI
{
    using System;
    using System.Timers;

    static class Generic
    {
        public static void PressAnyKey()
        {
            Console.WriteLine(Localization.Get("UI_PressAnyKey"));
            Console.ReadKey(true);
        }
    }

    static class MainMenu
    {

        public static void Show()
        {
            GameManager.Save = GameSave.LoadGame();
            Localization.UpdateLanguage();
            if (GameManager.Save.Stage >= 0) // If player started new game - show Continue button
            {
                Console.Clear();
                Console.WriteLine($"======={GameInfo.Name}=======\n");
                Console.WriteLine($"1. {Localization.Get("MainMenuContinue")}");
                Console.WriteLine($"2. {Localization.Get("MainMenuNewGame")}");
                Console.WriteLine($"3. {Localization.Get("MainMenuOptions")}");
                Console.WriteLine($"4. {Localization.Get("MainMenuExit")}");
                Console.Write(Localization.Get("UI_SelectOption"));
                string input = Console.ReadKey().KeyChar.ToString();
                switch (input)
                {
                    case "1":
                        // Show loading dots
                        Console.Clear();
                        Console.Write(Localization.Get("Loading"));
                        Timer loading = LoadingDots(60);
                        loading.Start();
                        Thread.Sleep(300); // Simulate loading
                        loading.Stop();
                        CreateSaveFile();
                        GameSave.LoadGame();
                        break;
                    case "2":
                        // Show loading dots
                        Console.Clear();
                        Console.WriteLine(" ////////////////\n//   WARNING   //\n////////////////");
                        Console.WriteLine("\n Are you sure you want to reset all your progress and start a new game? (Y/N)");
                        Console.Write(" >> ");
                        string confirm = Console.ReadKey().KeyChar.ToString();
                        if (confirm.Equals("Y", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.Clear();
                            Console.Write(Localization.Get("UI_LoadingNewGame"));
                            Timer loading2 = LoadingDots(100);
                            loading2.Start();
                            Thread.Sleep(600); // Simulate loading
                            loading2.Stop();
                            CreateSaveFile();
                        }
                        else
                        {
                            Show();
                        }
                        break;
                    case "3":
                        Options();
                        break;
                    case "4":
                        Console.WriteLine("\nExiting the game. Cya!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\n" + Localization.Get("UI_InvalidOption"));
                        Generic.PressAnyKey();
                        Show();
                        break;
                }
            }
            else // No game save detected - default menu
            {
                Console.Clear();
                Console.WriteLine($"======={GameInfo.Name}=======\n");
                Console.WriteLine($"1. {Localization.Get("MainMenuNewGame")}");
                Console.WriteLine($"2. {Localization.Get("MainMenuOptions")}");
                Console.WriteLine($"3. {Localization.Get("MainMenuExit")}");
                Console.Write(Localization.Get("UI_SelectOption"));
                string input = Console.ReadKey().KeyChar.ToString();
                switch (input)
                {
                    case "1":
                        // Show loading dots
                        Console.Clear();
                        Console.Write(Localization.Get("UI_LoadingNewGame"));
                        Timer loading = LoadingDots(100);
                        loading.Start();
                        Thread.Sleep(600); // Simulate loading
                        loading.Stop();
                        CreateSaveFile();
                        break;
                    case "2": Options(); break;
                    case "3":
                        Console.WriteLine("\nExiting the game. Cya!");
                        Environment.Exit(0); break;
                    default:
                        Console.WriteLine("\n " + Localization.Get("UI_InvalidOption"));
                        Generic.PressAnyKey();
                        Show();
                        break;
                }
            }
        }

        public static void Options()
        {
            Console.Clear();
            Console.WriteLine($"{Localization.Get("MainMenuOptions")}:");
            Console.WriteLine($"1. {Localization.Get("OptionsSetSeed")} ({GameOptions.setSeed})");
            Console.WriteLine($"2. {Localization.Get("OptionsDevMode")} ({GameOptions.isDev})");
            Console.WriteLine($"3. {Localization.Get("OptionsLang")} ({GameOptions.lang})");
            if (GameOptions.isDev)
            {
                Console.WriteLine($"-----------Dev-----------");
                Console.WriteLine($"D. Delete your save file");
                Console.WriteLine($"E. Test Feature");
                Console.WriteLine($"-----------Dev-----------");

            }
            Console.WriteLine($"4. {Localization.Get("UI_Return")}");
            Console.Write(Localization.Get("UI_SelectOption"));
            string input = Console.ReadKey(true).KeyChar.ToString();
            switch (input)
            {
                case "1":
                    GameOptions.setSeed = !GameOptions.setSeed;
                    Options();
                    break;
                case "2":
                    GameOptions.isDev = !GameOptions.isDev;
                    Console.Clear(); Console.Write(Localization.Get("UI_Wait"));
                    Timer loading = LoadingDots(40);
                    loading.Start();
                    GameManager.Save.SaveGame();
                    Thread.Sleep(120); // Simulate loading
                    loading.Stop();
                    Options();
                    break;
                case "3":
                    Console.Clear();
                    Console.WriteLine("1. English");
                    Console.WriteLine("2. Русский");
                    string langIndex = Console.ReadKey(true).KeyChar.ToString();
                    switch (langIndex)
                    {
                        case "1":
                            GameOptions.lang = "en";
                            break;
                        case "2":
                            GameOptions.lang = "ru";
                            break;
                        default:
                            Console.WriteLine("\n\n " + Localization.Get("UI_InvalidOption"));
                            Generic.PressAnyKey();
                            Options();
                            break;
                    }
                    GameManager.Save.SaveGame();
                    Localization.UpdateLanguage();
                    Show();
                    break;
                case "4":
                    Show();
                    break;
                case "D":
                    if (!GameOptions.isDev) { Options(); }
                    GameSave.DeleteSave();
                    break;
                case "E":
                    if (!GameOptions.isDev) { Options(); }
                    TestFeature.Run();
                    break;
                default:
                    Console.WriteLine("\n\n " + Localization.Get("UI_InvalidOption"));
                    Generic.PressAnyKey();
                    Options();
                    break;
            }
        }

        public static void CreateSaveFile()
        {
            Console.Clear();
            Console.WriteLine($"(#1) {Localization.Get("CreatingSaveProgress")}");
            Console.Write($"\n{Localization.Get("CreatingSaveEnterName")}");
            string name = Console.ReadLine() ?? "";
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.Write($"{Localization.Get("CreatingSaveEmptyNameError")} \n{Localization.Get("CreatingSaveEnterName")}");
                name = Console.ReadLine() ?? "";
            }

            Console.Clear();
            Console.WriteLine($"(#2) {Localization.Get("CreatingSaveProgress")}");
            int seed = GameRandom.GenerateSeed();
            if (GameOptions.setSeed)
            {
                Console.Write(Localization.Get("CreatingSaveEnterSeed"));
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    while (!int.TryParse(input, out seed))
                    {
                        Console.Clear();
                        Console.WriteLine($"(#2) {Localization.Get("CreatingSaveProgress")}");
                        Console.Write($"{Localization.Get("UI_InvalidNumberEntered")}\n{Localization.Get("CreatingSaveEnterSeed")}");
                        input = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            seed = GameRandom.GenerateSeed();
                            break;
                        }
                    }
                }
            }

            // Seed animation
            int seedsGen = 0;
            int maxSeeds = 50;
            Console.Write($"{Localization.Get("CreatingSaveGeneratingSeed")} ");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            while (seedsGen < maxSeeds)
            {
                seed = GameRandom.GenerateSeed();
                Console.SetCursorPosition(left, top);
                Console.Write(seed + "   "); // Extra spaces to clear previous numbers
                Thread.Sleep(30);
                seedsGen++;
            }
            Console.SetCursorPosition(0, top + 1);
            Console.Clear();
            Console.WriteLine($"(#2) {Localization.Get("CreatingSaveProgress")}");
            Console.WriteLine($"{Localization.Get("CreatingSaveYourSeed")} {seed}");
            Generic.PressAnyKey();
            Console.Clear();

            // Show loading dots
            Console.Write($"(#3) {Localization.Get("CreatingSaveProgress")}");
            Timer loading = LoadingDots(100);
            loading.Start();

            GameManager.Stage = 0;
            Player player = new Player(new Stats(), name);
            GameSave save = new GameSave(name, seed, player, GameManager.Stage, GameOptions.isDev);

            Thread.Sleep(800);
            loading.Stop();

            save.StartNewGame();
            save.SaveGame();


        }

        public static Timer LoadingDots(int interval = 500)
        {
            string loading = "";
            Timer timer = new Timer(interval);
            timer.Elapsed += delegate
            {
                loading = loading + ".";
                if (loading == "...") { loading = ""; }
                Console.Write(loading);
            };
            return timer;
        }
    }

    static class Battle
    {
        public static void ShowBattleInfo(Game.Battle battle)
        {
            Console.WriteLine("- Battle -");
            Console.WriteLine(Localization.Get("BattleInfo", battle.Attacker.Name, battle.Target.Name));
            Console.WriteLine(Localization.Get("BattleAttackerInfo", battle.Attacker.Name,
            Localization.Get("BattleEntityStats", battle.Attacker.Stats.Health, battle.Attacker.Stats.Mana)
            + $" {Localization.Get("BattleEntityStatsHint", 'A')}" ));
            Console.WriteLine(Localization.Get("BattleTargetInfo", battle.Target.Name,
            Localization.Get("BattleEntityStats", battle.Attacker.Stats.Health, battle.Attacker.Stats.Mana)
            + $" {Localization.Get("BattleEntityStatsHint", 'E')}" ));
        }
    }

    static class Tutorial
    {
        public static void Show()
        {
            Console.Clear();
            if (GameManager.Stage != 0) { return; }
            Console.WriteLine("Press any button to start. \n >> ");
            Console.ReadKey(false);
            Console.Clear();
            GameLog.Write("UI: Started tutorial");

            // Code
        }

    }
}