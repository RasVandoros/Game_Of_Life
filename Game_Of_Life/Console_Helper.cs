using System;

namespace Game_Of_Life
{
    public static class Console_Helper
    {
        internal static int gen = 0;

        /// <summary>
        /// Takes a jagged array and draws the visualization on the screen.
        /// </summary>
        /// <param name="grid"></param>
        internal static void Draw(object[,] grid)
        {
            Console.Clear(); //Comment this one out for debugging
            Console.WriteLine("Gen " + gen);

            DrawLine(grid.GetLength(1));

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] is null)
                    {
                        Console.Write("|   ");
                    }
                    else if (grid[i, j].GetType() == typeof(Ladybird))
                    {
                        Console.Write("| X ");
                    }
                    else if (grid[i, j].GetType() == typeof(Greenfly))
                    {
                        Console.Write("| O ");
                    }
                    else
                    {
                        Console.Write("BUG");
                    }
                }
                Console.Write("|");
                Console.Write("\n");
                DrawLine(grid.GetLength(1));
            }
            Console.WriteLine("‘O’: greenfly"); Console.WriteLine("‘X’: ladybird");
            Console.WriteLine("(X) to exit");
        }

        /// <summary>
        /// Takes the integer value of the grid's width and draws a line on the UI.
        /// </summary>
        /// <param name="width"></param>
        private static void DrawLine(int width)
        {
            for (int i = 0; i < width * 4 + 1; i++) // width * 4 because for every element i of the grid, prints 4 elements. For example "| O ".
            {
                Console.Write("-");
            }
            Console.Write("\n");
        }

        /// <summary>
        /// Posts a question on the UI and receives the answer. If the answer is legit, returns the it.
        /// The parameters are: String target of the question (How many of what?)/. Integer suggested value.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="suggested_value"></param>
        /// <returns></returns>
        internal static int GetInitialValues(string target, int suggested_value)
        {
            int input;
            bool done = false;
            do
            {
                Console.Clear();
                Console.WriteLine("How many " + target + " would you like?\n" +
                    "Options:\n" +
                    "   Insert your number. \n" +
                    "   Insert blank for default value(" + suggested_value +")\n" +
                    "   Insert 'x' to exit the application\n" +
                    "Press the Enter button to confirm your choice");

                string dirty_input = Console.ReadLine();
                if (int.TryParse(dirty_input, out input) && input > 0)
                {
                    done = true;
                }
                if (dirty_input == "")
                {
                    Console.WriteLine("Using default value " + suggested_value + " for " + target + "!");
                    input = suggested_value;
                    Console.ReadLine();
                    done = true;
                }
                if (dirty_input.ToLower() == "x")
                {
                    Console.WriteLine("Thank you for playing!");
                    Environment.Exit(0);
                }
            } while (!done);
            return input;
        }

        /// <summary>
        /// Posts the Help UI menu.
        /// </summary>
        internal static void Help()
        {
            Console.Clear();
            Console.WriteLine("Game Of Life");
            Console.WriteLine("Summary:");
            Console.WriteLine("In  1970  the  British  mathematician  John  Conway  formulated  ‘The Game  of  Life’ which models the  evolution  of  cells  in  a  closed  2D  grid  when  those  cells  are  governed  by  certain rules. The game is relevant for computer simulations of the real world in fields as diverse as biology and economics because patterns may emerge that can be used to predict the future (egpopulation  dynamics,  the  state  of  the  stock  market).");
            Console.WriteLine("\nFor help with the different game modes press:");
            Console.WriteLine("1: Game Mode");
            Console.WriteLine("2: Scientific Mode");
            Console.WriteLine("3: Go back to main menu");
            Console.WriteLine("4: To Exit");
            bool keepgoing = true;
            do
            {
                string key = Console.ReadLine();
                if (key == "1")
                {
                    HelpGameMode();
                    Console.ReadLine();
                    Console.Clear();
                    keepgoing = false;
                }
                else if (key == "2")
                {
                    HelpScientificMode();
                    Console.ReadLine();
                    Console.Clear();
                    keepgoing = false;
                }
                else if (key == "3")
                {
                    Game_Manager.Instance.RunApp();
                    keepgoing = false;
                }
                else if (key.ToLower() == "4")
                {
                    Environment.Exit(0);
                }
            } while (keepgoing);
        }

        private static void HelpScientificMode()
        {
            Console.Clear();
            Console.WriteLine("In scietific mode, you are given the option to choose how many times you would like to run the simulation." +
                "In this mode, the visualization is not available. The results from all the runs performed can be found inside the debug folder in a csv format." +
                "This mode is used purely to produce large datasets for analysis and therefore any visualisation tools for the turn based events would simply clutter the user.");
            Console.ReadLine();
            Help();
        }
        private static void HelpGameMode()
        {
            Console.Clear();
            Console.WriteLine("In game mode you are in control of the simulation. By pressing a button, you can skip to the next time step. " +
                "The Greenflies are represented by 'O' and the Ladybirds by 'X'. To exit the simulation, you can press the button 'X'. " +
                "The data from the run, is saved inside the Debug folder after it's completion.");
            Console.ReadLine();
            Help();
        }
        internal static string GetMode()
        {
            bool done = false;
            string dirty_input;
            do
            {
                Console.Clear();
                Console.WriteLine("Insert the number that corresponds to the mode you would like to initiate: \n" +
                    "1) Game Mode \n" +
                    "2) Scientific Mode \n" +
                    "3) Help \n" +
                    "4) Exit");
                dirty_input = Console.ReadLine();
                if (dirty_input == "1" || dirty_input == "2" || dirty_input == "3" || dirty_input == "4")
                {
                    done = true;
                }
            } while (!done);
            return dirty_input;
        }

        /// <summary>
        /// Takes a string, posts it to the user and keeps asking until the user enters a positive integer. Returns the integer. 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        internal static int GetPositiveInt(string msg)
        {
            Console.WriteLine(msg);
            int count;
            while (!Int32.TryParse(Console.ReadLine(), out count) || count <= 0)
            {
                Console.WriteLine("Please enter a valid numberical value!");
            }
            return count;
        }

    }

    /// <summary>
    /// Enum used for categorizing the different states of a grid cell.
    /// </summary>
    internal enum GameObject { Empty, Wall, Ladybird, Greenfly };

    /// <summary>
    /// Enum used for referring to the directions used.
    /// </summary>
    internal enum Directions { Up, Down, Left, Right, UpRight, UpLeft, DownRight, DownLeft };

    /// <summary>
    /// Struct used to model custom movement within the grid.
    /// </summary>
    internal struct Position
    {
        public int row;
        public int column;
        public Position(int x, int y)
        {
            row = x;
            column = y;
        }
        public static Position Up(Position pos)
        {
            Position step = new Position(pos.row - 1, pos.column);
            return step;
        }
        public static Position Down(Position pos)
        {
            Position step = new Position(pos.row + 1, pos.column);
            return step;
        }
        public static Position Right(Position pos)
        {
            Position step = new Position(pos.row, pos.column + 1);
            return step;
        }
        public static Position Left(Position pos)
        {
            Position step = new Position(pos.row, pos.column - 1);
            return step;
        }
        public static Position UpRight(Position pos)
        {
            Position step = new Position(pos.row - 1, pos.column + 1);
            return step;
        }
        public static Position UpLeft(Position pos)
        {
            Position step = new Position(pos.row - 1, pos.column - 1);
            return step;
        }
        public static Position DownRight(Position pos)
        {
            Position step = new Position(pos.row + 1, pos.column + 1);
            return step;
        }
        public static Position DownLeft(Position pos)
        {
            Position step = new Position(pos.row + 1, pos.column - 1);
            return step;
        }
    };
}