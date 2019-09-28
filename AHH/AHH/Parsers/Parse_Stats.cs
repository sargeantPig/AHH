using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using AHH.Interactable.Building;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AHH.UI;
using AHH.AI;
using AHH.Interactable.Spells;
using AHH.UI.Elements.Buttons;

namespace AHH.Parsers
{

	static partial class Parsers
	{
		public static Dictionary<T, Y> Parse_Stats<T, Y>(string filepath)
		{
            if (!File.Exists(filepath))
            {
                throw new Exception("Cannot locate file at: " + filepath);
                return null;
            }


            Dictionary<T, Y> stats = new Dictionary<T, Y>();
			StreamReader sr = new StreamReader(filepath);
			string line = "";

			dynamic temp_stats = new Stats(); //default

            if (typeof(Y) == typeof(BuildingData))
                temp_stats = new BuildingData();
            else if (typeof(Y) == typeof(Stats))
                temp_stats = new Stats();
			else if (typeof(Y) == typeof(Spell_Stats))
				temp_stats = new Spell_Stats();

			while ((line = sr.ReadLine()) != null)
			{
				if (line.StartsWith("Name"))
				{
					string[] split = line.Split('\t');
					temp_stats.Name = split[1];
				}

				if (line.StartsWith("Type"))
				{
					string[] split = line.Split('\t');
					temp_stats.Type = (T)Enum.Parse(typeof(T), split[1]);
					
				}

				if (line.StartsWith("Stats"))
				{
					string[] split = line.Split('\t', ',');

					if (typeof(Y) == typeof(Spell_Stats))
					{
						temp_stats.Cost = (float)Convert.ToDouble(split[1]);
                        temp_stats.OriginalCost = (float)Convert.ToDouble(split[1]);
						temp_stats.Range = (float)Convert.ToDouble(split[2]);
						temp_stats.Duration = (float)Convert.ToDouble(split[3]);
						temp_stats.Tick = (float)Convert.ToDouble(split[4]);
						temp_stats.Speed = (float)Convert.ToDouble(split[5]);
						temp_stats.Damage = (float)Convert.ToDouble(split[6]);
						temp_stats.Size = new Point(Convert.ToInt32(split[7]), Convert.ToInt32(split[8]));

					}

					if (typeof(Y) == typeof(BuildingData) || typeof(Y) == typeof(Stats))
					{
						temp_stats.Health = (float)Convert.ToDouble(split[1]);
						temp_stats.ArmourType = (ArmourType)Enum.Parse(typeof(ArmourType), split[3]);
					}

                    if (typeof(Y) == typeof(BuildingData))
                    {
                        temp_stats.Production = Convert.ToInt32(split[2]);
						temp_stats.Size = new Point(Convert.ToInt32(split[6]), Convert.ToInt32(split[7]));
                        temp_stats.Cost = (float)Convert.ToDouble(split[5]);
                        temp_stats.OriginalCost = (float)Convert.ToDouble(split[5]);
                        temp_stats.BuildTime = (float)Convert.ToDouble(split[4]);
                    }

                    if (typeof(Y) == typeof(Stats))
                    {
                        temp_stats.WeaponType = (WeaponType)Enum.Parse(typeof(WeaponType), split[2]);
                        temp_stats.BaseDamage = (float)Convert.ToInt32(split[4]);
                        temp_stats.Range = Convert.ToDouble(split[5]);
                        temp_stats.HitDelay = (float)Convert.ToDouble(split[6]);
                        temp_stats.Luck = (Luck)Enum.Parse(typeof(Luck), split[7]);
                        temp_stats.Focus = (Focus)Enum.Parse(typeof(Focus), split[8]);
						temp_stats.Speed = (float)Convert.ToDouble(split[9]);
                    }
                    
				}

                if (line.StartsWith("Descr"))
                {
                    string[] split = line.Split('\t');
                    string final = "";
                    for (int x = 1; x < split.Length; x++)
                    {
                        final += split[x];
                        final += "\r\n";
                    }

                    temp_stats.Descr = final;

                    if (!stats.ContainsKey(temp_stats.Type))
                    {
                        stats.Add(temp_stats.Type, temp_stats);
                    }
                    else stats[temp_stats.Type] = temp_stats;
                }

                if (line.StartsWith("Tier"))
                {
                    string[] split = line.Split('\t');

                    temp_stats.Tier = (Prerequisites)Enum.Parse(typeof(Prerequisites), split[1]);
                }

                if (line.StartsWith("ReqTier"))
                {
                    string[] split = line.Split('\t');

                    temp_stats.RequiredTier = (Prerequisites)Enum.Parse(typeof(Prerequisites), split[1]);
                }
            }
			return stats;
		}
	}
}
