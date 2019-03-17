using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using AHH.Interactable;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AHH.UI;
using AHH.AI;
namespace AHH.Parsers
{

	static partial class Parsers
	{
		public static Dictionary<Ai_Type, List<Stats>> Parse_UnitStats(string filepath)
		{
			if (!File.Exists(filepath))
				return null;

			Dictionary<Ai_Type, List<Stats>> stats = new Dictionary<Ai_Type, List<Stats>>();
			StreamReader sr = new StreamReader(filepath);
			string line = "";

			Stats temp_stats = new Stats();

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
					temp_stats.AiType = (Ai_Type)Enum.Parse(typeof(Ai_Type), split[1]);
					if (!stats.ContainsKey(temp_stats.AiType))
						stats.Add(temp_stats.AiType, new List<Stats>());
				}

				if (line.StartsWith("Stats"))
				{
					string[] split = line.Split('\t', ',');
					temp_stats.Health = Convert.ToInt32(split[1]);
					temp_stats.WeaponType = (WeaponType)Enum.Parse(typeof(WeaponType), split[2]);
					temp_stats.ArmourType = (ArmourType)Enum.Parse(typeof(ArmourType), split[3]);
					temp_stats.BaseDamage = Convert.ToInt32(split[4]);
					temp_stats.Range = Convert.ToInt32(split[5]);
					temp_stats.HitDelay = (float)Convert.ToDouble(split[6]);
					temp_stats.Luck = (Luck)Enum.Parse(typeof(Luck), split[7]);
					temp_stats.Focus = (Focus)Enum.Parse(typeof(Focus), split[8]);
					stats[temp_stats.AiType].Add(temp_stats);
				}



			}

			return stats;

		}

	}
}
