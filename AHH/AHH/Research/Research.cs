using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.Research
{
	class Research
	{
		ResearchState state { get; set; }
		ResearchData data { get; set; }
		float currentProgress;


		public Research(ResearchData rd)
		{
			data = rd;
			state = ResearchState.Researching;
		}

		public void Update(GameTime gt, Researcher rs)
		{
			if (state == ResearchState.Done)
				return;

			

		}

		public ResearchData Data
		{
			get { return this.data; }
		}
	}
}
