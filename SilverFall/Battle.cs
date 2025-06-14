namespace Game
{
    class Battle
    {
        public Entity Attacker { get; set; }
        public Enemy Target { get; set; }
        public int nTurn = 1;
        public int nAttackerTurn = 1;
        public int nTargetTurn = 1;
        public Entity eTurn;
        public enum BattleAction
        {
            None,
            Attack,
            Block,
            SelectSpell,
            Inventory,
            Equipment,
            ShowAttackerStats,
            ShowTargetStats
        }

        public static int TurnTime = 30; 

        public Battle(Entity attacker, Enemy target)
        {
            Attacker = attacker;
            Target = target;
            eTurn = Attacker;
        }

        public void Start()
        {
            GameLog.Write($"Battle between {Attacker.Name} and {Target.Name} has started");
            GameManager.Battle = this;
            while (Attacker.IsAlive && Target.IsAlive)
            {
                Console.Clear();

                Weapon? attackerWeapon = Attacker.Equipment.Weapon;
                Weapon? targetWeapon = Target.Equipment.Weapon;

                // Attacker's turn
                if (attackerWeapon != null)
                { eTurn = Attacker; MakeTurn(this, Attacker, Target); } // Make a turn
                else { throw new NullReferenceException($"Attacker {Attacker.Name} doesn't have any weapon equipped!"); }

                Thread.Sleep(10); // Small Delay

                if (!Target.IsAlive) { break; }

                // Target's turn
                if (targetWeapon != null)
                { eTurn = Target; MakeTurn(this, Target, Attacker, AI: true); } // Make a turn
                else { throw new NullReferenceException($"Attacker {Target.Name} doesn't have any weapon equipped!"); }

                Thread.Sleep(10); // Small Delay
            }
        }

        private static void StartCountdown(float durationSecnods, Func<bool> isTimeUp)
        {
            var start = DateTime.Now;
            float timeLeft = durationSecnods;
            int countdownLine = Console.CursorTop + 1;

            Console.CursorVisible = false;

            while (timeLeft > 0 && !isTimeUp())
            {
                timeLeft = durationSecnods - (float)(DateTime.Now - start).TotalSeconds;
                if (timeLeft < 0) { timeLeft = 0; }

                // Format number to float with 2 numbers after dot
                string display = Localization.Get("BattleSecRemains", timeLeft.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));

                Console.SetCursorPosition(0, countdownLine);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, countdownLine);
                Console.Write(display);

                UI.Battle.countdownLine = countdownLine;
                Thread.Sleep(50); // Add more delay for correctly updated UI
            }
            Console.CursorVisible = true;
        }

        public static void TimedPlayerTurn(int seconds, Action<Func<bool>, Action> playerActionLoop)
        {
            DateTime turnStart = DateTime.Now;
            bool timeUp = false;

            // Start countdown in a background task
            var timerTask = Task.Run(() =>
            {
                Thread.Sleep(15); // Add delay for other UI displays correctly
                StartCountdown(seconds, () => timeUp);
                timeUp = true;
            });

            // The playerActionLoop receives a function to check if time is up
            playerActionLoop(
                () => (DateTime.Now - turnStart).TotalSeconds >= seconds || timeUp,
                () =>
                {
                    // endTurn() method
                    timeUp = true;
                });

            timeUp = true; // Ensure timer stops if player ends early
            timerTask.Wait();
        }

        public void MakeTurn(Battle battle, Entity attacker, Entity target, bool AI = false)
        {
            if (battle == null) { throw new NullReferenceException("Can't make a turn when battle isn't started"); }
            if (!attacker.IsInBattle() || !target.IsInBattle()) { throw new ArgumentException("Can't make a turn when attacker/target isn't in a battle"); }

            // Regen hp, mand, etc. here >>

            if (attacker is Player player)
            {
                TimedPlayerTurn(TurnTime, (isTimeUp, endTurn) =>
                {
                    UI.Battle.ShowBattleInfo(battle);
                    UI.Battle.optionsLine = Console.CursorTop;
                    while (!isTimeUp())
                    {
                        bool clearBottomLines = true;
                        BattleAction action = UI.Battle.ShowBattleOptions(battle, PlayerClasses.Detector(player), isTimeUp);
                        switch (action)
                        {
                            // Ending the turn before the action is needed for properly updated UI
                            case BattleAction.Attack:
                                endTurn();
                                Console.Write("\n"); attacker.Attack(target);
                                Console.Write("\n"); UI.Generic.PressAnyKey();
                                break;
                            case BattleAction.Block: GameLog.Write("Block"); endTurn(); break;
                            case BattleAction.SelectSpell:
                                endTurn();
                                Console.Write("\n"); attacker.Equipment.SpellBook?.SelectSpell();
                                Console.Write("\n"); UI.Generic.PressAnyKey();
                                break;
                            case BattleAction.Equipment:
                                clearBottomLines = false;
                                //Console.Write("\n");
                                //UI.Battle.ShowInventory(attacker, isTimeUp); << Replace on ShowEquipment
                                break;
                            case BattleAction.Inventory:
                                clearBottomLines = false;
                                Console.Write("\n");
                                UI.Battle.ShowInventory(attacker, isTimeUp); // NOT FINISHED
                                break;
                            case BattleAction.ShowAttackerStats: UI.Battle.ShowAttackerInfo(battle); break;
                            case BattleAction.ShowTargetStats: UI.Battle.ShowTargetInfo(battle); break;
                            case BattleAction.None: endTurn(); return; // None - skips the timer
                        }
                        if (clearBottomLines) { UI.Generic.ClearLines(4); }
                    }
                    endTurn(); return; // Time's up
                });
            }
            else if (!AI)
            {
                // If attacker is not Player but !AI is true, handle accordingly or throw
                throw new InvalidOperationException("Non-player entity cannot perform a non-AI turn.");
            }
            else
            {
                // AI makes decision without timer
            }
            if (attacker == battle.Attacker) { battle.nAttackerTurn++; }
            else { battle.nTargetTurn++; }
            battle.nTurn++;
            GameLog.Write($"End of {battle.eTurn.Name}'s turn");
        }

    }
}