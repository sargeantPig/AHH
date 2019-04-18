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
using AHH.Extensions;
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
			Ai_States = Ai_States.Idle;
			IsZombie = true;
			home = arch.Home;
		}

		new public void Update(Cursor ms, GameTime gt, Architech arch, Grid grid, Random rng, Overseer os)
		{
			base.Update(gt);
			base.Update();
			CalculateCorners();
			GetFreeCorners(grid);
			foreach (var proj in projectile.Values)
			{
				if(os.Ais.ContainsKey(target))
					proj.Update(((AiUnit)os.Ais[target]).Center, this);
			}

			if (freeCorners.Count < 2)
			{
				Position = arch.GetBuilding(home).AdjacentTiles[0];
				WayPoints.Clear();
				Ai_States = Ai_States.Idle;
			}

			if (Ai_States == Ai_States.Dead)
				return;

			if (Ai_States == Ai_States.Thinking)
			{
				if (PFResult != null)
					Think_Pathing(grid, rng);
				else return;
			}

			if (p_wait)
			{
				elasped += (float)gt.ElapsedGameTime.TotalMilliseconds;

				if (elasped >= waitMax)
				{
					elasped = 0;
					p_wait = false;
				}
			}

			if (Ai_States == Ai_States.Pursue)
			{
				Pursue(os, grid, rng);
				return;
			}

			if (wait)
			{
				AcquireTarget(arch, os, grid, false, rng, false);
				return;
			}

			if (CheckTarget(os))
			{

			}

			switch (Ai_States)
			{
				case Ai_States.Idle:
					Idle(arch, grid, rng); //can go to either move or target
					break;
				case Ai_States.Target:
					AcquireTarget(arch,os, grid, true, rng, false); //can go to idle or move
					break;
				case Ai_States.Moving:
					Moving(os, grid); // can go to idle or attacking
					break;
				case Ai_States.Attacking:
					if(CheckTarget(os)) Attacking(os, arch, gt, rng, grid); // can go to idle or target
					break;

			}

			prevPos = Position;
		}

		protected void AcquireTarget(Architech arch, Overseer os, Grid grid, bool newTarget, Random rng, bool pursue)
		{
			List<Guid> units = new List<Guid>();
			//search for close by enemies
			if (arch.GetBuilding(home) != null)
				units = os.GetUnitsInRange(arch.GetBuilding(home).Center, 264 * 2, true);
			else SetHome(arch, os);

			if (newTarget)
			{
				if (units.Count > 0)
				{
					//attack unit
					target = units.First();
					Vector2 aipos;
					if (os.Ais[target].WayPoints.Count > 0)
						aipos = os.Ais[target].WayPoints.GetRandom(rng);
					else aipos = os.Ais[target].Position;
					var point = Grid.ToGridPosition(aipos, Grid.GetTileSize);
					if (grid.GetTile(point).State == TileStates.Blocked)
						return;

					destination = Grid.ToGridPosition(os.Ais[target].Position, Grid.GetTileSize);
				}

				else
				{ //hover around a grave
					PFResult = null;

					if(home == arch.Home)
						SetHome(arch, os);

					GraveHover(arch, os, grid, rng);
				}
			}


			if (Think_Pathing(grid, rng))
				Ai_States = Ai_States.Thinking;

			if (pathAttempts >= maxPattempts)
			{
				wait = false;
				pathAttempts = 0;
				Ai_States = Ai_States.Target;
				p_wait = true;
			}
		}

		protected void Moving(Overseer os, Grid grid)
		{
			if (WayPoints.Count > 0)
				Moving();
			else
				Ai_States = Ai_States.Target;

			if (CheckTarget(os))
				return;
		}

		//idle around a grave checks surroundings for enemies constantly 
		void GraveHover(Architech arch, Overseer os, Grid grid, Random rnd)
		{
			var grave = arch.GetBuilding(home);

			if (grave == null)
				SetHome(arch, os);

			var b = arch.GetBuilding(home);
			var ts = b.AdjacentTiles;
			destination = Grid.ToGridPosition(ts[rnd.Next(0, ts.Count)], Grid.GetTileSize);
		}

		void Pursue(Overseer os, Grid grid, Random rng)
		{
			if (!wait && os.Ais.ContainsKey(target))
			{

				var aiPos = os.Ais[target].Position;
				var point = Grid.ToGridPosition(aiPos, Grid.GetTileSize);
				if (grid.GetTile(point).State == TileStates.Blocked)
					return;
				destination = Grid.ToGridPosition(os.Ais[target].Position, Grid.GetTileSize);

			}
			if (Think_Pathing(grid, rng))
			{
				if(Ai_States != Ai_States.Moving)
					Ai_States = Ai_States.Thinking;

			}
		}

		//sets a home grave for the zombie a grave it will return to in less violent times
		void SetHome(Architech arch, Overseer os)
		{
			home = arch.GetGrave(os);
		}

		bool CheckTarget(Overseer os)
		{
			if (os.Ais.ContainsKey(target))
			{
				if (os.IsInRange(Corners, target, (float)Stats.Range) && os.Ais[target].Ai_States != Ai_States.Dead)
				{
					Ai_States = Ai_States.Attacking;
					return true;
				}

				else if (WayPoints.Count == 0)
				{
					Ai_States = Ai_States.Pursue;
				}

				else if (WayPoints.Count > 0)
					Ai_States = Ai_States.Moving;

				if (os.Ais[target].Ai_States == Ai_States.Dead)
				{
					target = new Guid();
					Ai_States = Ai_States.Idle;
					projectile.Clear();
					removeProj_queue.Clear();
				}
			}

			else
			{
				target = new Guid();
			
			}

			return false;
		}

		//attack target once in range, go back to pursue if not in range
		void Attacking(Overseer os, Architech arch, GameTime gt, Random rng, Grid grid)
		{
			hitElasped += gt.ElapsedGameTime.Milliseconds;

			if (hitElasped >= Stats.HitDelay)
			{
				hitElasped = 0;
				os.Combat(this, (Grounded)os.Ais[target], rng);
				var p = new Projectile(Position, new Point(16, 16), data.Projectile, 5);
				projectile.Add(p.ID, p);
			}
			
		}

		public Point Home
		{
			get { return home; }
			set { home = value; }
		}

	}
}
