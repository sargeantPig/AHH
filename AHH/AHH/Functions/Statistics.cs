using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AHH.Functions
{
    public enum Endings
    {
        God,
        Death,
        Passive,
    }

    public static class Statistics
    {
       static int kills { get; set; }
       static int  ressurections { get; set; }
       static int totalEnergyGained { get; set; }
       static int totalEnergySpent { get; set; }
       static int buildingsBuilt { get; set; }

        static Dictionary<Endings, int> endingsReached;
        static Endings ending { get; set; }

        static Statistics()
        {
            Reset();
        }

        public static void Reset()
        {
            kills = 0;
            ressurections = 0;
            totalEnergyGained = 0;
            totalEnergySpent = 0;
            buildingsBuilt = 0;
            ending = Endings.Passive;

            endingsReached = new Dictionary<Endings, int>() {
                {Endings.Death, 0 },
                {Endings.God, 0 },
                { Endings.Passive, 0 }
            };
        }

        public static void Load(string filepath)
        {
            if (!File.Exists(filepath))
            {
                Reset();
                return;
            }

            StreamReader sr = new StreamReader(filepath);

            string line = "";

            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("Kills"))
                {
                    string[] split = line.Split('\t');
                    kills = Convert.ToInt32(split[1]);
                }
                if (line.StartsWith("Ressurections"))
                {
                    string[] split = line.Split('\t');
                    ressurections = Convert.ToInt32(split[1]);
                }
                if (line.StartsWith("TotalEnergyGained"))
                {
                    string[] split = line.Split('\t');
                    totalEnergyGained = Convert.ToInt32(split[1]);
                }
                if (line.StartsWith("TotalEnergySpent"))
                {
                    string[] split = line.Split('\t');
                    totalEnergySpent = Convert.ToInt32(split[1]);
                }
                if (line.StartsWith("BuildingsBuilt"))
                {
                    string[] split = line.Split('\t');
                    buildingsBuilt = Convert.ToInt32(split[1]);
                }
                if (line.StartsWith("Endings"))
                {
                    string[] split = line.Split('\t', ',');
                    endingsReached[Endings.Death] = Convert.ToInt32(split[1]);
                    endingsReached[Endings.God] = Convert.ToInt32(split[2]);
                    endingsReached[Endings.Passive] = Convert.ToInt32(split[3]);
                }
            }
            sr.Close();
        }

        public static void Save(string filepath)
        {
            StreamWriter sw = new StreamWriter(filepath);

            sw.WriteLine("Kills\t" + kills.ToString());
            sw.WriteLine("Ressurections\t" + ressurections.ToString());
            sw.WriteLine("TotalEnergyGained\t" + totalEnergyGained.ToString());
            sw.WriteLine("TotalEnergySpent\t" + totalEnergySpent.ToString());
            sw.WriteLine("BuildingsBuilt\t" + buildingsBuilt.ToString());

            endingsReached[ending]++;

            sw.WriteLine("Endings\t" + endingsReached[Endings.Death].ToString() + ", " +
                endingsReached[Endings.God].ToString() + ", " + endingsReached[Endings.Passive]);

            sw.Close();
        }

        public static string Output
        {
            get {
                string str = "";

                str +=
                    "Stats Over all playthroughs." + "\r\n" +
                    "Kills: " + kills.ToString() + "\r\n" +
                    "Ressurections: " + ressurections.ToString() + "\r\n" +
                    "Total Energy Gained: " + totalEnergyGained.ToString() + "\r\n" +
                    "Total Energy Spent: " + totalEnergySpent.ToString() + "\r\n" +
                    "Buildings Built: " + BuildingsBuilt.ToString() + "\r\n" +
                    "Death Endings: " + endingsReached[Endings.Death].ToString() + "\r\n" +
                    "God Endings: " + endingsReached[Endings.God].ToString() + "\r\n" +
                    "Passive Ending: " + endingsReached[Endings.Passive].ToString();

                return str;

            }

        }

        static public int Kills {
            get { return kills; }
            set { kills = value; }
        }

        static public int Ressurections
        {
            get { return ressurections; }
            set { ressurections = value; }
        }

        static public int TotalEnergyGained
        {
            get { return totalEnergyGained; }
            set { totalEnergyGained = value; }
        }

        static public int TotalEnergySpent
        {
            get { return totalEnergySpent; }
            set { totalEnergySpent = value; }
        }

        static public int BuildingsBuilt
        {
            get { return buildingsBuilt; }
            set { buildingsBuilt = value; }
        }

        static public Endings Ending
        {
            get { return ending; }
            set { ending = value; }
        }
    }
}
