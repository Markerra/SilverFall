namespace Game
{
    using System.Collections.Generic;
    using System.Reflection.Metadata;

    class Player : Entity
    {
        public string Class = PlayerClasses.Knight;
        public Player(Stats stats, string name) : base(stats, name, 1)
        {
            Stats = stats;
            Name = name;
            Inventory = new List<Item>();
        }

        public void GiveWeapon(string weaponName)
        {
            Weapon? weapon = WeaponsList.GetWeaponByName(weaponName);
            if (weapon != null)
            {
                Inventory.Add(weapon);
            }
        }

        public void GiveWeapon(int index)
        {
            Weapon? weapon = WeaponsList.GetWeaponByIndex(index);
            if (weapon != null)
            {
                Inventory.Add(weapon);
            }
        }

        public void GiveWeapon()
        {
            Weapon? weapon = WeaponsList.GetRandomWeapon();
            if (weapon != null)
            {
                Inventory.Add(weapon);
            }
        }

    }
    
    static class PlayerClasses
    {
        public const string Knight = "Knight";
        public const string Archer = "Archer";
        public const string Magician = "Magician";
        public static string Detector(Player player)
        {
            Weapon? weapon = player.Equipment.Weapon;

            if (weapon != null)
            {
                Shield? shield = player.Equipment.Offhand;
                SpellBook? spellBook = player.Equipment.SpellBook;
                if (shield != null) { return Knight; }
                else if (weapon is Bow) { return Archer; }
                else if (weapon is MagicWand && spellBook != null) { return Magician; }
            }

            return player.Class; 
        } 
         
    }
}