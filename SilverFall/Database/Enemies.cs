namespace Game.Enemies
{
    using System.Collections.Generic;
    using Game.Items;

    static class Database
    {
        public static Enemy Zombie = new Enemy(
            new Stats()
            {
                Health = 100,
                Strength = 5,
                Agility = 2,
                Intelligence = 1,
                Defense = 3,
                MissChance = 3
            },
            "Zombie",
            1,
            Game.Items.Database.Claymore,
            "Zombie",
            new LootTable("Zombie", 3)
        );

        public static Enemy Goblin = new Enemy(
            new Stats()
            {
                Health = 70,
                Strength = 8,
                Agility = 4,
                Intelligence = 2,
                Defense = 3,
                MissChance = 4
            },
            "Goblin",
            1,
            Game.Items.Database.ShortSword,
            "Goblin",
            new LootTable("Goblin", 4)
        );

        public static Enemy Orc = new Enemy(
            new Stats()
            {
                Health = 120,
                Strength = 17,
                Agility = 7,
                Intelligence = 0,
                Defense = 15,
                MissChance = 1
            },
            "Orc",
            1,
            Game.Items.Database.BattleAxe,
            "Orc",
            new LootTable("Orc", 5)
        );

        public static Enemy Dragon = new Enemy(
            new Stats()
            {
                Health = 450,
                Strength = 17,
                Agility = 9,
                Intelligence = 10,
                Defense = 20,
                MissChance = 25
            },
            "Dragon",
            3,
            Game.Items.Database.DragonClaw,
            "Dragon",
            new LootTable("Dragon", 4)
        );
    }
}