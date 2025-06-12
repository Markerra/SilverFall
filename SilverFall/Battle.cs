namespace Game
{
    class Battle
    {
        public Entity Attacker { get; set; }
        public Enemy Target { get; set; }
        public Battle(Entity attacker, Enemy target)
        {
            Attacker = attacker;
            Target = target;
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
                { attackerWeapon.CombatStyle.MakeTurn(Attacker, Target); } // Make a turn
                else { throw new NullReferenceException($"Attacker {Attacker.Name} doesn't have any weapon equipped!"); }

                if (!Target.IsAlive) { break; }

                // Target's turn
                if (targetWeapon != null)
                { targetWeapon.CombatStyle.MakeTurn(Target, Attacker, AI: true); } // Make a turn
                else { throw new NullReferenceException($"Attacker {Attacker.Name} doesn't have any weapon equipped!"); }
            }
        }

        private static void StartCountdown(float durationSecnods)
        {
            var start = DateTime.Now;
            float timeLeft = durationSecnods;

            Console.CursorVisible = false;

            while (timeLeft > 0)
            {
                timeLeft = durationSecnods - (float)(DateTime.Now - start).TotalSeconds;
                if (timeLeft < 0) { timeLeft = 0; }

                // Format number to float with 2 numbers after dot
                string display = Localization.Get("BattleSecRemains", timeLeft.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));

                int top = Console.CursorTop;

                Console.SetCursorPosition(0, top);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, top);
                Console.Write(display);

                Thread.Sleep(10);
            }
            Console.CursorVisible = true;
        }

        public static void TimedPlayerTurn(int seconds, Action<Func<bool>> playerActionLoop)
        {
            DateTime turnStart = DateTime.Now;
            bool timeUp = false;

            // Start countdown in a background task
            var timerTask = Task.Run(() =>
            {
                StartCountdown(seconds);
                timeUp = true;
            });

            // The playerActionLoop receives a function to check if time is up
            playerActionLoop(() => (DateTime.Now - turnStart).TotalSeconds >= seconds || timeUp);

            timeUp = true; // Ensure timer stops if player ends early
            timerTask.Wait();
        }

    }

    interface ICombatStyle
    {
        void MakeTurn(Entity attacker, Entity target, bool AI = false);
    }

    // Logic for Knight class combat
    class KnightCombat : ICombatStyle
    {
        public void MakeTurn(Entity attacker, Entity target, bool AI)
        {
            if (GameManager.Battle == null) { throw new NullReferenceException("Can't make a turn when battle isn't started"); }
            if (!attacker.IsInBattle() || !target.IsInBattle()) { throw new ArgumentException("Can't make a turn when attacker/target isn't in a battle"); }
            if (attacker is Player || !AI)
            {
                Battle.TimedPlayerTurn(30, (isTimeUp) =>
                {
                    // Per-action input logic
                    UI.Battle.ShowBattleInfo(GameManager.Battle);
                });
                Console.ReadKey();
            }
            else
            {
                // AI makes decision without timer
            }
        }
    }

    // Logic for Archer class combat
    class ArcherCombat : ICombatStyle
    {
        public void MakeTurn(Entity attacker, Entity target, bool AI)
        {
            if (attacker is Player || !AI)
            {
                Battle.TimedPlayerTurn(30, (isTimeUp) =>
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
        public void MakeTurn(Entity attacker, Entity target, bool AI)
        {
            if (attacker is Player || !AI)
            {
                Battle.TimedPlayerTurn(30, (isTimeUp) =>
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