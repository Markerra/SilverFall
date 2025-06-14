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

        public static void ClearLines(int lines)
        {
            for (int i = 0; i < lines; i++) { Console.WriteLine(new string(' ', Console.WindowWidth)); }
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
                Console.Write("\n" + Localization.Get("UI_SelectOption"));
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
                Console.Write("\n" + Localization.Get("UI_SelectOption"));
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
            Console.Write("\n" + Localization.Get("UI_SelectOption"));
            string input = Console.ReadKey(true).KeyChar.ToString().ToUpper();
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
                    GameLog.Write("Dev mode enabled");
                    GameManager.Save.SaveGame();
                    Thread.Sleep(120); // Simulate loading
                    loading.Stop();
                    Options();
                    break;
                case "3":
                    Console.Clear();
                    Console.WriteLine("Select your language / Выберите язык");
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
            Player player = new Player(new Stats(false), name);
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
        public static int optionsLine;
        public static int attackerInfoLine;
        public static int targetInfoLine;
        public static int countdownLine;
        public static int inventoryLine;

        private static string TimerInput(string defaultValue, Func<bool> isTimeUp)
        {
            string input = defaultValue;
            // Non-blocking input loop
            while (!isTimeUp())
            {
                if (Console.KeyAvailable)
                {
                    input = Console.ReadKey(true).KeyChar.ToString().ToUpper();
                    break;
                }
                Thread.Sleep(10);
            }
            return input;
        }
        private static Game.Battle.BattleAction _showKnigtOptions(Entity attacker, Func<bool> isTimeUp)
        {
            Weapon? weapon = attacker.Equipment.Weapon;
            Shield? shield = attacker.Equipment.Offhand;

            if (weapon == null && shield == null)
            {
                Console.WriteLine($"3. {Localization.Get("BattleActionInventory")}");
                Console.WriteLine($"4. {Localization.Get("BattleActionEquipment")}");
                Console.WriteLine($"5. {Localization.Get("BattleActionNone")}");
            }
            else if (weapon != null && shield == null)
            {
                Console.WriteLine($"1. {Localization.Get("BattleActionAttack", weapon.Name)}");
                Console.WriteLine($"3. {Localization.Get("BattleActionInventory")}");
                Console.WriteLine($"4. {Localization.Get("BattleActionEquipment")}");
                Console.WriteLine($"5. {Localization.Get("BattleActionNone")}");
            }
            else if (weapon == null && shield != null)
            {
                Console.WriteLine($"2. {Localization.Get("BattleActionAttack", shield.Name, 999)}"); // <<< добавить в конце шанс блока
                Console.WriteLine($"3. {Localization.Get("BattleActionInventory")}");
                Console.WriteLine($"4. {Localization.Get("BattleActionEquipment")}");
                Console.WriteLine($"5. {Localization.Get("BattleActionNone")}");
            }
            else if (weapon != null && shield != null)
            {
                Console.WriteLine($"1. {Localization.Get("BattleActionAttack", weapon.Name)}");
                Console.WriteLine($"2. {Localization.Get("BattleActionAttack", shield.Name, 999)}"); // <<< добавить в конце шанс блока
                Console.WriteLine($"3. {Localization.Get("BattleActionInventory")}");
                Console.WriteLine($"4. {Localization.Get("BattleActionEquipment")}");
                Console.WriteLine($"5. {Localization.Get("BattleActionNone")}");
            }

            while (true)
            {
                int errorLine = countdownLine - 2; // Reserve a line for action messages
                Console.Write("\n");
                Console.WriteLine(Localization.Get("UI_SelectOption"));

                string input = TimerInput("5", isTimeUp);

                switch (input)
                {
                    case "1":
                        if (weapon != null)
                        {
                            _printAction(errorLine, Localization.Get("BattleActionAttackPreview"));
                            return Game.Battle.BattleAction.Attack;
                        }
                        _printAction(errorLine, Localization.Get("BattleNoWeapon"));
                        break;
                    case "2":
                        if (shield != null)
                        {
                            _printAction(errorLine, Localization.Get("BattleActionBlockPreview"));
                            return Game.Battle.BattleAction.Block;
                        }
                        _printAction(errorLine, Localization.Get("BattleNoShield"));
                        break;
                    case "3": return Game.Battle.BattleAction.Inventory;
                    case "4": return Game.Battle.BattleAction.Equipment;
                    case "5": return Game.Battle.BattleAction.None;
                    case "A": return Game.Battle.BattleAction.ShowAttackerStats;
                    case "E": return Game.Battle.BattleAction.ShowTargetStats;
                    default:
                        _printAction(errorLine, Localization.Get("UI_InvalidOption"));
                        break;
                }
            }
        }

        private static Game.Battle.BattleAction _showArcherOptions(Entity attacker, Func<bool> isTimeUp)
        {
            Weapon? bow = attacker.Equipment.Weapon;

            if (bow == null)
            {
                Console.WriteLine($"3. {Localization.Get("BattleActionInventory")}");
                Console.WriteLine($"4. {Localization.Get("BattleActionEquipment")}");
                Console.WriteLine($"5. {Localization.Get("BattleActionNone")}");
            }
            else
            {
                Console.WriteLine($"1. {Localization.Get("BattleActionAttack", bow.Name)}");
                // Here add try to evade enemy's Attack
                Console.WriteLine($"3. {Localization.Get("BattleActionInventory")}");
                Console.WriteLine($"4. {Localization.Get("BattleActionEquipment")}");
                Console.WriteLine($"5. {Localization.Get("BattleActionNone")}");
            }

            while (true)
            {
                int errorLine = countdownLine - 2; // Reserve a line for action messages
                Console.Write("\n");
                Console.WriteLine(Localization.Get("UI_SelectOption"));

                string input = TimerInput("5", isTimeUp);

                switch (input)
                {
                    case "1":
                        if (bow != null)
                        {
                            _printAction(errorLine, Localization.Get("BattleActionAttackPreview"));
                            return Game.Battle.BattleAction.Attack;
                        }
                        _printAction(errorLine, Localization.Get("BattleNoWeapon"));
                        break;
                    case "3": return Game.Battle.BattleAction.Inventory;
                    case "4": return Game.Battle.BattleAction.Equipment;
                    case "5": return Game.Battle.BattleAction.None;
                    case "A": return Game.Battle.BattleAction.ShowAttackerStats;
                    case "E": return Game.Battle.BattleAction.ShowTargetStats;
                    default:
                        _printAction(errorLine, Localization.Get("UI_InvalidOption"));
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
                Console.WriteLine($"3. {Localization.Get("BattleActionInventory")}");
                Console.WriteLine($"4. {Localization.Get("BattleActionEquipment")}");
                Console.WriteLine($"5. {Localization.Get("BattleActionNone")}");
            }
            else if (spellBook.SelectedSpell == null)
            {
                Console.WriteLine($"2. {Localization.Get("BattleActionSelectSpell", spellBook.Name)}");
                Console.WriteLine($"3. {Localization.Get("BattleActionInventory")}");
                Console.WriteLine($"4. {Localization.Get("BattleActionEquipment")}");
                Console.WriteLine($"5. {Localization.Get("BattleActionNone")}");
            }
            else
            {
                //Cast {spellBook.SelectedSpell.Name} using {wand.Name}
                Console.WriteLine($"1. {Localization.Get("BattleActionCastSpell", spellBook.SelectedSpell.Name, spellBook.SelectedSpell.Stats.GetManaCost(spellBook.Owner), wand.Name)}");
                Console.WriteLine($"2. {Localization.Get("BattleActionSelectSpell", spellBook.Name)}");
                Console.WriteLine($"3. {Localization.Get("BattleActionInventory")}");
                Console.WriteLine($"4. {Localization.Get("BattleActionEquipment")}");
                Console.WriteLine($"5. {Localization.Get("BattleActionNone")}");
            }


            while (true)
            {
                int errorLine = countdownLine - 2; // Reserve a line for action messages
                Console.Write("\n");
                Console.WriteLine(Localization.Get("UI_SelectOption"));

                string input = TimerInput("5", isTimeUp);

                switch (input)
                {
                    case "1":
                        if (spellBook?.SelectedSpell != null)
                        {
                            _printAction(errorLine, Localization.Get("BattleActionCastSpellPreview"));
                            return Game.Battle.BattleAction.Attack;
                        }
                        _printAction(errorLine, Localization.Get("BattleNoSpell"));
                        break;
                    case "2":
                        if (spellBook != null)
                        {
                            _printAction(errorLine, Localization.Get("BattleActionSelectSpellPreview"));
                            return Game.Battle.BattleAction.SelectSpell;
                        }
                        _printAction(errorLine, Localization.Get("BattleNoSpellBook"));
                        break;
                    case "3": return Game.Battle.BattleAction.Inventory;
                    case "4": return Game.Battle.BattleAction.Equipment;
                    case "5": return Game.Battle.BattleAction.None;
                    case "A": return Game.Battle.BattleAction.ShowAttackerStats;
                    case "E": return Game.Battle.BattleAction.ShowTargetStats;
                    default:
                        _printAction(errorLine, Localization.Get("UI_InvalidOption"));
                        break;
                }
            }
        }

        private static void _printAction(int errorLine, string text)
        {
            Console.SetCursorPosition(Localization.Get("UI_SelectOption").Length, errorLine);
            Console.Write(new string(' ', Console.WindowWidth)); // Clear previous action
            Console.SetCursorPosition(Localization.Get("UI_SelectOption").Length, errorLine);
            Console.Write(text.PadRight(Console.WindowWidth));
        }

        public static Game.Battle.BattleAction ShowBattleOptions(Game.Battle battle, string playerClass, Func<bool> isTimeUp)
        {
            Console.SetCursorPosition(0, optionsLine);

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
            attackerInfoLine = Console.CursorTop;
            Console.WriteLine(Localization.Get("BattleAttackerInfo", battle.Attacker.Name,
            Localization.Get("EntityStats", battle.Attacker.Stats.Health, battle.Attacker.Stats.MaxHealth,
            battle.Attacker.Stats.Mana, battle.Attacker.Stats.MaxMana)
            + $" {Localization.Get("BattleEntityStatsHint", 'A')}"));
            targetInfoLine = Console.CursorTop;
            Console.WriteLine(Localization.Get("BattleTargetInfo", battle.Target.Name,
            Localization.Get("EntityStats", battle.Target.Stats.Health, battle.Target.Stats.MaxHealth,
            battle.Target.Stats.Mana, battle.Target.Stats.MaxMana)
            + $" {Localization.Get("BattleEntityStatsHint", 'E')}"));
            Console.Write("\n");
        }

        public static void ShowAttackerInfo(Game.Battle battle)
        {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.SetCursorPosition(0, attackerInfoLine);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, attackerInfoLine);

            string advStats = Localization.Get("EntityAdvancedStats", battle.Attacker.Stats.Health, battle.Attacker.Stats.MaxHealth,
            battle.Attacker.Stats.TotalHealthRegen, battle.Attacker.Stats.Mana, battle.Attacker.Stats.MaxMana, battle.Attacker.Stats.TotalManaRegen, battle.Attacker.Stats.Defense);

            Console.WriteLine(Localization.Get("BattleAttackerInfo", battle.Attacker.Name, advStats));

            Console.SetCursorPosition(left, top);
        }

        public static void ShowTargetInfo(Game.Battle battle)
        {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.SetCursorPosition(0, targetInfoLine);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, targetInfoLine);

            string advStats = Localization.Get("EntityAdvancedStats", battle.Target.Stats.Health, battle.Target.Stats.MaxHealth,
            battle.Target.Stats.TotalHealthRegen, battle.Attacker.Stats.Mana, battle.Target.Stats.MaxMana, battle.Target.Stats.TotalManaRegen, battle.Target.Stats.Defense);

            Console.WriteLine(Localization.Get("BattleTargetInfo", battle.Target.Name, advStats));

            Console.SetCursorPosition(left, top);
        }

        public static void ShowInventory(Entity entity, Func<bool> isTimeUp)
        {
            inventoryLine = Console.CursorTop;
            while (true)
            {
                Console.SetCursorPosition(0, inventoryLine);
                Console.WriteLine($"{entity.Name}'s Inventory:");
                Console.WriteLine("(Press Esc to return)");
                for (int i = 1; i <= entity.Inventory.Count; i++)
                {
                    Item item = entity.Inventory[i - 1];
                    if (item.Action == null) { Console.WriteLine($" {i}. {item.Name}"); }
                    else { Console.WriteLine($" {i}. {item.Name} [Usable]"); }
                }
                Console.WriteLine($"\n{Localization.Get("UI_SelectOption")}");
                Console.CursorVisible = false;

                string input = TimerInput("Escape", isTimeUp);

                Console.CursorVisible = true;
                if (int.TryParse(input, out int index) && index > 0 && index <= entity.Inventory.Count)
                {
                    // Clear previous lines
                    Console.SetCursorPosition(0, inventoryLine);
                    Generic.ClearLines(3 + entity.Inventory.Count);
                    Console.SetCursorPosition(0, inventoryLine);
                    Item item = entity.Inventory[index - 1];
                    Console.WriteLine(item.Name);
                    Console.WriteLine(item.Description);
                    Console.WriteLine(item.Rarity);
                    switch (item)
                    {
                        case MagicWand magicWand: // NOT FINISHED !!
                            Console.WriteLine("Weapon stats:");
                            magicWand.Stats.PrintStats();
                            if (magicWand.BonusStats != null)
                            {
                                Console.WriteLine("Bonus stats:");
                                magicWand.Stats.PrintBonusStats();
                            }
                            Console.WriteLine(Localization.Get("BattleActions"));
                            Console.WriteLine("1. Equip");
                            Console.WriteLine("Press Esc to return");
                            break;
                        case Weapon weapon:
                            Console.WriteLine("Weapon stats:");
                            weapon.Stats.PrintStats();
                            if (weapon.BonusStats != null)
                            {
                                Console.WriteLine("Bonus stats:");
                                weapon.Stats.PrintBonusStats();
                            }
                            Console.WriteLine(Localization.Get("BattleActions"));
                            Console.WriteLine("1. Equip");
                            Console.WriteLine("Press Esc to return");
                            break;
                        case Equipment equipment:
                            Console.WriteLine("Stats:");
                            equipment.BonusStats.PrintStats();
                            Console.WriteLine(Localization.Get("BattleActions"));
                            Console.WriteLine("1. Equip");
                            Console.WriteLine("Press Esc to return");
                            break;
                        default:
                            Console.WriteLine(Localization.Get("BattleActions"));
                            if (item.Action != null)
                                Console.WriteLine("1. Use");
                            Console.WriteLine("Press Esc to return");
                            break;
                    }

                    Console.WriteLine($"\n{Localization.Get("UI_SelectOption")}");
                    string input2 = TimerInput("Escape", isTimeUp);
                    switch (input2)
                    {
                        case "1":
                            if (item is Equipment eq) { eq.Equip(); }
                            else if (item.Action != null) { item.Use(); }
                            break;
                        case "Escape": break;
                    }
                }
                else if (input == "Escape") { return; }
                else { Console.WriteLine($"\n{Localization.Get("UI_InvalidOption")}"); }
            }
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