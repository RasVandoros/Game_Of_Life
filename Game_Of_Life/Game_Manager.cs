using System;
using System.Collections;
using System.Collections.Generic;

namespace Game_Of_Life
{
    class Game_Manager
    {
        readonly int default_ladybirds = 5;
        readonly int default_greenflies = 20;
        private Hashtable parameters;
        internal Hashtable Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }
        private Insect[,] grid;
        internal Insect[,] Grid
        {
            get 
            { 
                return grid; 
            }
            set { grid = value; }
        }

        //Singleton
        private static Game_Manager instance;
        internal static Game_Manager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Game_Manager();
                }
                return instance;
            }
        }
        internal Game_Manager()
        {
            return;
        }


        /// <summary>
        /// Updates a single base class insect object. Recharging boolean is set to true, to signify that the object was updated during current time step.
        /// </summary>
        /// <param name="bug"></param>
        internal void UpdateInsect(Insect bug)
        {
           if (!bug.Recharging)
           {
                bug.Move();
                bug.Breed();
                bug.Recharging = true;
            }     
        }

        /// <summary>
        /// Takes in insect parameter and sets the recharging variable to false.
        /// </summary>
        /// <param name="bug"></param>
        internal void Recharge(Insect bug)
        {
                bug.Recharging = false;
        }

        /// <summary>
        /// Initiates all the reoccurring events within a time step.
        /// </summary>
        internal void Update()
        {
            Logger.ResetGlobals();
            Console_Helper.gen++;
            UpdateLadybirds();
            UpdateGreenflies();
            LoopThroughBugs(Recharge);
        }
        
        /// <summary>
        /// Updates all Ladybird objects in the grid.
        /// </summary>
        private void UpdateLadybirds()
        {
            Logger.ladybirds_count = 0;
            foreach (Insect bug in Game_Manager.Instance.Grid)
            {
                if (bug != null)
                {
                    if (bug is Ladybird && !bug.Recharging)
                    {
                        UpdateInsect(bug);
                        Logger.ladybirds_count++;
                    }
                }
            }
            
        }

        /// <summary>
        /// Updates all Greenfly objects in the grid.
        /// </summary>
        private void UpdateGreenflies()
        {
            Logger.greenflies_count = 0;
            foreach (Insect bug in Game_Manager.Instance.Grid)
            {
                if (bug != null)
                {
                    if (bug is Greenfly && !bug.Recharging)
                    {
                        UpdateInsect(bug);
                        Logger.greenflies_count++;
                    }
                }
            }

        }

        /// <summary>
        /// Expects number of ladybirds and greenflies. Spawns them at random positions within the grid.
        /// </summary>
        /// <param name="ladybird_count"></param>
        /// <param name="greenfly_count"></param>
        internal void InitiateWorld(int ladybird_count, int greenfly_count)
        {
            SpawnInRandomBlocks(SpawnLadybird, ladybird_count);
            SpawnInRandomBlocks(SpawnGreenfly, greenfly_count);
        }
        private Insect SpawnLadybird(Position pos)
        {
            return new Ladybird(pos);
        }
        private Insect SpawnGreenfly(Position pos)
        {
            return new Greenfly(pos);
        }

        /// <summary>
        /// Expects a function that creates an insect and how many times it has to be ran. Randomizes the spawning positions and then executes the spawns.
        /// </summary>
        /// <param name="SpawnBug"></param>
        /// <param name="count"></param>
        private void SpawnInRandomBlocks(Func<Position, Insect> SpawnBug, int count)
        {
            do
            {
                Random rnd = new Random();
                Position block = new Position(rnd.Next(0, Grid.GetLength(0)), rnd.Next(0, Grid.GetLength(1)));
                if (grid[block.row, block.column] is null)
                {
                    grid[block.row, block.column] = SpawnBug(block);
                    count--;
                }
            } while (count > 0);
        }

        /// <summary>
        /// Custom foreach method for itterating through array of objects without triping over null entries
        /// </summary>
        /// <param name="method"></param>
        internal void LoopThroughBugs(Action<Insect> method) 
        {
            foreach (Insect bug in Game_Manager.Instance.Grid)
            {
                if (bug != null)
                {
                    method(bug);
                }
            }
        }

        /// <summary>
        /// Game Mode initiates a version of the Game of Life where the grid is printed and the user controls the time steps.
        /// </summary>
        internal void RunGameMode()
        {
            SetUpApp();
            while (Console.ReadKey().Key != ConsoleKey.X)
            {
                Update();
                Console_Helper.Draw(Grid);
                Logger.Observe();
                Logger.Frameshot();
            }
            Logger.ReportStats();
            Logger.SaveFrameshots();
        }

        /// <summary>
        /// Sets up the initial state of the board. 
        /// </summary>
        internal void SetUpApp()
        {
            SetParameters();
            Logger.SetGlobals((int)Parameters["ladybirds"], (int)Parameters["greenflies"]);
            BuildWorld((int)Parameters["rows"], (int)Parameters["columns"], (int)Parameters["ladybirds"], (int)Parameters["greenflies"]);
            Console_Helper.Draw(Game_Manager.Instance.Grid);
        }

        /// <summary>
        /// Sets initial value of variables. Needed before starting a new run.
        /// </summary>
        internal void SetParameters()
        {
            int ladybirds, greenflies, rows, columns;
            bool keepgoing;
            do
            {
                keepgoing = false;
                ladybirds = Console_Helper.GetInitialValues("Ladybirds", default_ladybirds);
                greenflies = Console_Helper.GetInitialValues("Greenflies", default_greenflies);
                Logger.ladybirds_count = ladybirds;
                Logger.greenflies_count = greenflies;
                int min_cells = ladybirds + greenflies;
                rows = Console_Helper.GetInitialValues("rows", (int)Math.Sqrt(min_cells));
                columns = Console_Helper.GetInitialValues("columns", (int)Math.Ceiling((double)((min_cells / rows) + 1)));
                if (rows * columns < ladybirds + greenflies)
                {
                    keepgoing = true;
                    Console.Clear();
                    Console.WriteLine("Wrong Input Provided. The number of Ladybirds and Greenflies requested is higher than the number of cells. \nPlease try again.");
                    Console.ReadLine();
                }
            } while (keepgoing);
            Parameters = new Hashtable
            {
                { "ladybirds", ladybirds },
                { "greenflies", greenflies },
                { "rows", rows },
                { "columns", columns }
            };
        }

        /// <summary>
        /// Builds the gameboard at the backend. 
        /// Takes the number of rows, columns, ladybirds and greenflies.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="ladybirds"></param>
        /// <param name="greenflies"></param>
        internal void BuildWorld(int rows, int columns, int ladybirds, int greenflies)
        {
            grid = new Insect[rows, columns];
            InitiateWorld(ladybirds, greenflies);
            Logger.Observe();
            Logger.Frameshot();
        }

        /// <summary>
        /// Scientific Mode initiates a version of the Game of Life where everything is done in the back end.
        /// Game of life is run multiple times until only one species is alive on the board.
        /// </summary>
        internal void RunScientificMode()
        {
            SetParameters();
            int count = Console_Helper.GetPositiveInt("How many iterations would you like?");
            for (int i = 0; i < count; i++)
            {
                Logger.SetGlobals((int)Parameters["ladybirds"], (int)Parameters["greenflies"]);
                BuildWorld((int)Parameters["rows"], (int)Parameters["columns"], (int)Parameters["ladybirds"], (int)Parameters["greenflies"]);
                while (Logger.greenflies_count != 0 && Logger.ladybirds_count != 0)
                {
                    Game_Manager.Instance.Update();
                    Logger.Observe();
                    Logger.Frameshot();
                }
                Logger.SaveFrameshots();
                Logger.ReportStats();
                
            }
        }

        /// <summary>
        /// Run the main menu of the application.
        /// </summary>
        internal void RunApp()
        {
            string game_mode = Console_Helper.GetMode();
            if (game_mode == "1")
            {
                RunGameMode();
            }
            else if (game_mode == "2")
            {
                RunScientificMode();
            }
            else if (game_mode == "3")
            {
                Console_Helper.Help();
            }
            else if (game_mode == "4")
            {
                Environment.Exit(0);
            }
        }
    }
}