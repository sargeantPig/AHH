using AHH.Base;
using AHH.Interactable.Building;
using AHH.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.AI
{
	class Zombie : Grounded
	{
		Point home { get; set; }
		Guid target;
		float hitElasped = 0;

		const float stuckAttempts = 100;
		int attempts = 0;

		

		public Zombie(Vector2 position, Point rectExtends, Type_Data<Ai_Type> types, Stats stats, Grid grid, Architech arch)
			: base(position, rectExtends, types, stats, grid)
		{
			Ai_States = Ai_States.Thinking;
			IsZombie = true;
			home = arch.Home;
		}

		new public void Update(Cursor ms, GameTime gt, Architech arch, Grid grid, Random rng, Overseer os)
		{
			base.Update();

			if (Ai_States == Ai_States.Dead)
				return;

			CalculateCorners();

			GetFreeCorners(grid);

			if (wait)
			{
				Think_Pathing(grid);
				return;
			}

			if (home == arch.Home)
				SetHome(arch, os);

			switch (nextState)
			{
				case Ai_States.Attacking: if (CheckTarget(os, grid))
					{ Ai_States = Ai_States.Attacking; }; break;
			}

			switch (Ai_States)
			{
				case Ai_States.Thinking: CurrentState = "Think"; Think(arch, os, grid, rng); break;
				case Ai_States.Moving: CurrentState = "Move"; Moving(); break;
				case Ai_States.Attacking: CurrentState = "Attack"; Attacking(os, arch, gt, rng, grid); break;
			}

			prevPos = Position;

			freeCorners.Clear();
		}

		protected void Think(Architech arch, Overseer os, Grid grid, Random rng)
		{

			//Work out preffered action - taking into account hp and focus.
			var points = PrefferedAction();

			List<Focus> bag = new List<Focus>();
			foreach (KeyValuePair<Focus, int> kv in points)
			{
				for (int x = 0; x < kv.Value; x++)
					bag.Add(kv.Key);
			}

			int choice = rng.Next(0, bag.Count);

			switch (bag[choice])
			{
				case Focus.Focused:
					nextState = Ai_States.Thinking;
					GraveHover(arch, os, grid, rng);
					break;
				case Focus.Aggressive:
					nextState = Ai_States.Thinking;
					PFResult = null;
					break;
				case Focus.Hyper:
					nextState = Ai_States.Thinking;
					GraveHover(arch, os, grid, rng);
					PFResult = null;
					break;
				case Focus.Violent:
					nextState = Ai_States.Attacking;
					PFResult = null; 
					break;

			}

			bag.Clear();
		}

		//idle around a grave checks surroundings for enemies constantly 
		void GraveHover(Architech arch, Overseer os, Grid grid, Random rnd)
		{
			var grave = arch.GetBuilding(home);

			var enemies = os.GetUnitsInRange(Position, 5 * 64, true);

			if (enemies.Count > 0)
			{
				target = enemies[0];
				Pursue(os.Ais[enemies[0]].Position, grid);
			}

			else
			{
				var b = arch.GetBuilding(home);
				var ts = b.AdjacentTiles;
				destination = Grid.ToGridPosition(ts[rnd.Next(0, ts.Count)], Grid.GetTileSize);
				nextState = Ai_States.Thinking;
				Think_Pathing(grid);

			}

		}

		//sets a home grave for the zombie a grave it will return to in less violent times
		void SetHome(Architech arch, Overseer os)
		{
			home = arch.GetGrave(os);
		}

		//targets an enemy that enters range and pursues them relentlessly, 
		//uses the enemies previous position then fetches a new one once that destination is reached
		void Pursue(Vector2 targetPos, Grid grid )
		{
			nextState = Ai_States.Attacking;
			destination = Grid.ToGridPosition(targetPos, Grid.GetTileSize);
			PFResult = null;
			Think_Pathing(grid);
		}

		bool CheckTarget(Overseer os, Grid grid)
		{
			if (os.Ais.ContainsKey(target))
			{
				if (os.Ais[target].Ai_States != Ai_States.Dead)
				{
					if (os.IsInRange(Corners, target, (float)Stats.Range))
					{
						WayPoints.Clear();
						return true;
						
					}

					else if (WayPoints.Count == 0)
					{
						Pursue(os.Ais[target].Position, grid);
					}

					else if (WayPoints.Count > 0)
						Ai_States = Ai_States.Moving;
				}

				else { target = new Guid(); Ai_States = Ai_States.Thinking; nextState = Ai_States.Thinking; }
			}

			else { Ai_States = Ai_States.Thinking; nextState = Ai_States.Thinking; }

			return false;
		}


		//attack target once in range, go back to pursue if not in range
		void Attacking(Overseer os, Architech arch, GameTime gt, Random rng, Grid grid)
		{
			hitElasped += gt.ElapsedGameTime.Milliseconds;

			if (hitElasped >= Stats.HitDelay)
			{
				hitElasped = 0;
				os.Combat<Grounded>(this, (Grounded)os.Ais[target], rng);
			}
		}

		public Point Home
		{
			get { return home; }
			set { home = value; }
		}

	}
}
