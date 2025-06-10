namespace Game
{
    using Game.Items;
    using Game.Enemies;

    class Enemy : Entity
    {
        public string Type { get; set; }
        public LootTable Loot { get; set; }

        public Enemy(Stats stats, string name, int level, Weapon weapon, string type, LootTable loot) : base(stats, name, level)
        {
            Type = type;
            Inventory = new List<Item> {weapon};
            Loot = loot;
            OnDeath += (attacker) =>
            {
                // Generate new LootTable
                Loot.Generate();

                // Adds enemy's equipment to the loot table
                foreach (string slot in Equipment.Keys)
                {
                    Equipment? equipment = Equipment[slot];
                    if (equipment != null)
                    {
                        Loot.Items.Add(equipment);
                    }
                }

                // Adds items from the enemy's inventory to the loot table
                foreach (Item item in Inventory)
                {
                    if (item != null)
                    {
                        Loot.Items.Add(item);
                    }
                }

                GameLog.Write($"{Name} has been killed by {attacker.Name}");

                Console.WriteLine($"{Name} has been defeated!");
                Console.WriteLine($"{attacker.Name} gained {Loot.Experience} experience points.");
                Console.WriteLine($"{attacker.Name} found the following items:");
                foreach (var item in Loot.Items)
                {
                    AddItem(item);
                    Console.WriteLine($"- {item.Name}: {item.Description}");
                }
            };
        }
    }

    static class EnemyList
    {
        public static List<Enemy> Enemies = new List<Enemy>
        {
            Game.Enemies.Database.Zombie,
            Game.Enemies.Database.Goblin,
            Game.Enemies.Database.Orc,
            Game.Enemies.Database.Dragon
        };

        public static Enemy? FindEnemy(string enemyType)
        {
            return Enemies.Find(enemy => enemy.Type == enemyType);
        }
    }

}