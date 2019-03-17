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

	}
}
