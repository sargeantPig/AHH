using System.Collections.Generic;
using AHH.Base;
using AHH.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AHH.Interactable;
using System.Threading;
using AHH.Extensions;

namespace AHH.AI
{
	class AiUnit : InteractableMovingSprite, Ai
	{
		AiUnit attacker;
		Building defender;
		Stats stats;
		float real_health { get; set; }
		Ai_States ai_State { get; set; }
		static Pathfinder pathfinder { get; set; }
		bool isZombie { get; set; }
		List<Vector2> waypoints { get; set; }
		object pf_result { get; set; }
		public AiUnit(Vector2 position, Point rectExtends, float speed, Dictionary<string, Vector3> states, Stats stats, Unit_Types unit_types, Grid grid)
			: base(position, rectExtends, unit_types.Texture, unit_types.H_texture, unit_types.C_texture, speed, states)
		{
			this.stats = stats;
			real_health = this.stats.Health;
			waypoints = new List<Vector2>();
			ai_State = Ai_States.Thinking;


		}

		public bool GetPath(Grid grid, Vector2 position)
		{
			if (pathfinder == null)
			{
				pathfinder = new Pathfinder(grid, position);
				Pathfinder.Th_Pathfinder = new ThreadStart(() => {
					this.pf_result = grid.Pathfinder(Grid.ToWorldPosition(new Point(15, 6), Grid.GetTileSize), Position);
				});

				Pathfinder.Th_PathChild = new Thread(Pathfinder.Th_Pathfinder);
				pathfinder.Th_PathChild.Start();
				return true;
			}

			else return false;
		}

		protected List<Vector2> CalculateWayPoints(object grid)
		{
			WTuple<Vector2, TileStates, int>[,] _grid = ((WTuple<Vector2, TileStates, int>[,])grid);
			List<Vector2> points = new List<Vector2>();
			//find start point
			Point start = Grid.ToGridPosition(Position, Grid.GetTileSize);
			Point current = start;
			//find finish
			Point finish = new Point();
			for (int x = 0; x < _grid.GetLength(0); x++)
			{
				for (int y = 0; y < _grid.GetLength(1); y++)
				{
					if (_grid[x, y].Item3 == 0)
					{
						finish = new Point(x, y);
						break;
					}
				}
			}

			while (current != finish)
			{
				Vector2 next = new Vector2();
				bool found = false;
				int maxx = _grid.GetLength(0);
				int maxy = _grid.GetLength(1);

				for (int x = MathHelper.Clamp(current.X - 1, 0, maxx); x < maxx; x++)
				{
					if (found)
						break;

					for (int y = MathHelper.Clamp(current.Y - 1, 0, maxy); y < maxy; y++)
					{
						if (x != current.X || y != current.Y)
						{
							if (_grid[x, y].Item3 < _grid[current.X, current.Y].Item3)
							{
								next = new Vector2(_grid[x, y].Item1.X, _grid[x, y].Item1.Y);
								current = new Point(x, y);
								points.Add(next);
								found = true;
								break;
							}
						}
						if (found)
							break;

					}
				}


			}

			return points;
		}

		protected void CheckActivity()
		{
			if (pathfinder != null)
			{
				if (!pathfinder.Th_PathChild.IsAlive)
				{
					Pathfinder_ = null;
				}
			}
		}

		public AiUnit GetAttacker()
		{
			return attacker;
		}

		public void SetAttacker(ref AiUnit attacker)
		{
			this.attacker = attacker;
		}

		public float Health
		{
			get { return real_health; }
			set { real_health = value; }
		}

		public bool IsZombie
		{
			get { return isZombie; }
			set { isZombie = value; }
		}

		public Building GetDefender()
		{
			return defender;
		}

		public void SetDefender(ref Building defender)
		{
			this.defender = defender;
		}

		public Ai_States Ai_States
		{
			get { return ai_State; }
			set { ai_State = value; }
		}

		public Pathfinder Pathfinder
		{
			get { return pathfinder; }
			set { pathfinder = value; }
		}

		static public Pathfinder Pathfinder_
		{
			get { return pathfinder; }
			set { pathfinder = value; }
		}

		public List<Vector2> WayPoints
		{
			get { return waypoints; }
			set { waypoints = value; }
		}

		public Stats Stats
		{
			get { return stats; }
			set { stats = value; }
		}

		public float RHealth
		{
			get { return real_health; }
			set { real_health = value; }
		}

		public object PFResult
		{
			get { return pf_result; }
			set { pf_result = value; }
		}
	}
}
