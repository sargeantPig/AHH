using System;
using System.Collections.Generic;
using System.Linq;
using AHH.Base;
using AHH.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AHH.Interactable.Building;
using AHH.Extensions;
using AHH.UI.Elements;

namespace AHH.AI
{
	class Grounded : AiUnit
	{
		protected int pathAttempts = 0;
		protected bool wait { get; set; }
		protected Point destination = new Point(10, 10);
		Point defenderID = new Point();
        Guid attackerID = new Guid();
		protected Ai_States nextState = Ai_States.Idle;
		protected Vector2 prevPos = new Vector2();
		float hitElasped = 0;
		protected bool keepTarget = false;
        float norm_speed;
        int waypointsReached = 0;
        bool marcher = true;
        protected bool stuck = false;
        bool noRetaliate = false;
		public Grounded(Vector2 position, Point rectExtends, Type_Data<Ai_Type> types, Stats stats, Grid grid)
			: base(position, rectExtends, stats.Speed, types.Animations, stats, types, grid)
		{
			RefreshInfo();
            norm_speed = stats.Speed;
            Ai_States = Ai_States.Marching;
            this.Speed = 1;
		}

		protected void RefreshInfo()
		{
			Info = new InfoPanel(
				new Dictionary<Text, Text>()
				{
                    { new Text(Vector2.One, "", Color.White), new Text(Vector2.One, Stats.Name, Color.White) },
                    { new Text(Vector2.One, "Health: ", Color.White), new Text(Vector2.One, Stats.Health.ToString() + " Damage: " + Stats.BaseDamage.ToString(), Color.White) },
                    { new Text(Vector2.One, "Armour: ", Color.White), new Text(Vector2.One, Stats.ArmourType.ToString(), Color.White) },
                    {new Text(Vector2.One, "Descr: ", Color.White), new Text(Vector2.One, Stats.Descr.ToString(), Color.White)  }
                }, data.Texture, Vector2.Zero);
		}

		public void Update(Cursor ms, GameTime gt, Architech arch, Grid grid, Random rng, Overseer os, UiMaster ui)
		{
			base.Update(ms, gt);
			base.Update(gt);
			base.Update(grid);
			CalculateCorners();
			GetFreeCorners(grid);

			RefreshInfo();

            if (Ai_States != Ai_States.Marching)
                this.Speed = norm_speed;

			if (IsHighlighted)
				ui.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(this.ID, Info));
			else ui.RemoveInfo(this.ID);

            if (base.data.Type == Ai_Type.Archer || base.data.Type == Ai_Type.Priest)
            {
                foreach (var proj in projectile.Values)
                {
                    if (arch.GetBuilding(defenderID) != null)
                        proj.Update(gt);
                }
            }

            if (Ai_States == Ai_States.Dead)
            {
                CurrentState = "Dead";
                return;
            }
            if (Ai_States != Ai_States.Retaliating && Ai_States != Ai_States.Marching && !marcher)
			    CheckTarget(arch);

			switch (Ai_States)
			{
				case Ai_States.Idle:
                    CurrentState = "Think";
					if (wait)
						Think_Pathing(grid, rng);
					else if (PFResult != null)
						Think_Pathing(grid, rng);
					return;
				case Ai_States.Target:
                    CurrentState = "Think";
                    EasyGetTarget(arch, grid, rng); //can go to idle or move
					break;
				case Ai_States.Moving: Moving(arch, grid, gt); // can go to idle or attacking
                    CurrentState = "Move";
					break;
				case Ai_States.Attacking:
                    CurrentState = "Attack";
                    Attacking(os, arch, gt, rng); // can go to idle or target
					break;
                case Ai_States.Retaliating:
                    Fighting(os, gt, rng);
                    break;
				case Ai_States.Dead:
                    CurrentState = "Dead";
					return;
                case Ai_States.Marching:
                    Forward(grid, arch);
                    break;

			}

			prevPos = Position;
		}

		protected void Idle(Architech arch, Grid grid, Random rng)
		{
			//decide whether to move or find a target
			//move or acquire a target
			Focus focus = Think(rng);

			switch (focus)
			{
				case Focus.Aggressive: Ai_States = Ai_States.Moving;
					break;
				case Focus.Focused: Ai_States = Ai_States.Target;
					break;
				case Focus.Hyper: Ai_States = Ai_States.Moving;
					break;
				case Focus.Violent: Ai_States = Ai_States.Target;
					break;
			}

		}

