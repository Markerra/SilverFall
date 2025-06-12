namespace Game
{
    using Game.Items;
    class LootTable
    {
        public int Experience { get; set; }
        public List<Item> Items = new List<Item>();

        private string _type;
        private int _amount;

        private static List<Item> GetLootForEnemy(string enemyType)
        {
            List<Item> lootTable = enemyType switch
            {
                "Zombie" => new List<Item>
                { Database.LeatherArmor, Database.BronzeTrinket,
                Database.HealthPotion},
                "Goblin" => new List<Item>
                { Database.HealthPotion, Database.LeatherArmor, Database.BronzeTrinket,
                Database.GoldenTrinket, Database.RubyPendant },
                "Orc" => new List<Item>
                { Database.BronzeTrinket, Database.HealthPotion, Database.GoldenTrinket,
                Database.IronHelmet, Database.AmuletOfWisdom },
                "Dragon" => new List<Item>
                { Database.ShadowLeggings, Database.HealthPotion, Database.GoldenTrinket,
                Database.RubyPendant, Database.AmuletOfWisdom, Database.DragonHelm,
                Database.KnightArmor },
                _ => new List<Item> { ItemsList.GetRandomItem() }
            };

            GameLog.Write($"Generated loot for {enemyType} - {GameLog.ItemListToString(lootTable)}");

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
            _type = enemyType;
            _amount = amountOfItems;
            Generate();
        }

        public void Generate()
        {
            Items = new List<Item>();
            for (int i = 0; i < _amount; i++)
            {
                Items.Add(ItemsList.GetRandomItemFor(GetLootForEnemy(_type)));
            }
            Experience = GetExpForEnemy(_type);
            GameLog.Write($"Generated loot table for {_type}. Items: {GameLog.ItemListToString(Items)}, Exp: {Experience}");
        }
    }
}