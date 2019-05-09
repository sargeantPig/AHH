using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.Research
{
	//denotes what to modify
	enum Researchables
	{
		ZHealth,
		ZSpeed,
		ZDamage,
        ZRange,
		WHealth,
		WProduct,
		WCost,
		SPower,
		SLength,
		SSpeed,
        AiHealth,
        AiSpeed,
        AiDamage,
        AiRange
	}

	enum ResearchState
	{
        Waiting,
		Researching,
		Done
	}

	struct ResearchData
	{
		string name { get; set; }
		List<KeyValuePair<Researchables, float>> modifiers { get; set; }
		float researchTime { get; set; }
        Texture2D texture { get; set; }

		public ResearchData(ResearchData rd)
		{
			this.modifiers = rd.modifiers;
			this.name = rd.name;
			this.researchTime = rd.researchTime;
            this.texture = rd.texture;
		}

		public string Name
		{

			get { return name; }
			set { this.name = value; }
		}

		public float ResearchTime
		{
			get { return this.researchTime; }
			set { this.researchTime = value; }
		}

		public List<KeyValuePair<Researchables, float>> Modifiers
		{
			get { return modifiers; }
			set { this.modifiers = value; }
			
		}

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
		
	}


}
