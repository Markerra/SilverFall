namespace Game.Enemies
{
    using System.Collections.Generic;
    using Game.Items;

    static class Database
    {
        public static Enemy Zombie = new Enemy(
            new Stats(
                str: 10,
                agi: 5,
                intell: 1,
                baseHp: 200,
                hpRegen: 3,
                baseMana: 0,
                manaRegen: 0,
                evasion: 10,
                def: 0
            ),
            "Zombie",
            1,
            Items.Database.Claymore,
            "Zombie",
            new LootTable("Zombie", GameRandom.Instance.Next(2, 4))
        );
        
        public static Enemy Goblin = new Enemy(
            new Stats(
                str: 15,
                agi: 12,
                intell: 9,
                baseHp: 230,
                hpRegen: 4,
                baseMana: 90,
                manaRegen: 2.5f,
                evasion: 3,
                def: 0
            ),
            "Goblin",
            1,
            Items.Database.ShortSword,
            "Goblin",
            new LootTable("Goblin", GameRandom.Instance.Next(2, 4))
        );

        public static Enemy Orc = new Enemy(
            new Stats(
                str: 35,
                agi: 3,
                intell: 0,
                baseHp: 230,
                hpRegen: 7,
                baseMana: 140,
                manaRegen: 3,
                evasion: 0,
                def: 8
            ),
            "Orc",
            1,
            Items.Database.BattleAxe,
            "Orc",
            new LootTable("Orc", GameRandom.Instance.Next(2, 4))
        );

        public static Enemy Dragon = new Enemy(
            new Stats(
                str: 60,
                agi: 15,
                intell: 40,
                baseHp: 400,
                hpRegen: 6,
                baseMana: 555,
                manaRegen: 12.5f,
                evasion: 30,
                def: 10
            ),
            "Dragon",
            3,
            Items.Database.DragonClaw,
            "Dragon",
            new LootTable("Dragon", GameRandom.Instance.Next(2, 4))
        );
    }
}