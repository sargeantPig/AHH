using System;
using System.Collections.Generic;
using System.Linq;
using AHH.Base;
using AHH.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AHH.Interactable.Building;
using AHH.Extensions;

namespace AHH.AI
{
	class Grounded : AiUnit
	{
		int pathAttempts = 0;
		protected bool wait { get; set; }
		protected Point destination = new Point(10, 10);
		Point defenderID = new Point();
        Guid attackerID = new Guid();
		protected Ai_States nextState = Ai_States.Thinking;
		protected Vector2 prevPos = new Vector2();
		float hitElasped = 0;

		public Grounded(Vector2 position, Point rectExtends, Type_Data<Ai_Type> types, Stats stats, Grid grid)
			: base(position, rectExtends, stats.Speed, types.Animations, stats, types, grid)
		{
           
		}

		public void Update(Cursor ms, GameTime gt, Architech arch, Grid grid, Random rng, Overseer os)
		{
			base.Update(gt);
			base.Update();
			CalculateCorners();

			GetFreeCorners(grid);

			if (Ai_States == Ai_States.Retaliating)
			{
				Fighting(os, gt, rng);
				CurrentState = "Attack";
				return;
			}
			if (Ai_States == Ai_States.Dead)
			{
				CurrentState = "Dead";
				return;
			}
				

			if (grid.CheckPositions(new List<Vector2>() { Position }).Count == 0)
			{
				Position = prevPos;
				var nearCheck = arch.NearBuildings(Position, 1f);
				if (arch.NearBuildings(Position, 1f).Count != 0 && Ai_States == Ai_States.Thinking)
				{
					Ai_States = Ai_States.Attacking;
					defenderID = nearCheck.Keys.ElementAt(0);
					Attacking(os, arch, gt, rng);
					return;
				}
			}

			if (arch.GetBuilding(defenderID) != null)
			{
				if (arch.IsInRange(Corners, defenderID, (float)Stats.Range) && nextState == Ai_States.Attacking)
				{
					Ai_States = Ai_States.Attacking;
				}
			}

			if (wait)
			{
				Think_Pathing(grid);
				return;
			}

			switch (Ai_States)
			{
				case Ai_States.Thinking: CurrentState = "Think"; Think(arch, grid, rng); break;
				case Ai_States.Resurrecting: Ressurect(); break;
				case Ai_States.Moving: CurrentState = "Move"; Moving(); break;
				case Ai_States.Attacking: CurrentState = "Attack"; Attacking(os, arch, gt, rng); break;
			}

			prevPos = Position;
		}

		void Attacking(Overseer os, Architech arch, GameTime gt, Random rng)
		{
			hitElasped += gt.ElapsedGameTime.Milliseconds;
						
			if (arch.GetBuilding(defenderID) != null)
			{
				if (arch.IsInRange(Corners, defenderID, (float)Stats.Range))
				{
					if (hitElasped >= Stats.HitDelay)
					{
						os.Combat(this, arch.GetBuilding(defenderID), rng);
						hitElasped = 0;
					}
				}

				else { nextState = Ai_States.Thinking; Ai_States = Ai_States.Thinking; }
			}
			else
			{
				nextState = Ai_States.Thinking;
				Ai_States = Ai_States.Thinking;
			}
		}

		void Fighting(Overseer os, GameTime gt, Random rng)
		{
			if (os.Zombies.ContainsKey(attackerID))
			{
				if (os.Zombies[attackerID].Ai_States != Ai_States.Dead)
				{
					if (!os.ZIsInRange(Corners, attackerID, (float)Stats.Range))
					{
						Ai_States = Ai_States.Thinking;
					}

					else
					{
						Ai_States = Ai_States.Retaliating;

						hitElasped += gt.ElapsedGameTime.Milliseconds;

						if (hitElasped >= Stats.HitDelay)
						{
							hitElasped = 0;
							os.Combat<Grounded>(this, (Grounded)os.Zombies[attackerID], rng);
						}
					}

				}
				else { nextState = Ai_States.Thinking; Ai_States = Ai_States.Thinking; }

			}
			else { nextState = Ai_States.Thinking; Ai_States = Ai_States.Thinking; }


		}

		public void Retaliate(Guid atkid)
        {
			this.attackerID = atkid;
			Ai_States = Ai_States.Retaliating;
        }


		protected void Moving( )
		{
			if (WayPoints.Count == 0)
			{
				Ai_States = nextState;
				return;
			}

			bool reached = MoveTo(WayPoints[0]);
			if (reached)
				WayPoints.RemoveAt(0);

		}

