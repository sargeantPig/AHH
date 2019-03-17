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
					while ((line = sr.ReadLine()) != "" && line != null )
					{
						string[] split = line.Split(',');

						if (!temp_animations.ContainsKey(split[0]))
						{
							temp_animations.Add(split[0], new Vector3(Convert.ToInt32(split[1]), Convert.ToInt32(split[2]), (float)Convert.ToDouble(split[3])));
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

