using System;
using System.Collections.Generic;
using AHH.Base;
using AHH.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AHH.Interactable;
using AHH.Extensions;
namespace AHH.AI
{
	class Grounded : AiUnit
	{
		int pathAttempts = 0;
		bool wait = false;
		public Grounded(Vector2 position, Point rectExtends, float speed, Unit_Types types, Stats stats, Grid grid)
			: base(position, rectExtends, speed, types.Animations, stats, types, grid)
		{

		}

		public void Update(Cursor ms, GameTime gt, Grid grid, Random rng)
		{
			base.Update(gt);

			if (wait)
			{
				Think_Pathing(grid);
				return;
			}

			switch (Ai_States)
			{
				case Ai_States.Thinking: CurrentState = "Think"; Think(grid, rng); break;
				case Ai_States.Resurrecting: Ressurect(); break;
				case Ai_States.Moving: CurrentState = "Move"; Moving(); break;
				case Ai_States.Attacking: CurrentState = "Attack"; Attacking(); break;
			}


		}

		void Attacking()
		{
			Ai_States = Ai_States.Thinking;

		}

		void Moving()
		{
			bool reached = MoveTo(WayPoints[0]);
			if (reached)
				WayPoints.RemoveAt(0);
			if (WayPoints.Count == 0)
				Ai_States = Ai_States.Thinking;
		}

		void Think(Grid grid, Random rng)
		{

			//Work out preffered action - taking into account hp and focus.
			var points = PrefferedAction();
			int mxpoint = 0;

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
					Think_Pathing(grid);
					break;
				case Focus.Aggressive:
					PFResult = null; Think_Forward(grid);
					break;
				case Focus.Hyper:
					PFResult = null; Think(grid, rng);
					break;
				case Focus.Violent:
					PFResult = null; Think_Violence(grid);
					break;

			}
		}

		void Think_Violence(Grid grid)
		{

			WayPoints.Clear();
			//check if buildings are nearby if path is blocke
			List<Building> near = grid.NearBuildings(Position);
			Vector2 closestBuildingPos = new Vector2(int.MaxValue, int.MaxValue);
			float min = 99999;

			if (near.Count == 0)
			{
				Ai_States = Ai_States.Thinking;
				return;
			}

			foreach (Building b in near)
			{
				float dis = Vector2.Distance(Position, b.Position);

				if (dis < min)
				{
					min = dis;
					closestBuildingPos = b.Position;
				}
			}

			if (closestBuildingPos.X != int.MaxValue)
			{
				WayPoints.Add(closestBuildingPos);
				Ai_States = Ai_States.Moving;
			}


		}

		void Think_Pathing(Grid grid)
		{
			WayPoints.Clear();

			if (PFResult != null)
			{
				wait = false;
				WayPoints = CalculateWayPoints(PFResult);
				Ai_States = Ai_States.Moving;
				Pathfinder = null;
			}
			else
			{
				if (!GetPath(grid, Position))
				{
					CheckActivity();
					pathAttempts++;
				}
				else wait = true;

				wait = true;

				if (pathAttempts > 20)
				{
					pathAttempts = 0;
					Ai_States = Ai_States.Thinking;
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
				Ai_States = Ai_States.Moving;
			}

			else
			{
				Ai_States = Ai_States.Attacking;
			}

		}

		Dictionary<Focus, int> PrefferedAction()
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
			switch (Stats.AiType)
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
			float decrease = (int)Health.PercentDecrease(Stats.Health);
			focusPoints[Focus.Focused] -= (int)(focusPoints[Focus.Focused] * (decrease / 100));

			return focusPoints;
		}

		void Ressurect()
		{

		}

		new public void Draw(SpriteBatch sb)
		{


		}

	}
}
