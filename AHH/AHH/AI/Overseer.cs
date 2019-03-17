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
		float s_elasped = 100000;
		float s_target = 100000;

		public Overseer(ContentManager cm)
		{
			ais = new List<Ai>();
			ai_stats = Parsers.Parsers.Parse_UnitStats(@"Content\unit\unit_descr.txt");
			unit_types = Parsers.Parsers.Parse_UnitTypes(@"Content\unit\unit_types.txt", cm);
		}

		public void Update(GameTime gt, Cursor ms, Grid grid)
		{
			if (grid.BuildingPlaced)
			{
				foreach (Ai ai in ais)
				{
					ai.Ai_States = Ai_States.Thinking;
				}
			}


			foreach (Ai ai in ais)
			{
				if (ai is Grounded)
					((Grounded)ai).Update(ms, gt, grid);
				else if (ai is AiUnit)
					ai.Update(ms);
			}

			s_elasped += gt.ElapsedGameTime.Milliseconds;
			if(s_elasped > s_target)
			{
				Spawn(grid);
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

		void Spawn(Grid grid)
		{
			Stats stats = new Stats(ai_stats[Ai_Type.Knight][0]);
			Unit_Types ut = new Unit_Types(unit_types[Ai_Type.Knight]) ;

			ais.Add(new Grounded(new Vector2(100, 100), new Point(64, 64), 1, ut, stats, grid));
		}

		public List<Ai> Ais
		{
			get { return ais; }
		}

	}
}
