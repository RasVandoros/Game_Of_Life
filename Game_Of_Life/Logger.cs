using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game_Of_Life
{
    internal static class Logger
    {
        //Static fields within Logger keeping track of the in game statistics.
        internal static List<Observation> Observations;
        internal static List<List<string>> Frames;
        internal static int greenflies_count;
        internal static int ladybirds_count;
        internal static int greenfly_breed_count;
        internal static int ladybird_breed_count;
        internal static int eat_count;

        /// <summary>
        /// Records the state of the board (generation, greenfly count, ladybird count, number of times a greenfly breaded, number of times a ladybird breaded, number of times a greenfly was eaten)
        /// </summary>
        internal static void Observe()
        {
            Observations.Add(new Observation());
        }

        /// <summary>
        /// Writes the statistics gathered in a csv file inside the bin folder of the project.
        /// </summary>
        internal static void ReportStats()
        {
            StringBuilder sb = new StringBuilder();
            string filename = "Stats" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            string extension = ".csv";
            string fullPath = filename + extension;
            int count = 1;
            while (File.Exists(fullPath))
            {
                string tempFileName = string.Format("{0}({1})", filename, count++);
                fullPath = Path.Combine(tempFileName + extension);
            }
            string delimiter = ",";

            sb.AppendLine(string.Join(delimiter, "Generation", "Greenflies", "Ladybirds", "Greenfly Breed Count", "Ladybird Breed Count", "Eat Count"));
            for (int i = 0; i < Observations.Count; i++)
            {
                sb.AppendLine(string.Join(delimiter, Observations[i].generation, Observations[i].greenfly_count, Observations[i].ladybird_count, Observations[i].greenfly_breed_count, Observations[i].ladybird_breed_count, Observations[i].eat_count));
            }
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + fullPath, sb.ToString());
        }

        /// <summary>
        /// Saves the frameshots gathered during the run in a csv file, inside the bin folder of the project.
        /// </summary>
        internal static void SaveFrameshots()
        {
            StringBuilder sb = new StringBuilder();
            string filename = "Frameshot" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            string extension = ".csv";
            string fullPath = filename + extension;
            int count = 1;
            while (File.Exists(fullPath))
            {
                string tempFileName = string.Format("{0}({1})", filename, count++);
                fullPath = Path.Combine(tempFileName + extension);
            }
            string delimiter = ",";

            int rows = (int)Game_Manager.Instance.Parameters["rows"];
            int columns = (int)Game_Manager.Instance.Parameters["columns"];
            List<string> positions = new List<string>();
            for (int k = 0; k < rows; k++)
            {
                for (int l = 0; l < columns; l++)
                {
                    positions.Add("(" + k + "-" + l + ")"); //Used when multiple runs are finished at the same time.
                }
            }
            sb.AppendLine(String.Join(delimiter, positions));
            for (int i = 0; i < Frames.Count; i++)
            {
                sb.AppendLine(String.Join(delimiter, Frames[i]));
            }
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + fullPath, sb.ToString());
        }

        /// <summary>
        /// Initialize values required to log the statistics of the game.
        /// Takes in the number of ladybirds and greenflies on the board.
        /// </summary>
        /// <param name="ld_count"></param>
        /// <param name="gf_count"></param>
        internal static void SetGlobals(int ld_count, int gf_count)
        {
            Logger.Frames = new List<List<string>>();
            Logger.Observations = new List<Observation>();
            Console_Helper.gen = 0;
            Logger.greenfly_breed_count = 0;
            Logger.ladybird_breed_count = 0;
            Logger.eat_count = 0;
            Logger.ladybirds_count = ld_count;
            Logger.greenflies_count = gf_count;
        }

        /// <summary>
        /// Sets all the values used to gather statistics back to 0.
        /// </summary>
        internal static void ResetGlobals()
        {
            Logger.greenfly_breed_count = 0;
            Logger.ladybird_breed_count = 0;
            Logger.eat_count = 0;
            Logger.ladybirds_count = 0;
            Logger.greenflies_count = 0;
        }
        internal static void Frameshot()
        {
            Insect[,] odds = Game_Manager.Instance.Grid;
            List<string> temp = new List<string>();
            for (int k = 0; k < odds.GetLength(0); k++)
            {
                for (int l = 0; l < odds.GetLength(1); l++)
                {
                    if (odds[k, l] is null)
                    {
                        temp.Add("E");
                    }
                    else if (odds[k, l] is Ladybird)
                    {
                        temp.Add("L");
                    }
                    else if (odds[k, l] is Greenfly)
                    {
                        temp.Add("G");
                    }
                    else
                    {
                        temp.Add("Something is wrong with the frameshot function. Please contact the developer.");
                    }
                }
            }
            Frames.Add(temp);
        }
    }

    /// <summary>
    /// Class for keeping track of statistics of the game
    /// </summary>
    internal class Observation
    {
        public int generation, greenfly_count, ladybird_count, greenfly_breed_count, ladybird_breed_count, eat_count;
        public Observation() 
        {
            generation = Console_Helper.gen;
            greenfly_count = Logger.greenflies_count;
            ladybird_count = Logger.ladybirds_count;
            greenfly_breed_count = Logger.greenfly_breed_count;
            ladybird_breed_count = Logger.ladybird_breed_count;
            eat_count = Logger.eat_count;
        }
    }
}
