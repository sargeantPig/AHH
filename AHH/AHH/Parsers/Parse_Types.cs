using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using AHH.Interactable;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AHH.UI;
using AHH.AI;
using AHH.Interactable.Building;
using AHH.Base;
namespace AHH.Parsers
{
	static partial class Parsers
	{
		public static Dictionary<T, Y> Parse_Types<T, Y>(string filepath, ContentManager cm)
		{
            if (!File.Exists(filepath))
            {
                throw new Exception("Cannot locate file at: " + filepath);
                return null;
            }


            Dictionary<T, Y> types = new Dictionary<T, Y>();
			StreamReader sr = new StreamReader(filepath);
			string line = "";

            dynamic temp_types = new Type_Data<T>(); //assign def

            Type y = typeof(Y);


			while ((line = sr.ReadLine()) != null)
			{
				if (line.StartsWith("Type"))
				{
					string[] split = line.Split('\t');
					temp_types.Type = (T)Enum.Parse(typeof(T), split[1]);
					if (!types.ContainsKey(temp_types.Type))
						types.Add(temp_types.Type, temp_types);
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
					types[temp_types.Type] = temp_types;
				}


			}

			return types;

		}
	}

}

