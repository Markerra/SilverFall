namespace Game
{

    static class TestFeature
    {
        static public void Run()
        {
            //// This code can be runned from options with dev mode

            GameLog.Write("Test");
            Console.Clear();
            GameManager.Player.AddItem(Items.Database.MagicWand);
            GameManager.Player.AddItem(Items.Database.ShortSword);
            GameManager.Player.AddItem(Items.Database.BronzeTrinket);
            GameManager.Player.AddItem(Items.Database.IronHelmet);
            GameManager.Player.AddItem(Items.Database.SpellBook);
            GameManager.Player.Equip("Magic Wand");
            GameManager.Player.Equip("Spell Book");
            
            SpellBook? book = GameManager.Player.Equipment.SpellBook;
            if (book != null)
            {
                for (int i = 0; i < SpellsList.Spells.Count; i++)
                {
                    book.Spells.Add(SpellsList.Spells[i]);
                }
               // book.SelectSpell();
            }

            foreach (Enemy enemy in EnemyList.Enemies)
            {
                Console.WriteLine(enemy.Name);
                enemy.Stats.PrintStats();
            }

            UI.Generic.PressAnyKey();

            new Battle(GameManager.Player, Enemies.Database.Zombie).Start();

            //Weapon? weapon = GameManager.Player.Equipment.Weapon;
            //.Clear();
            //if (weapon is MagicWand magicWand) { magicWand.CastSpell(Enemies.Database.Orc); }

        }
    }
}