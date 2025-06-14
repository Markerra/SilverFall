namespace Game
{
    class Equipment : Item
    {
        public string Slot { get; set; }
        public Stats BonusStats { get; set; }
        public Equipment(string name, string description, string rarity, string slot, Stats bonusStats) : base(name, description, rarity, null)
        {
            this.Slot = slot switch
            {
                "Head" => "Head",
                "Chest" => "Chest",
                "Legs" => "Legs",
                "Feet" => "Feet",
                "Hands" => "Hands",
                "Necklace" => "Necklace",
                "Offhand" => "Offhand",
                "Weapon" => "Weapon",
                "SpellBook" => "SpellBook",
                _ => throw new ArgumentException("Invalid equipment slot.")
            };
            this.BonusStats = bonusStats;
        }

        public void Equip() { if (Owner != null) { Owner.Equip(this); } }
    }

    class Helmet : Equipment
    {
        public Helmet(string name, string description, string rarity, Stats bonusStats) : base(name, description, rarity, "Head", bonusStats) { }

    }


    class Chestplate : Equipment
    {
        public Chestplate(string name, string description, string rarity, Stats bonusStats) : base(name, description, rarity, "Chest", bonusStats) { }

    }

    class Leggings : Equipment
    {
        public Leggings(string name, string description, string rarity, Stats bonusStats) : base(name, description, rarity, "Legs", bonusStats) { }

    }

    class Boots : Equipment
    {
        public Boots(string name, string description, string rarity, Stats bonusStats) : base(name, description, rarity, "Feet", bonusStats) { }

    }

    class Gloves : Equipment
    {
        public Gloves(string name, string description, string rarity, Stats bonusStats) : base(name, description, rarity, "Hands", bonusStats) { }

    }

    class Necklace : Equipment
    {
        public Necklace(string name, string description, string rarity, Stats bonusStats) : base(name, description, rarity, "Necklace", bonusStats) { }

    }

    class Shield : Equipment
    {
        public Shield(string name, string description, string rarity, Stats bonusStats) : base(name, description, rarity, "Offhand", bonusStats) { }

    }

    class SpellBook : Equipment
    {
        public List<Spell> Spells { get; set; }
        public Spell? SelectedSpell { get; set; }
        public SpellBook(string name, string description, string rarity, Stats bonusStats) : base(name, description, rarity, "SpellBook", bonusStats)
        {
            Spells = new List<Spell>();
        }

        public void SelectSpell()
        {
            if (Spells.Count == 0) { Console.WriteLine($"{Name} has no spells in it"); return; }
            for (int i = 0; i < Spells.Count; i++)
            {
                Spell spell = Spells[i];
                string text = $"{i + 1}. {spell.Name} (Damage: {spell.Stats.Damage}"
                + $"Cost: {spell.Stats.GetManaCost(Owner)})";
                if (SelectedSpell != spell) { Console.WriteLine(text); } else { Console.WriteLine(text + " [selected]"); }
            }
            Console.WriteLine(" >> Select your spell: ");
            int index;
            GameLog.Write($"{GameManager.Player.Name} is selecting spell in {Name}");
            while (true)
            {
                char c = Console.ReadKey(true).KeyChar;
                if (char.IsDigit(c))
                {
                    int digit = c - 48; // IDK HOW IT WORKS BUT IT WORKS. DONT CHANGE THIS NUMBER
                    if (digit > 0 && digit <= Spells.Count) { index = digit; GameLog.Write($"index: {index}"); break; }
                }
            }
            SelectedSpell = Spells[index - 1];
        }
    }
}