namespace Game
{
    using System.Collections.Generic;

    class Player : Entity
    {

        public Player(Stats stats, string name) : base(stats, name, 1)
        {
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
}