		protected void Moving(Architech arch, Grid grid, GameTime gt)
		{
			if (WayPoints.Count > 0)
				Moving(gt);
		}

		protected void Moving(GameTime gt)
		{
			bool reached = MoveTo(WayPoints[0], gt, true);
            if (reached)
            {
                WayPoints.RemoveAt(0);
                waypointsReached++;
                noRetaliate = false;
            }

            if (waypointsReached > 15 && marcher)
            {
                WayPoints.Clear();
                Ai_States = Ai_States.Target;
                this.Speed = norm_speed;
                marcher = false;
            }

            else if (waypointsReached <= 15 && WayPoints.Count > 0)
            {
                Ai_States = Ai_States.Moving;

            }

            else if (waypointsReached <= 15 && WayPoints.Count == 0)
                Ai_States = Ai_States.Marching;
		}

		protected void Attacking(Overseer os, Architech arch, GameTime gt, Random rng)
		{
			hitElasped += gt.ElapsedGameTime.Milliseconds;

			if (CheckTarget(arch))
			{
				if (hitElasped >= Stats.HitDelay)
				{
					os.Combat(this, arch.GetBuilding(defenderID), rng);
					hitElasped = 0;
					Projectile p = new Projectile(this.Center, new Point(16, 16), data.Projectile, 5, arch.GetBuilding(defenderID).Center);
					projectile.Add( p.ID, p );
				}
			}

		}

		void EasyGetTarget(Architech arch, Grid grid, Random rng)
		{
			if (!keepTarget)
			{
				var buildings = arch.GetBuildings;
				var focus = Think(rng);
				int index = 0;
				switch (focus)
				{
					case Focus.Focused:
						index = rng.Next((int)(buildings.Count * 0.8), buildings.Count);
						break;
					case Focus.Hyper:
						index = rng.Next(0, (int)(buildings.Count * 0.3));
						break;
					case Focus.Aggressive:
						index = rng.Next((int)(buildings.Count * 0.3), buildings.Count);
						break;
					case Focus.Violent:
						index = rng.Next(0, (int)(buildings.Count * 0.1));
						break;
				}

                if (buildings.Count == 0)
                    return;

				KeyValuePair<Point, Building> building = buildings.ToList()[index];
				var edges = building.Value.GetAdjacent(grid);

				if (edges == null || edges.Count == 0)
				{
					defenderID = Point.Zero;
					return; }

				destination = Grid.ToGridPosition(edges.GetRandom(rng), Grid.GetTileSize);
				defenderID = building.Key;
			}

			else
			{
				Building building = arch.GetBuilding(defenderID);
				var edges = building.AdjacentTiles;

				if (edges == null || edges.Count == 0)
				{
					defenderID = Point.Zero;
					return; }

				destination = Grid.ToGridPosition(edges.GetRandom(rng), Grid.GetTileSize);
			}

			Ai_States = Ai_States.Idle;
			Think_Pathing(grid, rng);
		}

