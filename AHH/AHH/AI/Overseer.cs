using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.UI;
using AHH.Parsers;
using Microsoft.Xna.Framework.Graphics;
using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace AHH.AI
{
	//sets up and manages AIs, including creation and deleting.
	class Overseer
	{
		List<Ai> ais { get; }
		Dictionary<Ai_Type, List<Stats>> ai_stats { get; }
		Dictionary<Ai_Type, Unit_Types> unit_types { get; }
		float s_elasped = 0;
		float s_target = 100;

		public Overseer(ContentManager cm)
		{
			ai_stats = Parsers.Parsers.Parse_UnitStats(@"Content\unit\unit_descr.txt");
			unit_types = Parsers.Parsers.Parse_UnitTypes(@"Content\unit\unit_types.txt", cm);
		}

		public void Update(GameTime gt, Cursor ms, Grid grid)
		{
			foreach (Ai ai in ais)
			{
				if (ai is AiUnit)
					ai.Update(ms);
				else if (ai is Grounded)
					((Grounded)ai).Update(ms, grid);
			}

			s_elasped += gt.ElapsedGameTime.Milliseconds;
			if(s_elasped > s_target)
			{
				Spawn();
				s_elasped = 0;
			}
		}

		public void Draw(SpriteBatch sb)
		{
			foreach (Ai ai in ais)
			{
				ai.Draw(sb);
			}
		}

		void Spawn()
		{
			Stats stats = ai_stats[Ai_Type.Knight][0];
			Unit_Types unit_Types = unit_types[Ai_Type.Knight];

			ais.Add(new Grounded(new Vector2(), ))

		}

		public List<Ai> Ais
		{
			get { return ais; }
		}

	}
}
