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
                string input = Console.ReadKey(true).KeyChar.ToString();
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
                string input = Console.ReadKey(true).KeyChar.ToString();
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
            Console.CursorVisible = false;
            while (seedsGen < maxSeeds)
            {
                seed = GameRandom.GenerateSeed();
                Console.SetCursorPosition(left, top);
                Console.Write(seed + "           "); // Extra spaces to clear previous numbers
                Thread.Sleep(30);
                seedsGen++;
            }
            Console.CursorVisible = true;
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
        private static Game.Battle.BattleAction _showKnigtOptions(Entity attacker, Func<bool> isTimeUp)
        {
            Weapon? weapon = attacker.Equipment.Weapon;
            Shield? shield = attacker.Equipment.Offhand;

            if (weapon == null && shield == null)
            {
                Console.WriteLine("3. Open inventory");
                Console.WriteLine("4. Open equipment");
                Console.WriteLine("5. Do nothing (Skip timer)");
            }
            else if (weapon != null && shield == null)
            {
                Console.WriteLine($"1. Attack using {weapon.Name}");
                Console.WriteLine("3. Open inventory");
                Console.WriteLine("4. Open equipment");
                Console.WriteLine("5. Do nothing (Skip timer)");
            }
            else if (weapon == null && shield != null)
            {
                Console.WriteLine($"2. Try to block using {shield.Name} ({shield})"); // <<< добавить в конце шанс блока
                Console.WriteLine("3. Open inventory");
                Console.WriteLine("4. Open equipment");
                Console.WriteLine("5. Do nothing (Skip timer)");
            }
            else if (weapon != null && shield != null)
            {
                Console.WriteLine($"1. Attack using {weapon.Name}");
                Console.WriteLine($"2. Try to block using {shield.Name} ({shield})"); // <<< добавить в конце шанс блока
                Console.WriteLine("3. Open inventory");
                Console.WriteLine("4. Open equipment");
                Console.WriteLine("5. Do nothing (Skip timer)");
            }

            // Waiting for user Input
            int errorTop = Console.CursorTop + 1; // Reserve a line for error messages

            while (true)
            {
                Console.SetCursorPosition(0, errorTop);
                Console.Write(new string(' ', Localization.Get("UI_SelectOption").Length)); // Clear previous error
                Console.SetCursorPosition(0, errorTop - 1); // Move back to input line

                Console.Write("\n");
                Console.WriteLine(Localization.Get("UI_SelectOption"));

                string input = "5";
                // Non-blocking input loop
                while (!isTimeUp())
                {
                    if (Console.KeyAvailable)
                    {
                        input = Console.ReadKey(true).KeyChar.ToString();
                        break;
                    }
                    Thread.Sleep(10);
                }

                switch (input)
                {
                    case "1":
                        if (weapon != null)
                            return Game.Battle.BattleAction.Attack;
                        Console.SetCursorPosition(0, errorTop);
                        Console.Write(Localization.Get("BattleNoWeapon").PadRight(Console.WindowWidth));
                        break;
                    case "2":
                        if (shield != null)
                            return Game.Battle.BattleAction.Block;
                        Console.SetCursorPosition(Localization.Get("UI_SelectOption").Length, errorTop);
                        Console.Write(Localization.Get("BattleNoShield").PadRight(Console.WindowWidth));
                        break;
                    case "3": return Game.Battle.BattleAction.Inventory;
                    case "4": return Game.Battle.BattleAction.Equipment;
                    case "5": return Game.Battle.BattleAction.None;
                    default:
                        Console.SetCursorPosition(Localization.Get("UI_SelectOption").Length, errorTop);
                        Console.Write(Localization.Get("UI_InvalidOption").PadRight(Console.WindowWidth));
                        break;
                }
            }
        }

        private static Game.Battle.BattleAction _showArcherOptions(Entity attacker, Func<bool> isTimeUp)
        {
            Weapon? bow = attacker.Equipment.Weapon;

            if (bow == null)
            {
                Console.WriteLine("2. Open inventory");
                Console.WriteLine("3. Open equipment");
                Console.WriteLine("4. Do nothing (Skip timer)");
            }
            else
            {
                Console.WriteLine($"1. Attack using {bow.Name}");
                Console.WriteLine("2. Open inventory");
                Console.WriteLine("3. Open equipment");
                Console.WriteLine("4. Do nothing (Skip timer)");
            }

            // Waiting for user Input
            int errorTop = Console.CursorTop + 1; // Reserve a line for error messages

            while (true)
            {
                Console.SetCursorPosition(0, errorTop);
                Console.Write(new string(' ', Console.WindowWidth)); // Clear previous error
                Console.SetCursorPosition(0, errorTop - 1); // Move back to input line

                Console.Write("\n");
                Console.WriteLine(Localization.Get("UI_SelectOption"));
                
                string input = "5";
                // Non-blocking input loop
                while (!isTimeUp())
                {
                    if (Console.KeyAvailable)
                    {
                        input = Console.ReadKey(true).KeyChar.ToString();
                        break;
                    }
                    Thread.Sleep(10);
                }

                switch (input)
                {
                    case "1":
                        if (bow != null)
                            return Game.Battle.BattleAction.Attack;
                        Console.SetCursorPosition(0, errorTop);
                        Console.Write(Localization.Get("BattleNoWeapon").PadRight(Console.WindowWidth));
                        break;
                    case "2": return Game.Battle.BattleAction.Inventory;
                    case "3": return Game.Battle.BattleAction.Equipment;
                    case "4": return Game.Battle.BattleAction.None;
                    default:
                        Console.SetCursorPosition(0, errorTop);
                        Console.Write(Localization.Get("UI_InvalidOption").PadRight(Console.WindowWidth));
                        break;
                }
            }
        }

        private static Game.Battle.BattleAction _showMagicianOptions(Entity attacker, Func<bool> isTimeUp)
        {
            Weapon? wand = attacker.Equipment.Weapon;
            SpellBook? spellBook = attacker.Equipment.SpellBook;

            if (wand == null || spellBook == null)
            {
                Console.WriteLine("3. Open inventory");
                Console.WriteLine("4. Open equipment");
                Console.WriteLine("5. Do nothing (Skip timer)");
            }
            else if (spellBook.SelectedSpell == null)
            {
                Console.WriteLine($"2. Select a spell from {spellBook.Name}");
                Console.WriteLine("3. Open inventory");
                Console.WriteLine("4. Open equipment");
                Console.WriteLine("5. Do nothing (Skip timer)");
            }
            else
            {
                Console.WriteLine($"1. Cast {spellBook.SelectedSpell.Name} using {wand.Name}");
                Console.WriteLine($"2. Select a spell from {spellBook.Name}");
                Console.WriteLine("3. Open inventory");
                Console.WriteLine("4. Open equipment");
                Console.WriteLine("5. Do nothing (Skip timer)");
            }

            // Waiting for user Input
            int errorTop = Console.CursorTop + 1; // Reserve a line for error messages

            while (true)
            {
                Console.SetCursorPosition(0, errorTop);
                Console.Write(new string(' ', Console.WindowWidth)); // Clear previous error
                Console.SetCursorPosition(0, errorTop - 1); // Move back to input line

                Console.Write("\n");
                Console.WriteLine(Localization.Get("UI_SelectOption"));
                
                string input = "5";
                // Non-blocking input loop
                while (!isTimeUp())
                {
                    if (Console.KeyAvailable)
                    {
                        input = Console.ReadKey(true).KeyChar.ToString();
                        break;
                    }
                    Thread.Sleep(10);
                }

                switch (input)
                {
                    case "1":
                        if (spellBook?.SelectedSpell != null)
                            return Game.Battle.BattleAction.CastSpell;
                        Console.SetCursorPosition(0, errorTop);
                        Console.Write(Localization.Get("BattleNoSpell").PadRight(Console.WindowWidth));
                        break;
                    case "2":
                        if (spellBook != null)
                            return Game.Battle.BattleAction.SelectSpell;
                        Console.SetCursorPosition(0, errorTop);
                        Console.Write(Localization.Get("BattleNoSpellBook").PadRight(Console.WindowWidth));
                        break;
                    case "3": return Game.Battle.BattleAction.Inventory;
                    case "4": return Game.Battle.BattleAction.Equipment;
                    case "5": return Game.Battle.BattleAction.None;
                    default:
                        Console.SetCursorPosition(0, errorTop);
                        Console.Write(Localization.Get("UI_InvalidOption").PadRight(Console.WindowWidth));
                        break;
                }
            }
        }

        public static Game.Battle.BattleAction ShowBattleOptions(Game.Battle battle, string playerClass, Func<bool> isTimeUp)
        {

            Console.WriteLine(Localization.Get("BattleActions"));

            Game.Battle.BattleAction action;

            // Show different options for different classes
            switch (playerClass)
            {
                case PlayerClasses.Knight: action = _showKnigtOptions(battle.Attacker, isTimeUp); break;
                case PlayerClasses.Archer: action = _showArcherOptions(battle.Attacker, isTimeUp); break;
                case PlayerClasses.Magician: action = _showMagicianOptions(battle.Attacker, isTimeUp); break;
                default: throw new ArgumentException($"Unknown player class: {playerClass}");
            }

            // Return an action
            return action;
        }

        public static void ShowBattleInfo(Game.Battle battle)
        {
            Console.WriteLine("- Battle -");
            Console.WriteLine(Localization.Get("BattleInfoVersus", battle.Attacker.Name, battle.Target.Name));
            Console.WriteLine(Localization.Get("BattleInfoTurn", battle.nTurn, battle.eTurn.Name));
            Console.Write("\n");
            Console.WriteLine(Localization.Get("BattleAttackerInfo", battle.Attacker.Name,
            Localization.Get("BattleEntityStats", battle.Attacker.Stats.Health, battle.Attacker.Stats.MaxHealth,
            battle.Target.Stats.Mana, battle.Target.Stats.MaxMana)
            + $" {Localization.Get("BattleEntityStatsHint", 'A')}"));
            Console.WriteLine(Localization.Get("BattleTargetInfo", battle.Target.Name,
            Localization.Get("BattleEntityStats", battle.Target.Stats.Health, battle.Target.Stats.MaxHealth,
            battle.Target.Stats.Mana, battle.Target.Stats.MaxMana)
            + $" {Localization.Get("BattleEntityStatsHint", 'E')}"));
            Console.Write("\n");
        }
    }

    static class Tutorial
    {
        public static void Show()
        {
            Console.Clear();
            if (GameManager.Stage != 0) { return; }
            Console.WriteLine("Press any button to start. \n >> ");
            Console.ReadKey(true);
            Console.Clear();
            GameLog.Write("UI: Started tutorial");

            // Code
        }

    }
}