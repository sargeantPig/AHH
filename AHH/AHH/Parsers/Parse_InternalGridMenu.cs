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
	partial class Parsers
	{
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

	}
	}
