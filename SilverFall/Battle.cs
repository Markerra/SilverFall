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
                { eTurn = Attacker; attackerWeapon.CombatStyle.MakeTurn(this, Attacker, Target); } // Make a turn
                else { throw new NullReferenceException($"Attacker {Attacker.Name} doesn't have any weapon equipped!"); }

                if (!Target.IsAlive) { break; }

                // Target's turn
                if (targetWeapon != null)
                { eTurn = Target; targetWeapon.CombatStyle.MakeTurn(this, Target, Attacker, AI: true); } // Make a turn
                else { throw new NullReferenceException($"Attacker {Target.Name} doesn't have any weapon equipped!"); }
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

                Thread.Sleep(8);
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
                Thread.Sleep(5); // Add delay for other UI displays correctly
                StartCountdown(seconds, () => timeUp);
                timeUp = true;
            });

            // The playerActionLoop receives a function to check if time is up
            playerActionLoop(
                () => (DateTime.Now - turnStart).TotalSeconds >= seconds || timeUp,
                () => timeUp = true);

            timeUp = true; // Ensure timer stops if player ends early
            timerTask.Wait();
        }

        public enum BattleAction
        {
            None,
            Attack,
            Block,
            CastSpell,
            SelectSpell,
            Inventory,
            Equipment
        }

    }

    interface ICombatStyle
    {
        void MakeTurn(Battle battle, Entity attacker, Entity target, bool AI = false);
    }

    // Logic for Knight class combat
    class KnightCombat : ICombatStyle
    {
        public void MakeTurn(Battle battle, Entity attacker, Entity target, bool AI)
        {
            if (battle == null) { throw new NullReferenceException("Can't make a turn when battle isn't started"); }
            if (!attacker.IsInBattle() || !target.IsInBattle()) { throw new ArgumentException("Can't make a turn when attacker/target isn't in a battle"); }
            if (attacker is Player player)
            {
                Battle.TimedPlayerTurn(5, (isTimeUp, endTurn) =>
                {
                    while (!isTimeUp())
                    {
                        // Per-action input logic
                        UI.Battle.ShowBattleInfo(battle);

                        // Shows and stores the user's battle action
                        switch (UI.Battle.ShowBattleOptions(battle, PlayerClasses.Detector(player), isTimeUp))
                        {
                            case Battle.BattleAction.Attack: GameLog.Write("Attack"); endTurn(); return;
                            case Battle.BattleAction.Block: GameLog.Write("Block"); endTurn(); return;
                            case Battle.BattleAction.SelectSpell: GameLog.Write("SelectSpell"); endTurn(); break;
                            case Battle.BattleAction.CastSpell: GameLog.Write("CastSpell"); endTurn(); break;
                            case Battle.BattleAction.Equipment: GameLog.Write("Equipment"); break;
                            case Battle.BattleAction.Inventory: GameLog.Write("Inventory"); break;
                            default: endTurn(); return; // None - skips the timer
                        }
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

    // Logic for Archer class combat
    class ArcherCombat : ICombatStyle
    {
        public void MakeTurn(Battle battle, Entity attacker, Entity target, bool AI)
        {
            if (attacker is Player || !AI)
            {
                Battle.TimedPlayerTurn(30, (isTimeUp, endTurn) =>
                {
                    // Per-action input logic
                    
                });
                Console.ReadKey();
            }
            else
            {
                // AI makes decision without timer
            }
        }
    }

    // Logic for Magician class combat
    class MagicianCombat : ICombatStyle
    {
        public void MakeTurn(Battle battle, Entity attacker, Entity target, bool AI)
        {
            if (attacker is Player || !AI)
            {
                Battle.TimedPlayerTurn(30, (isTimeUp, endTurn) =>
                {
                    // Per-action input logic
                });
                Console.ReadKey();
            }
            else
            {
                // AI makes decision without timer
            }
        }
    }
}