		protected  void Think(Architech arch, Grid grid, Random rng)
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
					//nextState = Ai_States.Thinking;
					destination = Grid.ToGridPosition(arch.GetBuilding(arch.Home).AdjacentTiles[0], Grid.GetTileSize);
					Think_Pathing(grid);
					break;
				case Focus.Aggressive:
					nextState = Ai_States.Thinking;
					PFResult = null; Think_Forward(grid);
					break;
				case Focus.Hyper:
					nextState = Ai_States.Thinking;
					PFResult = null;
					break;
				case Focus.Violent:
					nextState = Ai_States.Attacking;
					PFResult = null; Think_Violence(arch, grid,rng);
					break;

			}

            bag.Clear();
		}

		void Think_Violence(Architech arch, Grid grid, Random rng)
		{

			WayPoints.Clear();
			//check if buildings are nearby if path is blocke
			Dictionary<Point, Building> near = arch.NearBuildings(Position, 5);
			Vector2 closestBuildingPos = new Vector2(int.MaxValue, int.MaxValue);
			float min = 99999;

			if (near.Count == 0)
			{
				Ai_States = Ai_States.Thinking;
				return;
			}

			Building temp = new Building();
			foreach (KeyValuePair<Point, Building> kv in near)
			{
				float dis = Vector2.Distance(Position, kv.Value.Position);

				if (dis < min && kv.Value.AdjacentTiles != null)
				{
					min = dis;
					temp = kv.Value;
					defenderID = kv.Key;
					closestBuildingPos = kv.Value.Position;
				}
			}

			if (closestBuildingPos.X != int.MaxValue)
			{
				//get adjacent points
				List<Vector2> edges = temp.AdjacentTiles;

				edges = new List<Vector2>(grid.CheckPositions(edges));

				if (edges == null || edges.Count == 0)
				{
					Ai_States = Ai_States.Thinking;
					return;
				}

				Vector2 closestEdge = Position.ClosestVector(edges);
				destination = Grid.ToGridPosition(edges[rng.Next(0, edges.Count - 1)], Grid.GetTileSize);
				//destination = Grid.ToGridPosition(closestEdge, Grid.GetTileSize);
				Think_Pathing(grid);
			}

			else { Ai_States = Ai_States.Thinking; Ai_States = nextState = Ai_States.Thinking; }
		}

		protected void Think_Pathing(Grid grid)
		{
			WayPoints.Clear();

			if (PFResult != null)
			{
				if (PFResult.GetType() == typeof(TileStates))
				{
					wait = false;
					nextState = Ai_States.Thinking;
					Ai_States = Ai_States.Thinking;
					return;
				}

				wait = false;
				WayPoints = CalculateWayPoints(PFResult);

				if (grid.CheckPositions(WayPoints).Count != WayPoints.Count) //theres a break in a waypoint
                {
					PFResult = null;
					nextState = Ai_States.Thinking;
                    Ai_States = Ai_States.Thinking;
                    WayPoints.Clear();
                }
                 
                else Ai_States = Ai_States.Moving;
				//Ai_States = Ai_States.Moving;
                //Pathfinder = null;
			}
			else
			{
				int counter = 0;
				if (!GetPath(grid, freeCorners[0], destination))
				{
					CheckActivity();
					pathAttempts++;
				}

				else { wait = false; pathAttempts = 0; }

				wait = true;

				if (pathAttempts > 20)
				{
					pathAttempts = 0;
					Ai_States = Ai_States.Thinking;
					nextState = Ai_States.Thinking;
					wait = false;
				}
			}

		}

		void Think_Forward(Grid grid)
		{
			WayPoints.Clear();
			//check if can move forward
			Vector2 a_nextLocation = Position;

			Point gridLocation = Grid.ToGridPosition(Position, Grid.GetTileSize);
			Tile nextTile = grid.GetTile(new Point(gridLocation.X + 1, gridLocation.Y));

			if (nextTile != null)
			{
				if (nextTile.State != TileStates.Blocked && nextTile.State != TileStates.Immpassable)
					a_nextLocation = nextTile.Position;
				WayPoints.Add(a_nextLocation);
				nextState = Ai_States.Thinking;
				Ai_States = Ai_States.Moving;
			}

			else
			{

				Ai_States = Ai_States.Thinking;
			}

		}

		protected Dictionary<Focus, int> PrefferedAction()
		{

			Dictionary<Focus, int> focusPoints = new Dictionary<Focus, int>()
			{
				{ Focus.Aggressive, 0},
				{ Focus.Focused, 0},
				{ Focus.Hyper, 0},
				{ Focus.Violent, 0}
			};

			//0 = focused
			//1 = aggressive
			//2 = hyper
			//3 = violent

			//gather focus points
			switch (Stats.Focus)
			{
				case Focus.Focused:
					focusPoints[Focus.Focused] += 3;
					break;
				case Focus.Aggressive:
					focusPoints[Focus.Aggressive] += 3;
					break;
				case Focus.Hyper:
					focusPoints[Focus.Hyper] += 3;
					break;
				case Focus.Violent:
					focusPoints[Focus.Violent] += 3;
					break;
			}

			//apply percent of lost health
			//points -= (int)Health.PercentDecrease(Stats.Health);

			//apply unit type
			switch (Stats.Type)
			{
				case Ai_Type.Archer:
					focusPoints[Focus.Focused] += 3; focusPoints[Focus.Hyper] += 1;
					focusPoints[Focus.Aggressive] += 2; focusPoints[Focus.Violent] += 2;
					break;
				case Ai_Type.Horseman:
					focusPoints[Focus.Focused] += 5; focusPoints[Focus.Hyper] += 2;
					focusPoints[Focus.Aggressive] += 2; focusPoints[Focus.Violent] += 1;
					break;
				case Ai_Type.Knight:
					focusPoints[Focus.Focused] += 2; focusPoints[Focus.Hyper] += 4;
					focusPoints[Focus.Aggressive] += 4; focusPoints[Focus.Violent] += 2;
					break;
				case Ai_Type.Priest:
					focusPoints[Focus.Focused] += 10; focusPoints[Focus.Hyper] += 1;
					focusPoints[Focus.Aggressive] += 1; focusPoints[Focus.Violent] += 1;
					break;
				case Ai_Type.Z_Archer:
				case Ai_Type.Z_Horseman:
				case Ai_Type.Z_Knight:
				case Ai_Type.Z_Priest:
					focusPoints[Focus.Focused] += 2; focusPoints[Focus.Hyper] += 2;
					focusPoints[Focus.Aggressive] += 5; focusPoints[Focus.Violent] += 5;
					break;
				case Ai_Type.SkeletalRemains:
					focusPoints[Focus.Focused] += 1; focusPoints[Focus.Hyper] += 1;
					focusPoints[Focus.Aggressive] += 1; focusPoints[Focus.Violent] += 1;
					break;
				default:
					focusPoints[Focus.Focused] += 0; focusPoints[Focus.Hyper] += 0;
					focusPoints[Focus.Aggressive] += 0; focusPoints[Focus.Violent] += 0;
					break;
			}

			//apply weapon type
			switch (Stats.WeaponType)
			{
				case WeaponType.Bone:
					focusPoints[Focus.Focused] += 1; focusPoints[Focus.Hyper] += 5;
					focusPoints[Focus.Aggressive] += 1; focusPoints[Focus.Violent] += 1;
					break;
				case WeaponType.Bow:
					focusPoints[Focus.Focused] += 3; focusPoints[Focus.Hyper] += 1;
					focusPoints[Focus.Aggressive] += 4; focusPoints[Focus.Violent] += 2;
					break;
				case WeaponType.Spear:
					focusPoints[Focus.Focused] += 2; focusPoints[Focus.Hyper] += 1;
					focusPoints[Focus.Aggressive] += 3; focusPoints[Focus.Violent] += 3;
					break;
				case WeaponType.Sword:
					focusPoints[Focus.Focused] += 1; focusPoints[Focus.Hyper] += 5;
					focusPoints[Focus.Aggressive] += 5; focusPoints[Focus.Violent] += 3;
					break;
				case WeaponType.Voice:
					focusPoints[Focus.Focused] += 10; focusPoints[Focus.Hyper] += 1;
					focusPoints[Focus.Aggressive] += 1; focusPoints[Focus.Violent] += 1;
					break;
			}

            // decrease focus based on percent of health lost
            if (Stats.Health >= 1)
            {
                float decrease = (int)Health.PercentDecrease(Stats.Health);
                focusPoints[Focus.Focused] -= (int)(focusPoints[Focus.Focused] * (decrease / 100));

            }

			return focusPoints;
		}

		void Ressurect()
		{

		}

		new public void Draw(SpriteBatch sb)
		{
			foreach (Vector2 v in WayPoints)
				sb.Draw(Texture, v, new Rectangle(0, 0, 64, 64), Color.Green);

		}

		public bool Wait
		{
			set { wait = value; }
		}

		
	}
}
