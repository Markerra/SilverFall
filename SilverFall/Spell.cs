namespace Game
{
    using System.Buffers;
    using System.ComponentModel;
    using Game.Spells;

    class Spell
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ConsoleKey[] KeyCombo { get; set; }
        public SpellStats Stats { get; set; }

        public Spell(string name, string desc, char[] keyCombo, SpellStats stats)
        {
            Name = name;
            Description = desc;
            KeyCombo = CreateCombo(keyCombo);
            Stats = stats;
        }

        public void Cast(Entity? owner, Entity target)
        {
            GameLog.Write($"{GameManager.Player.Name} casted {Name} on {target.Name}");
        }

        public static ConsoleKey[] CreateCombo(char[] keys)
        {
            List<ConsoleKey> combo = new List<ConsoleKey>();
            foreach (char c in keys)
            {
                string upper = c.ToString().ToUpper();
                if (Enum.TryParse(upper, out ConsoleKey key)) { combo.Add(key); }
                else { Console.WriteLine($"Invalid key combo params in: {keys}"); }
            }
            return combo.ToArray();
        }
    }

    static class SpellsList
    {
        public static List<Spell> Spells = new List<Spell>
        {
            Database.IceBlast,
            Database.Fireball,
        };

        
    }
}