namespace Game_Of_Life
{
    class Greenfly : Insect
    {
        private readonly int breed_cap = 3;
        internal override int Breed_cap { get { return breed_cap; } }

        /// <summary>
        /// Greenfly constructor, sets the breed cooldown to the correct value as set within the class, and then uses the insect constructor. 
        /// </summary>
        /// <param name="pos"></param>
        internal Greenfly(Position pos) : base(pos)
        {
            Breed_cd = Breed_cap;
        }
    }
}