		protected void AcquireTarget(Architech arch, Grid grid, bool newTarget, Random rng)
		{
			if (newTarget)
			{
				PFResult = null;
				if (Stats.Focus == Focus.Focused && rng.Next(0, 10) > 5)
				{
					
					defenderID = arch.Home;
					var home = arch.GetBuilding(defenderID);
					var edges = home.AdjacentTiles;
					if (edges == null)
					{
						Ai_States = Ai_States.Idle;
						return;
					}
					var dest = edges.GetRandom(rng);
					destination = Grid.ToGridPosition(dest, Grid.GetTileSize);
				}


				else {
					var building = arch.NearBuilding(Position, 1000);
					var edges = building.Value.AdjacentTiles;

					if (edges == null)
					{
						Ai_States = Ai_States.Idle;
						return;
					}
					var dest = edges.GetRandom(rng);
					destination = Grid.ToGridPosition(dest, Grid.GetTileSize);
					defenderID = building.Key;

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

		bool CheckTarget(Architech arch)
		{
			if (arch.GetBuilding(defenderID) != null)
			{
				if (arch.IsInRange(Corners, defenderID, (float)Stats.Range))
				{
                    noRetaliate = false;
					Ai_States = Ai_States.Attacking;
					return true;
				}
				else if (WayPoints.Count == 0)
				{ Ai_States = Ai_States.Target;  keepTarget = !keepTarget;}
			}

			else { Ai_States = Ai_States.Target; defenderID = new Point(); WayPoints.Clear(); keepTarget = false; }

			return false;
		}
		
		void Fighting(Overseer os, GameTime gt, Random rng)
		{
			if (os.Zombies.ContainsKey(attackerID))
			{
				if (os.Zombies[attackerID].Ai_States != Ai_States.Dead)
				{
					if (!os.ZIsInRange(Corners, attackerID, (float)Stats.Range))
					{
						Ai_States = Ai_States.Idle;
					}

					else
					{
						Ai_States = Ai_States.Retaliating;

						hitElasped += gt.ElapsedGameTime.Milliseconds;

						if (hitElasped >= Stats.HitDelay)
						{
							hitElasped = 0;
							os.Combat<Grounded>(this, (Grounded)os.Zombies[attackerID], rng);

                            if (base.data.Type == Ai_Type.Archer || base.data.Type == Ai_Type.Priest)
                            {
                                var p = new Projectile(Position, new Point(16, 16), data.Projectile, 5, os.Zombies[attackerID].Center);
                                projectile.Add(p.ID, p);
                            }
						}
					}

				}
				else { Ai_States = Ai_States.Target; noRetaliate = true; }

			}
			else { Ai_States = Ai_States.Target; noRetaliate = true; }


		}

		public void Retaliate(Guid atkid)
        {
            if (!noRetaliate)
            {
                this.attackerID = atkid;
                Ai_States = Ai_States.Retaliating;
            }
        }

		protected Focus Think(Random rng)
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

			return bag[choice];
		}

		protected bool Think_Pathing(Grid grid, Random rng)
		{
			WayPoints.Clear();

			if (PFResult != null)
			{
				wait = false;
				if (PFResult.GetType() == typeof(TileStates))
				{
					PFResult = null;
					Ai_States = Ai_States.Target;
					WayPoints.Clear();
					p_wait = true;
					return false;
				}

                if (PFResult.GetType() == typeof(bool))
                {
                    PFResult = null;
                    Ai_States = Ai_States.Target;
                    WayPoints.Clear();
                    p_wait = true;
                    stuck = true;
                    return false;
                }

                WayPoints = ((List<Vector2>)(PFResult));

				if (grid.CheckPositions(WayPoints).Count != WayPoints.Count) //theres a break in a waypoint
				{
					PFResult = null;
					Ai_States = Ai_States.Idle;
					WayPoints.Clear();
					p_wait = true;
					return false;
				}

				else
				{
					PFResult = null;
					p_wait = false;
					Ai_States = Ai_States.Moving;
					return true;
				}
			}
			else if(freeCorners.Count > 0)
			{
				if (!GetPath(grid, Center, destination, rng))
				{
					CheckActivity();
					pathAttempts++;
				}

				else {
					wait = false; pathAttempts = 0;
					return true; }

				wait = true;
			}

			return false; 
		}

		void Forward(Grid grid, Architech arch)
		{
			WayPoints.Clear();
			//check if can move forward
			Vector2 a_nextLocation = Position;

			Point gridLocation = Grid.ToGridPosition(Position, Grid.GetTileSize);
			Tile nextTile = grid.GetTile(new Point(gridLocation.X + 1, gridLocation.Y));

			if (nextTile != null)
			{
                if (nextTile.State != TileStates.Blocked && nextTile.State != TileStates.Immpassable)
                {
                    a_nextLocation = nextTile.Position;
                    WayPoints.Add(new Vector2(Position.X + 64, Position.Y));
                    Ai_States = Ai_States.Moving;

                }

                else {
                    Ai_States = Ai_States.Target;
                    waypointsReached += 15;
                    this.Speed = norm_speed;
                }
			}

			else
			{
				Ai_States = Ai_States.Target;
                waypointsReached += 15;
                this.Speed = norm_speed;
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
