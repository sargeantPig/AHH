using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using AHH.Interactable.Building;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AHH.UI;
using AHH.AI;
namespace AHH.Parsers
{

	static partial class Parsers
	{
		public static Dictionary<T, List<Y>> Parse_Stats<T, Y>(string filepath)
		{
			if (!File.Exists(filepath))
				return null;

			Dictionary<T, List<Y>> stats = new Dictionary<T, List<Y>>();
			StreamReader sr = new StreamReader(filepath);
			string line = "";

			dynamic temp_stats = new Stats(); //default

            if (typeof(Y) == typeof(BuildingData))
                temp_stats = new BuildingData();
            else if (typeof(Y) == typeof(Stats))
                temp_stats = new Stats();

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
					if (!stats.ContainsKey(temp_stats.Type))
						stats.Add(temp_stats.Type, new List<Y>());
				}

				if (line.StartsWith("Stats"))
				{
					string[] split = line.Split('\t', ',');

                    temp_stats.Health = Convert.ToInt32(split[1]);
                    temp_stats.ArmourType = (ArmourType)Enum.Parse(typeof(ArmourType), split[3]);

                    if (typeof(Y) == typeof(BuildingData))
                    {
                        temp_stats.Production = Convert.ToInt32(split[2]);
						temp_stats.Size = new Point(Convert.ToInt32(split[4]), Convert.ToInt32(split[5]));
                    }

                    if (typeof(Y) == typeof(Stats))
                    {
                        temp_stats.WeaponType = (WeaponType)Enum.Parse(typeof(WeaponType), split[2]);
                        temp_stats.BaseDamage = Convert.ToInt32(split[4]);
                        temp_stats.Range = Convert.ToDouble(split[5]);
                        temp_stats.HitDelay = (float)Convert.ToDouble(split[6]);
                        temp_stats.Luck = (Luck)Enum.Parse(typeof(Luck), split[7]);
                        temp_stats.Focus = (Focus)Enum.Parse(typeof(Focus), split[8]);
                    }
					stats[temp_stats.Type].Add(temp_stats);
				}

			}
			return stats;
		}
	}
}
