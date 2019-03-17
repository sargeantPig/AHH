using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using AHH.Interactable;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AHH.UI;
using AHH.AI;
namespace AHH.Parsers
{
	static class Parsers
	{
		public static Dictionary<string, Building> Parse_Buildings(string filepath, ContentManager cm)
		{
			StreamReader sr = new StreamReader(filepath);
			string line = "";
			Dictionary<string, Building> buildings = new Dictionary<string, Building>();

			string temp_name = "";
			int temp_cost = 0;
			Point temp_size = new Point();
			string[] temp_texturePath = new string[3];

			while ((line = sr.ReadLine()) != null)
			{
				if (line.StartsWith("name"))
				{
					string[] split = line.Split('\t');
					temp_name = split[1].Trim();
				}

				if (line.StartsWith("cost"))
				{
					string[] split = line.Split('\t');
					temp_cost = Convert.ToInt16(split[1].Trim());
				}

				if (line.StartsWith("size"))
				{
					string[] split = line.Split('\t', ',');
					temp_size.X = Convert.ToInt16(split[1].Trim());
					temp_size.Y = Convert.ToInt16(split[2].Trim());
				}

				if (line.StartsWith("texture"))
				{
					string[] split = line.Split('\t');
					temp_texturePath[0] = split[1].Trim();

				}

				if (line.StartsWith("H_texture"))
				{
					string[] split = line.Split('\t');
					temp_texturePath[1] = split[1].Trim();

				}

				if (line.StartsWith("C_texture"))
				{
					string[] split = line.Split('\t');
					temp_texturePath[2] = split[1].Trim();

					buildings.Add(temp_name, new Building(new Vector2(0, 0), cm.Load<Texture2D>(temp_texturePath[0]), cm.Load<Texture2D>(temp_texturePath[1]), cm.Load<Texture2D>(temp_texturePath[2]), temp_size));
				}



			}

			return buildings;

		}

		public static Dictionary<string, Control> Parse_UiElements(string filepath, ContentManager cm)
		{
			if (!File.Exists(filepath))
				return null;

			StreamReader sr = new StreamReader(filepath);
			string line = "";
			Dictionary<string, Control> elements = new Dictionary<string, Control>();

			string temp_type = "";
			string temp_name = "";
			string temp_value = "";
			string texture_path = "";
			Point temp_size = new Point();
			string[] temp_texturePath = new string[3];


			while ((line = sr.ReadLine()) != null)
			{
				if (line.StartsWith("Type"))
				{
					string[] split = line.Split('\t');
					temp_type = split[1];

				}

				if (line.StartsWith("Name"))
				{
					string[] split = line.Split('\t');
					temp_name = split[1];

				}
				if (line.StartsWith("Value"))
				{
					string[] split = line.Split('\t');
					temp_value = split[1];

				}

				if (line.StartsWith("Size"))
				{
					string[] split = line.Split('\t', ',');
					temp_size.X = Convert.ToInt32(split[1]);
					temp_size.Y = Convert.ToInt32(split[2]);
				}

				if (line.StartsWith("H_texture"))
				{
					string[] split = line.Split('\t');
					temp_texturePath[1] = split[1].Trim();

				}


				if (line.StartsWith("Texture"))
				{
					string[] split = line.Split('\t');
					temp_texturePath[0] = split[1];
					Texture2D texture = cm.Load<Texture2D>(temp_texturePath[0]);
					Texture2D texture_h = cm.Load<Texture2D>(temp_texturePath[1]);
					elements.Add(temp_name, new Button(new Vector2(0, 0), temp_size, texture, texture_h, texture, temp_value));
				}

			}

			return elements;




		}

		public static Dictionary<ButtonFunction, string> Parse_InternalGridMenu(string filepath)
		{
			if (!File.Exists(filepath))
				return null;

			Dictionary<ButtonFunction, string> actions = new Dictionary<ButtonFunction, string>();
			StreamReader sr = new StreamReader(filepath);
			string line = "";

			while ((line = sr.ReadLine()) != null)
			{
				if (!line.StartsWith("#"))
				{
					string[] split = line.Split('\t');
					actions.Add((ButtonFunction)Enum.Parse(typeof(ButtonFunction), split[1]), split[0]);
				}

			}

			return actions;

		}

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

					stats[temp_stats.AiType].Add(temp_stats);
				}

				

			}

			return stats;

		}

		public static Dictionary<Ai_Type, Unit_Types> Parse_UnitTypes(string filepath, ContentManager cm)
		{
			if (!File.Exists(filepath))
				return null;

			Dictionary<Ai_Type, Unit_Types> unit_types = new Dictionary<Ai_Type, Unit_Types>();
			StreamReader sr = new StreamReader(filepath);
			string line = "";

			Unit_Types temp_types = new Unit_Types();

			while ((line = sr.ReadLine()) != null)
			{
				if (line.StartsWith("Type"))
				{
					string[] split = line.Split('\t');
					temp_types.Type = (Ai_Type)Enum.Parse(typeof(Ai_Type), split[1]);
					if (!unit_types.ContainsKey(temp_types.Type))
						unit_types.Add(temp_types.Type, new Unit_Types());
				}

				if (line.StartsWith("Texture"))
				{
					string[] split = line.Split('\t');
					temp_types.Texture = cm.Load<Texture2D>(split[1]);
				}

				if (line.StartsWith("H_Texture"))
				{
					string[] split = line.Split('\t');
					temp_types.H_texture = cm.Load<Texture2D>(split[1]);
				}

				if (line.StartsWith("C_Texture"))
				{
					string[] split = line.Split('\t');
					temp_types.C_texture = cm.Load<Texture2D>(split[1]);
				}

				if (line.StartsWith("Projectile"))
				{
					string[] split = line.Split('\t');
					temp_types.Projectile = cm.Load<Texture2D>(split[1]);
				}

				if (line.StartsWith("Animation"))
				{
					Dictionary<string, Vector3> temp_animations = new Dictionary<string, Vector3>();
					while ((line = sr.ReadLine()) != null)
					{
						string[] split = line.Split(',');

						if (!temp_animations.ContainsKey(split[0]))
						{
							temp_animations.Add(split[0], new Vector3(Convert.ToInt32(split[1]), Convert.ToInt32(split[2]),(float)Convert.ToDouble(split[3])));
						}

					}

					temp_types.Animations = temp_animations;
					unit_types[temp_types.Type] = temp_types;
				}

				
			}

			return unit_types;
		}
	}
}
