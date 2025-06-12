namespace Game
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = GameInfo.Name;
            UI.MainMenu.Show();
        }
    }
}