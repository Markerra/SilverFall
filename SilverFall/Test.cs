namespace Game
{
    static class TestFeature
    {
        static public void Run()
        {
            // This code can be runned from options with dev mode

            //GameLog.Write("Test");
            while (true) {
                if(Items.Database.MagicWand.CastSpell(Enemies.Database.Orc)) { break; }
            }
        }
    }
}