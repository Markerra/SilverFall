namespace Game
{
    class Equipment : Item
    {
        public string slot { get; set; }
        public Stats BonusStats { get; set; }
        public Equipment(string name, string description, string rarity, string slot, Stats bonusStats) : base(name, description, rarity, null)
        {
            this.slot = slot switch
            {
                "Head" => "Head",
                "Chest" => "Chest",
                "Legs" => "Legs",
                "Feet" => "Feet",
                "Hands" => "Hands",
                "Necklace" => "Necklace",
                "Ring" => "Ring",
                "Weapon" => "Weapon",
                "Other" => "Other",
                _ => throw new ArgumentException("Invalid equipment slot.")
            };
            this.BonusStats = bonusStats;
        }

        public void Equip()
        {
            if (this.Owner != null)
            {
                this.Owner.Equipment[this.slot] = this;
                this.Owner.Stats.AddStats(this.BonusStats);
            }
        }

        public void Equip(Equipment item) { item.Equip(); }

        public void Unequip()
        {
            if (this.Owner != null)
            {
                this.Owner.Equipment[this.slot] = null;
                this.Owner.Stats.RemoveStats(this.BonusStats);
            }
        }

        public void Unequip(Equipment item) { item.Unequip(); }
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

    class Ring : Equipment
    {
        public Ring(string name, string description, string rarity, Stats bonusStats) : base(name, description, rarity, "Ring", bonusStats) { }

    }
}