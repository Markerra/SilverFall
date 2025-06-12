namespace Game
{
    using Game.Items;
    using System.Collections.Generic;
    class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Rarity { get; set; }
        public static Dictionary<string, int> RarityChances = new Dictionary<string, int>
        {
            { "Common", 48 }, // Common has 48% chance
            { "Uncommon", 24 }, // Uncommon has 24% chance
            { "Rare", 18 }, // Rare has 18% chance
            { "Epic", 8 }, // Epic has 8% chance
            { "Legendary", 2 } // Legendary has 2% chance
        };
        public Entity? Owner { get; set; }
        public Action<Item>? Action { get; set; }
        public Item(string name, string description, string rarity = "Common", Action<Item>? action = null)
        {
            Name = name;
            Description = description;
            Action = action;
            Rarity = rarity;
        }

        public void Use()
        {
            Action?.Invoke(this);
        }

    }

    static class ItemsList
    {
        public static List<Item> Items = new List<Item>
        {
            Database.HealthPotion,
            Database.GoldenTrinket,
            Database.IronHelmet,
            Database.DragonHelm,
            Database.LeatherArmor,
            Database.KnightArmor,
            Database.IronLeggings,
            Database.ShadowLeggings,
            Database.SwiftBoots,
            Database.StoneBoots,
            Database.ThiefGloves,
            Database.GauntletsOfMight,
            Database.AmuletOfWisdom,
            Database.RubyPendant,
            Database.WoodenShield,
        };

        public static Item GetRandomItem(bool RarityCheck = true)
        {
            if (RarityCheck)
            {
                while (true)
                {
                    Item item = Items[GameRandom.Instance.Next(0, Items.Count)];
                    if (GameRandom.Instance.Next(0, 100) < Item.RarityChances[item.Rarity])
                    {
                        return item;
                    }
                }
            }
            else
            {
                return Items[GameRandom.Instance.Next(0, Items.Count)];
            }
        }

        public static Item GetRandomItemByRarity(string rarity)
        {
            List<Item> filteredItems = Items.FindAll(item => item.Rarity == rarity);
            if (filteredItems.Count == 0)
            {
                throw new ArgumentException($"No items found with rarity: {rarity}");
            }
            return filteredItems[GameRandom.Instance.Next(0, filteredItems.Count)];
        }

        public static List<Item> GetRandomItems(int amount, bool RarityCheck = true)
        {
            if (amount <= 0 || amount > Items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be between 1 and the total number of items.");
            }

            List<Item> randomItems = new List<Item>();


            for (int i = 0; i < amount;)
            {
                if (RarityCheck)
                {
                    var item = Items[GameRandom.Instance.Next(0, Items.Count)];
                    if (GameRandom.Instance.Next(0, 100) < Item.RarityChances[item.Rarity])
                    {
                        randomItems.Add(item);
                        i++;
                    }
                }
                else
                {
                    Item item = Items[GameRandom.Instance.Next(0, Items.Count)];
                    randomItems.Add(item);
                    i++;
                }
            }

            return randomItems;
        }

        public static Item GetRandomItemFor(List<Item> items, bool RarityCheck = true)
        {
            if (items == null || items.Count == 0)
            {
                throw new ArgumentException("The item list cannot be null or empty.");
            }

            if (RarityCheck) // Gets random item and incude item's chances
            {
                while (true)
                {
                    Item item = items[GameRandom.Instance.Next(0, items.Count)];
                    if (GameRandom.Instance.Next(0, 100) < Item.RarityChances[item.Rarity])
                    {
                        GameLog.Write($"Picked random item from {GameLog.ItemListToString(items)} with rarity check. Got {item.Name}");
                        return item;
                    }
                }
            }
            else
            {
                Item item = items[GameRandom.Instance.Next(0, items.Count)];
                GameLog.Write($"Picked random item from {GameLog.ItemListToString(items)} without rarity check. Got {item.Name}");
                return item;
            }
        }

        public static Item? GetItemByIndex(int index)
        {
            if (index < 0 || index >= Items.Count)
            {
                return null;
            }
            return Items[index];
        }

        public static Item? GetItemByName(string name)
        {
            return Items.Find(item => item.Name == name);
        }
    }
}