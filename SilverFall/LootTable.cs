namespace Game
{
    using Game.Items;
    class LootTable
    {
        public int Experience { get; set; }
        public List<Item> Items = new List<Item>();

        private static List<Item> GetLootForEnemy(string enemyType)
        {
            List<Item> lootTable = enemyType switch
            {
                "Zombie" => new List<Item>
                { Database.LeatherArmor, Database.RingOfFortitude, Database.BronzeTrinket,
                Database.HealthPotion},
                "Goblin" => new List<Item>
                { Database.HealthPotion, Database.LeatherArmor, Database.BronzeTrinket,
                Database.GoldenTrinket, Database.RubyPendant },
                "Orc" => new List<Item>
                { Database.SilverRing, Database.HealthPotion, Database.GoldenTrinket,
                Database.IronHelmet, Database.AmuletOfWisdom },
                "Dragon" => new List<Item>
                { Database.ShadowLeggings, Database.HealthPotion, Database.GoldenTrinket,
                Database.RubyPendant, Database.AmuletOfWisdom, Database.DragonHelm,
                Database.KnightArmor },
                _ => new List<Item> { ItemsList.GetRandomItem() }
            };

            Enemy? enemy = EnemyList.FindEnemy(enemyType);

            // Adds enemy's equipment to the loot table
            if (enemy != null)
            {
                foreach (string slot in enemy.Equipment.Keys)
                {
                    Equipment? equipment = enemy.Equipment[slot];
                    if (equipment != null)
                    {
                        lootTable.Add(equipment);
                    }
                }
            }

            // Adds items from the enemy's inventory to the loot table
            if (enemy != null)
            {
                foreach (Item item in enemy.Inventory)
                {
                    if (item != null)
                    {
                        lootTable.Add(item);
                    }
                }
            }


            return lootTable;
        }
        private static int GetExpForEnemy(string enemyType)
        {
            return enemyType switch
            {
                "Zombie" => GameRandom.Instance.Next(200, 500),
                "Goblin" => GameRandom.Instance.Next(500, 800),
                "Orc" => GameRandom.Instance.Next(700, 1000),
                "Dragon" => GameRandom.Instance.Next(1000, 1500),
                _ => 1000
            };
        }

        public LootTable(string enemyType, int amountOfItems)
        {
            for (int i = 0; i < amountOfItems; i++)
            {
                Items.Add(ItemsList.GetRandomItemFor(GetLootForEnemy(enemyType)));
            }
            Experience = GetExpForEnemy(enemyType);
        }
    }
}