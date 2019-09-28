using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AHH.User;
using AHH.UI;
using AHH.UI.Elements;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace AHH.Parsers
{
	static partial class Parsers
	{
		public static Dictionary<Player_Modes, List<IElement>> Parse_Ui_Master(string filepath, ContentManager cm)
		{
			if (!File.Exists(filepath))
            {
                throw new Exception("Cannot locate file at: " + filepath);
                return null;
            }

			StreamReader sr = new StreamReader(filepath);
			string line = "";

			Dictionary<Player_Modes, List<IElement>> ndic = new Dictionary<Player_Modes, List<IElement>>();
			Player_Modes mode = Player_Modes.Building;
			Align align = Align.Horizontal;
			string sm = "";

			Vector2 loc = new Vector2();
			Point size = new Point();
			while ((line = sr.ReadLine()) != null)
			{
				if (line.StartsWith("Mode"))
				{
					string[] split = line.Split('\t');
					mode = (Player_Modes)Enum.Parse(typeof(Player_Modes), split[1]);

					if (!ndic.ContainsKey(mode))
						ndic.Add(mode, new List<IElement>());
				}

				if (line.StartsWith("ParseType"))
				{
					string[] split = line.Split('\t');
					sm = split[1];
				}

				if (sm == "single")
				{
					if (line.StartsWith("File"))
					{
						string[] split = line.Split('\t');
						var data = Parse_UiElements(split[1], cm);

						foreach (IElement ie in data.Values)
						{
							ndic[mode].Add(ie);
						}
					}


				}

				else if (sm == "multi")
				{
					if (line.StartsWith("Location"))
					{
						string[] split = line.Split('\t',',');
						loc = new Vector2(Convert.ToInt32(split[1]), Convert.ToInt32(split[2]));
					}

					if (line.StartsWith("Size"))
					{
						string[] split = line.Split('\t', ',');
						size = new Point(Convert.ToInt32(split[1]), Convert.ToInt32(split[2]));
					}

					if (line.StartsWith("Align"))
					{
						string[] split = line.Split('\t');
						align = (Align)Enum.Parse(typeof(Align), split[1]);
					}

					if (line.StartsWith("File"))
					{
						string[] split = line.Split('\t');
                        Dictionary<string, IElement> data;
                        if (!split[1].Contains("Nest"))
                            data = Parse_UiElements(split[1], cm);
                        else
                        {
                            var temp = Parse_Ui_Master(split[1], cm);

                            Dictionary<string, IElement> correction = new Dictionary<string, IElement>();

                            foreach (var e in temp.Values)
                            {
                                foreach (var b in e)
                                {
                                    correction.Add(Guid.NewGuid().ToString(), b);
                                }
                            }
                            data = correction;
                        };

                        

						ndic[mode].Add(new Strip(loc, true, size, align, data));
					}
				}

			}

			return ndic;
		}
	}
}
