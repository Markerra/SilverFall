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
            this.Type = type;
            this.Inventory = new List<Item>();
            this.Inventory.Add(weapon);

            this.Loot = loot;
            this.OnDeath += (attacker) =>
            {
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