using System;

namespace Game_Of_Life
{
    class Ladybird : Insect
    {
        private readonly int starve_cap = 3;
        internal int Starve_cap { get { return starve_cap; } }
        private readonly int breed_cap = 8;
        internal override int Breed_cap { get { return breed_cap; } }
        private int starve_cd;
        internal int Starve_cd
        {
            get { return starve_cd; }
            set { starve_cd = value; }
        }
        internal Ladybird(Position pos):base(pos)
        {
            Breed_cd = Breed_cap;
            Starve_cd = Starve_cap;
        }

        /// <summary>
        /// Moves an object, if it has not eaten this turn.
        /// </summary>
        internal override void Move()
        {
            /*
             * Every  time  step,  if  there  is  an  adjacent  greenfly  (up,  down,  left  or  right),
             * then the ladybird will move to that cell and eat the greenfly. 
             * Otherwise, the ladybird moves according  to  the  same  rules  as  the  greenfly.  
             * Note  that  a  ladybird  cannot  eat  other ladybirds. 
             */
            if (!Eat())
            {
                base.Move();
                Starve();
            }
            return;
        }

        /// <summary>
        /// Checks directions around the ladybird for a greenfly and if it finds one, moves ladybird to the greenfly's position overwriting it.
        /// </summary>
        /// <returns></returns>
        internal bool Eat()
        {
            foreach (var direction in Enum.GetValues(typeof(Directions))) //doing this to randomize the order of looking around for food
            {
                switch (direction)
                {
                    case Directions.Up:
                        if (ConsumeAtPosition(Position.Up(this.Location)))
                        {
                            return true;
                        }
                        break;
                    case Directions.Down:
                        if (ConsumeAtPosition(Position.Down(this.Location)))
                        {
                            return true;
                        }
                        break;
                    case Directions.Left:
                        if (ConsumeAtPosition(Position.Left(this.Location)))
                        {
                            return true;
                        }
                        break;
                    case Directions.Right:
                        if (ConsumeAtPosition(Position.Right(this.Location)))
                        {
                            return true;
                        }
                        break;
                    case Directions.UpRight:
                        if (ConsumeAtPosition(Position.UpRight(this.Location)))
                        {
                            return true;
                        }
                        break;
                    case Directions.UpLeft:
                        if (ConsumeAtPosition(Position.UpLeft(this.Location)))
                        {
                            return true;
                        }
                        break;
                    case Directions.DownRight:
                        if (ConsumeAtPosition(Position.DownRight(this.Location)))
                        {
                            return true;
                        }
                        break;
                    case Directions.DownLeft:
                        if (ConsumeAtPosition(Position.DownLeft(this.Location)))
                        {
                            return true;
                        }
                        break;
                    default:
                        Console.WriteLine("Eat() direction is off");
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// Updates starve counter. Kills off the ladybird if it has reached zero.
        /// Needs to be called at the end of each turn, after eating process is completed.
        /// </summary>
        internal void Starve()
        {
            Starve_cd--;
            if (starve_cd == 0)
            {
                Game_Manager.Instance.Grid[this.Location.row, this.Location.column] = null;
                Logger.ladybirds_count--;
            }
            return;
        }

        /// <summary>
        /// Requires a Position. Checks if it is occupied by a greenfly, if so, moves the examined ladybird to that position overwriting the greenfly.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal bool ConsumeAtPosition(Position target)
        {
            if (IdentifyBlock(target) == GameObject.Greenfly)
            {
                MoveToPosition(target);
                Logger.eat_count++;
                starve_cd = Starve_cap;
                return true;
            }
            return false;
        }
    }
}