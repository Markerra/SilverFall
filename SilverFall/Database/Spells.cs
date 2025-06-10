namespace Game.Spells
{
    static class Database
    {
        public static Spell IceBlast = new Spell(
            "Ice Blast",
            "",
            ['I', 'C', 'E'],
            new SpellStats(
                damage:  25,
                manaCost: 13
            )
        );

        public static Spell Fireball = new Spell(
            "Fireball",
            "",
            ['F', 'I', 'R', 'E'],
            new SpellStats(
                damage:  32,
                manaCost: 10
            )
        );
    }
    
}