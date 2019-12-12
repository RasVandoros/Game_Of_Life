using System;

namespace Game_Of_Life
{
    abstract class Insect
    {
        private int breed_cd;
        internal int Breed_cd
        {
            get { return breed_cd; }
            set { breed_cd = value; }
        }
        internal abstract int Breed_cap { get; }
        private bool recharging;
        internal bool Recharging
        {
            get { return recharging; }
            set { recharging = value; }
        }
        private Position location;
        internal Position Location
        {
            get { return location; }
            set { location = value; }
        }

        internal Insect(Position pos)
        {
            this.Location = pos;
            this.Recharging = false;
        }


        /// <summary>
        /// Move insect to new random location. Can move in every direction. (Vertically, horizontally and diagonally)
        /// </summary>
        internal virtual void Move()
        {
            /* Tries to move to a random direction, if it is impossible it doesnt move at all. This is intentional because of the requirements set. 
            If I wanted to make it keep looking until a free position is found, I would:
            Keep all directions in a list
            Start Loop. 
            Pick a random direction from the list. 
            If available move there and exit the loop.  
            If not available, take it out of the list of directions and go to start of loop. 
            */
            Random random = new Random();
            int rnd = random.Next(0, 8);
            Enum.TryParse<Directions>(rnd.ToString(), out Directions direction);
            switch (direction)
            {
                case Directions.Up:
                    MoveIfEmpty(Position.Up(Location));
                    break;
                case Directions.Down:
                    MoveIfEmpty(Position.Down(Location));
                    break;
                case Directions.Left:
                    MoveIfEmpty(Position.Left(Location));
                    break;
                case Directions.Right:
                    MoveIfEmpty(Position.Right(Location));
                    break;
                case Directions.UpRight:
                    MoveIfEmpty(Position.UpRight(Location));
                    break;
                case Directions.DownRight:
                    MoveIfEmpty(Position.DownRight(Location));
                    break;
                case Directions.UpLeft:
                    MoveIfEmpty(Position.UpLeft(Location));
                    break;
                case Directions.DownLeft:
                    MoveIfEmpty(Position.DownLeft(Location));
                    break;
                default:
                    Console.WriteLine("Random number generator is off");
                    break;
            }
            return;
        }

        /// <summary>
        /// Checks if the breeding cooldown of the examined insect has reached 0 and if so, spawns a new insect of the same type.
        /// </summary>
        internal virtual void Breed()
        {
            this.Breed_cd--;
            if (Breed_cd == 0)
            {
                SpawnChild();

                Breed_cd = Breed_cap;
            }
            return;
        }

        /// <summary>
        /// Spawns an insect of the same type as the examined insect.
        /// </summary>
        private void SpawnChild()
        {
            /* To look through every position around the parent, I foreach though all possible directions
             * This was used as a more agnostic aproach compared to setting an order for looking around for a free spot.
             * Could have used a way to randomise between all available Directions similar to the one explained in the comments of the Move() method.
             */
            foreach (var direction in Enum.GetValues(typeof(Directions)))
            {
                bool exitLoop = false;
                switch (direction)
                {
                    case Directions.Up:
                        exitLoop = SpawnIfEmpty(Position.Up(Location));
                        break;
                    case Directions.Down:
                        exitLoop = SpawnIfEmpty(Position.Down(Location));
                        break;
                    case Directions.Left:
                        exitLoop = SpawnIfEmpty(Position.Left(Location));
                        break;
                    case Directions.Right:
                        exitLoop = SpawnIfEmpty(Position.Right(Location));
                        break;
                    case Directions.UpRight:
                        exitLoop = SpawnIfEmpty(Position.UpRight(Location));
                        break;
                    case Directions.DownRight:
                        exitLoop = SpawnIfEmpty(Position.DownRight(Location));
                        break;
                    case Directions.UpLeft:
                        exitLoop = SpawnIfEmpty(Position.UpLeft(Location));
                        break;
                    case Directions.DownLeft:
                        exitLoop = SpawnIfEmpty(Position.DownLeft(Location));
                        break;
                    default:
                        Console.WriteLine("Random number generator is off");
                        break; //These break back to the foreach.
                }
                if (exitLoop)
                {
                    break; //This one is breaking the foreach look in case a free spot is found.
                }
            } 
        }

        /// <summary>
        /// Requires a Position. Checks if it is empty, if so it spawns an insect of the same type as the examined insect there.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private bool SpawnIfEmpty ( Position step )
        {
            if (IdentifyBlock(step) == GameObject.Empty)
            {
                SpawnAtPosition(step);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Requires a Position. Spawns an insect of the same type as the examined insect there.
        /// </summary>
        /// <param name="step"></param>
        internal void SpawnAtPosition(Position step)
        {
            if (GetType() == typeof(Ladybird))
            {
                Game_Manager.Instance.Grid[step.row, step.column] = new Ladybird(step);
                Logger.ladybird_breed_count++;
                Logger.ladybirds_count++;
            }
            else if (GetType() == typeof(Greenfly))
            {
                Game_Manager.Instance.Grid[step.row, step.column] = new Greenfly(step);
                Logger.greenflies_count++;
                Logger.greenfly_breed_count++;

            }
            else
            {
                Console.WriteLine("Spawning child error");
            }
        }

        /// <summary>
        /// Requires a Potition. Checks if it is empty, if so it moves the examined insect there.
        /// </summary>
        /// <param name="step"></param>
        private void MoveIfEmpty(Position step)
        {
            if (IdentifyBlock(step) == GameObject.Empty)
            {
                MoveToPosition(step);
            }
            return;
        }

        /// <summary>
        /// Requires a Position. Move the examined insect there.
        /// </summary>
        /// <param name="step"></param>
        internal void MoveToPosition(Position step) 
        {
            //Moves insect to new block blindly. Will overwrite if used on its own, therefore used for eating.
            Game_Manager.Instance.Grid[Location.row, Location.column] = null;
            this.Location = step;
            Game_Manager.Instance.Grid[step.row, step.column] = this;
        }

        /// <summary>
        /// Requires a Position. Checks the position in the grid and returns a GameObject object with information regarding what is there.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        internal GameObject IdentifyBlock(Position pos)
        {
            if (pos.row <  0 || pos.row >= Game_Manager.Instance.Grid.GetLength(0) || pos.column < 0 || pos.column >= Game_Manager.Instance.Grid.GetLength(1))
            {
                return GameObject.Wall;
            }
            if (Game_Manager.Instance.Grid[pos.row, pos.column] is null)
            {
                return GameObject.Empty;
            }
            if (Game_Manager.Instance.Grid[pos.row, pos.column].GetType() == typeof(Greenfly))
            {
                return GameObject.Greenfly;
            }
            else
            {
                return GameObject.Ladybird;
            }
        }
    }
}