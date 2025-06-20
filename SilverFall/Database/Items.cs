namespace Game.Items
{
    static class Database
    {

        // Items
        // ---------------------


        // All Potions 

        public static Item HealthPotion = new Item
        ("Health Potion",
        "Restores 50 health points.",
        "Common",
        item =>
        {
            if (item != null)
            {
                item.Owner?.Heal(50);
                Console.WriteLine($"{item.Owner?.Name} used {item.Name} and restored 50 health points.");
            }
        });

        // All Accesories / Trophies

        public static Item BronzeTrinket = new Item(
            "Bronze Trinket",
            "A simple trinket made of bronze.",
            "Common",
            null
        );

        public static Item GoldenTrinket = new Item(
            "Golden Trinket",
            "A mysterious trinket that glows faintly.",
            "Rare",
            null
        );

        // All Helmets

        public static Helmet IronHelmet = new Helmet(
            "Iron Helmet",
            "A sturdy iron helmet. Increases Strength and Defense.",
            "Common",
            new Stats(false) { Strength = 2, Defense = 5 }
        );

        public static Helmet DragonHelm = new Helmet(
            "Dragon Helm",
            "A legendary helm forged from dragon scales. Greatly increases Strength and Defense.",
            "Legendary",
            new Stats(false) { Strength = 10, Defense = 20, Agility = 2 }
        );

        // All Chestplates

        public static Chestplate LeatherArmor = new Chestplate(
            "Leather Armor",
            "Light armor made from tough leather. Increases Agility and Defense.",
            "Common",
            new Stats(false) { Agility = 3, Defense = 4 }
        );

        public static Chestplate KnightArmor = new Chestplate(
            "Knight's Armor",
            "Heavy armor worn by knights. Greatly increases Strength and Defense, but reduces Agility.",
            "Rare",
            new Stats(false) { Strength = 5, Defense = 15, Agility = -2 }
        );

        // All Leggings

        public static Leggings IronLeggings = new Leggings(
            "Iron Leggings",
            "Iron leggings that offer solid protection.",
            "Common",
            new Stats(false) { Defense = 6 }
        );

        public static Leggings ShadowLeggings = new Leggings(
            "Shadow Leggings",
            "Leggings that grant the wearer swift movement.",
            "Rare",
            new Stats(false) { Agility = 7, Defense = 3 }
        );

        // All Boots

        public static Boots SwiftBoots = new Boots(
            "Swift Boots",
            "Boots that increase your speed and agility.",
            "Uncommon",
            new Stats(false) { Agility = 6 }
        );

        public static Boots StoneBoots = new Boots(
            "Stone Boots",
            "Heavy boots that offer great protection but slow you down.",
            "Rare",
            new Stats(false) { Defense = 8, Agility = -3 }
        );

        // All Gloves

        public static Gloves ThiefGloves = new Gloves(
            "Thief's Gloves",
            "Gloves favored by thieves. Increase Agility.",
            "Common",
            new Stats(false) { Agility = 4 }
        );

        public static Gloves GauntletsOfMight = new Gloves(
            "Gauntlets of Might",
            "Heavy gauntlets that boost Strength.",
            "Uncommon",
            new Stats(false) { Strength = 4 }
        );

        // All Necklaces

        public static Necklace AmuletOfWisdom = new Necklace(
            "Amulet of Wisdom",
            "A magical amulet that increases Intelligence.",
            "Rare",
            new Stats(false) { Intelligence = 8 }
        );

        public static Necklace RubyPendant = new Necklace(
            "Ruby Pendant",
            "A pendant that slightly increases all stats.",
            "Uncommon",
            new Stats(false) { Strength = 2, Agility = 2, Intelligence = 2 }
        );

        // All Shields

        public static Shield WoodenShield = new Shield(
            "Wooden Shield",
            "A simple wooden shield",
            "Common",
            new Stats(false) { Defense = 2 } // At the bottom add shield blocking logic for knight class
        );

        // Weapons
        // ---------------------

        // All Swords

        public static Sword ShortSword = new Sword
        ("Short Sword",
        "A basic sword for close combat.",
        rarity: "Common",
        new WeaponStats(
            damage: 10,
            critChance: 5F,
            critMult: 1.2F,
            null));
        public static Sword Katana = new Sword
        ("Katana",
        "A razor-sharp blade favored by samurai.",
        rarity: "Rare",
        new WeaponStats(
            damage: 18,
            critChance: 18,
            critMult: 1.6F,
            null));
        public static Sword Claymore = new Sword
        ("Claymore",
        "A heavy two-handed sword with devastating power.",
        rarity: "Rare",
        new WeaponStats(
            damage: 30,
            critChance: 12,
            critMult: 1.8F,
            null));
        public static Sword Excalibur = new Sword
        ("Excalibur",
        "A legendary sword of immense power.",
        rarity: "Legendary",
        new WeaponStats(
            damage: 65,
            critChance: 25,
            critMult: 2.3F,
            new Stats(false) { Strength = 20, Agility = -5 }));

        public static Weapon DragonClaw = new Weapon
        ("Dragon Claw",
        "A fearsome claw from a dragon. Increases Strength and Agility.",
        rarity: "Legendary",
        new WeaponStats(
            damage: 65,
            critChance: 45,
            critMult: 2.3F,
            new Stats(false) { Strength = 35, Agility = 10, Intelligence = -5 }));

        // All Heavy Weapons

        public static HeavyWeapon BattleAxe = new HeavyWeapon
        ("Battle Axe",
        "A heavy axe designed for powerful strikes.",
        rarity: "Rare",
        new WeaponStats(
            damage: 40,
            critChance: 20,
            critMult: 2.0F,
            new Stats(false) { Strength = 30, Agility = -10, Intelligence = -5 }));

        // All Bows

        public static Bow WoodenBow = new Bow
        ("Wooden Bow",
        "A simple bow made out of wood.",
        rarity: "Common",
        new WeaponStats(
            damage: 8,
            critChance: 5,
            critMult: 1.2F,
            null));
        public static Bow ShortBow = new Bow
        ("Short Bow",
        "A basic bow for ranged combat.",
        rarity: "Common",
        new WeaponStats(
            damage: 10,
            critChance: 10,
            critMult: 1.3F,
            null));
        public static Bow ElvenBow = new Bow
        ("Elven Bow",
        "A finely crafted bow with enhanced crit chance.",
        rarity: "Common",
        new WeaponStats(
            damage: 22,
            critChance: 30,
            critMult: 1.7F,
            null));
        public static Bow Longbow = new Bow
        ("Longbow",
        "A bow with great range and accuracy.",
        rarity: "Uncommon",
        new WeaponStats(
            damage: 25,
            critChance: 15,
            critMult: 1.5F,
            null));

        // All Magic Wands

        public static MagicWand MagicWand = new MagicWand
        ("Magic Wand",
        "A simple magic wand.",
        rarity: "Common",
        new WeaponStats(
            0, 0, 0, // Magic wand doesn't have damage, crit, etc.
            new Stats(false)
            {
                SpellAmplify = 0,
                ManaCostDecrease = 0
            }));

        // All Spell Books

        public static SpellBook SpellBook = new SpellBook
        ("Spell Book",
        "Default book used for storing many spells and their combos.",
        rarity: "Common",
        new Stats(false));

    }
}