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
		public static Dictionary<string, IElement> Parse_UiElements(string filepath, ContentManager cm)
		{
			if (!File.Exists(filepath))
				return null;

			StreamReader sr = new StreamReader(filepath);
			string line = "";
			Dictionary<string, IElement> elements = new Dictionary<string, IElement>();

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

	}
}
