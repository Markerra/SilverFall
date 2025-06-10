namespace Game.UI
{
    using System;
    using System.Timers;

    static class MainMenu
    {

        public static void Show()
        {
            GameSave save = GameSave.LoadGame();
            if (save.Stage >= 0) // If player started new game - show Continue button
            {
                Console.Clear();
                Console.WriteLine($"======={GameInfo.Name}=======\n");
                Console.WriteLine("1. Continue");
                Console.WriteLine("2. New Game");
                Console.WriteLine("3. Options");
                Console.WriteLine("4. Exit");
                Console.Write(" >> Please select an option: ");
                string input = Console.ReadKey().KeyChar.ToString();
                switch (input)
                {
                    case "1":
                        // Show loading dots
                        Console.Clear();
                        Console.Write("Loading game");
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
                            Console.Write("Starting a new game");
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
                        Console.WriteLine("\n Invalid option. Please try again.");
                        Console.WriteLine(" >> Press any key to continue...");
                        Console.ReadKey();
                        Show();
                        break;
                }
            }
            else // No game save detected - default menu
            {
                Console.Clear();
                Console.WriteLine($"======={GameInfo.Name}=======\n");
                Console.WriteLine("1. New Game");
                Console.WriteLine("2. Load Game");
                Console.WriteLine("3. Options");
                Console.WriteLine("4. Exit");
                Console.Write(" >> Please select an option: ");
                string input = Console.ReadKey().KeyChar.ToString();
                switch (input)
                {
                    case "1":
                        // Show loading dots
                        Console.Clear();
                        Console.Write("Starting a new game");
                        Timer loading = LoadingDots(100);
                        loading.Start();
                        Thread.Sleep(600); // Simulate loading
                        loading.Stop();
                        CreateSaveFile();
                        break;
                    case "2":
                        Console.WriteLine("\nLoading game...");
                        GameSave.LoadGame();
                        break;
                    case "3":
                        Options();
                        break;
                    case "4":
                        Console.WriteLine("\nExiting the game. Cya!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\n Invalid option. Please try again.");
                        Console.WriteLine(" >> Press any key to continue...");
                        Console.ReadKey();
                        Show();
                        break;
                }
            }
        }

        public static void Options()
        {
            Console.Clear();
            Console.WriteLine("Options:");
            Console.WriteLine($"1. Set Seed Mode ({GameOptions.setSeed})");
            Console.WriteLine($"2. Dev Mode ({GameOptions.isDev})");
            if (GameOptions.isDev)
            {
                Console.WriteLine($"-----------Dev-----------");
                Console.WriteLine($"D. Delete your save file");
                Console.WriteLine($"E. Test Feature");
                Console.WriteLine($"-----------Dev-----------");
                
            }
            Console.WriteLine("3. Return");
            Console.Write(" >> Please select an option: ");
            string input = Console.ReadKey().KeyChar.ToString();
            switch (input)
            {
                case "1":
                    GameOptions.setSeed = !GameOptions.setSeed;
                    Options();
                    break;
                case "2":
                    GameOptions.isDev = !GameOptions.isDev;
                    Console.Clear(); Console.Write("Please wait");
                    Timer loading = LoadingDots(40);
                    loading.Start();
                    new GameSave().SaveGame();
                    Thread.Sleep(120); // Simulate loading
                    loading.Stop();
                    Options();
                    break;
                case "3":
                    Show();
                    break;
                case "D":
                    GameSave.DeleteSave();
                    break;
                default:
                    Console.WriteLine("\n\n Invalid option. Please try again.");
                    Console.WriteLine(" >> Press any key to continue...");
                    Console.ReadKey();
                    Options();
                    break;
            }
        }

        public static void CreateSaveFile()
        {
            Console.Clear();
            Console.WriteLine("Creating Save File (#1)");
            Console.Write("\n >> Enter your Name: ");
            string name = Console.ReadLine() ?? "";
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.Write("Name cannot be empty. \n >> Please enter your Name: ");
                name = Console.ReadLine() ?? "";
            }

            Console.Clear();
            Console.WriteLine("(#2) Creating Save File");
            int seed = GameRandom.GenerateSeed();
            if (GameOptions.setSeed)
            {
                Console.Write(" >> Enter your Seed (leave blank for random): ");
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    while (!int.TryParse(input, out seed))
                    {
                        Console.Clear();
                        Console.WriteLine("(#2) Creating Save File");
                        Console.Write("Invalid number entered. \n >> Enter your Seed (leave blank for random): ");
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
            Console.Write("Generating Seed: \n");
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
            Console.WriteLine("(#2) Creating Save File");
            Console.WriteLine($"Your seed: \n{seed}");
            Console.WriteLine(" >> Press any key to continue...");
            Console.ReadKey();
            Console.Clear();

            // Show loading dots
            Console.Write("(#3) Creating Save File");
            Timer loading = LoadingDots(100);
            loading.Start();

            GameStage.CurrentStage = 0;
            Player player = new Player(new Stats(), name);
            GameSave save = new GameSave(name, seed, player, GameStage.CurrentStage, GameOptions.isDev);
            GameSave.StartNewGame(save);
            save.SaveGame();

            Thread.Sleep(800);
            loading.Stop();
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
}