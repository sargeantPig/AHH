using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Research;
using AHH.UI;
using AHH.UI.Elements.Buttons;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AHH.Parsers
{
    partial class Parsers
    {
        public static Dictionary<ButtonFunction, Dictionary<string, Research.Research>> ParseResearch(string filepath, ContentManager cm)
        {
            if (!File.Exists(filepath))
            {
                throw new Exception("Cannot locate file at: " + filepath);
                return null;
            }


            Dictionary<ButtonFunction, Dictionary<string, Research.Research>> dic = new Dictionary<ButtonFunction, Dictionary<string, Research.Research>>();
            
            StreamReader sr = new StreamReader(filepath);
            string line = "";

            var temp_data = new ResearchData();
            List<KeyValuePair<Researchables, float>> res = new List<KeyValuePair<Researchables, float>>();
            ButtonFunction bf = ButtonFunction.R1;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("ButtonName"))
                {
                    string[] split = line.Split('\t');
                    bf = (ButtonFunction)Enum.Parse(typeof(ButtonFunction), split[1]);
                }

                else if (line.StartsWith("ResearchName"))
                {
                    string[] split = line.Split('\t');
                    temp_data.Name = split[1];
                   
                }

                else if (line.StartsWith("Cost"))
                {
                    string[] split = line.Split('\t');
                    temp_data.ResearchTime = (float)Convert.ToDouble(split[1]);
                }

                else if (line.StartsWith("Texture"))
                {
                    string[] split = line.Split('\t');
                    temp_data.Texture = cm.Load<Texture2D>(split[1]);
                }


                else if (line.StartsWith("Researchables"))
                {
                    while ((line = sr.ReadLine()) != "RE")
                    {
                        string[] split = line.Split('\t');
                        res.Add(new KeyValuePair<Researchables, float>((Researchables)Enum.Parse(typeof(Researchables), split[0]), (float)Convert.ToDouble(split[1])));
                    }

                    temp_data.Modifiers = new List<KeyValuePair<Researchables, float>>(res);

                    if (!dic.ContainsKey(bf))
                    {
                        dic.Add(bf, new Dictionary<string, Research.Research>());
                        dic[bf].Add(temp_data.Name, new Research.Research(temp_data));
                    }

                    else
                    {
                        dic[bf].Add(temp_data.Name, new Research.Research(temp_data));
                    }

                    res.Clear();
                }

            }

            return dic;

        }

    }
}
