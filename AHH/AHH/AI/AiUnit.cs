using System.Collections.Generic;
using System;
using AHH.Base;
using AHH.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AHH.Interactable.Building;
using System.Threading;
using AHH.Extensions;
using AHH.UI.Elements;
namespace AHH.AI
{
	class AiUnit : InteractableMovingSprite, Ai
	{
		public static Texture2D[] statusBarTexture;
		AiUnit attacker;
		Building defender;
		Stats stats;
		float real_health { get; set; }
		Ai_States ai_State { get; set; }
		static OffloadThread[] pathfinder { get; set; }
		bool isZombie { get; set; }
		List<Vector2> waypoints { get; set; }
		object pf_result { get; set; }
		Dictionary<Corner, Vector2> corners { get; set; }
		static protected float waitMax = 100;
		protected float elasped = 0;
		protected bool p_wait = false;
		StatusBar statusBar;
		protected Dictionary<Guid, Projectile> projectile = new Dictionary<Guid, Projectile>();
		protected List<Guid> removeProj_queue = new List<Guid>();

		protected int maxPattempts = 10000;

		protected List<Vector2> freeCorners = new List<Vector2>();

		protected int pathfinderID = -1;

		protected Type_Data<Ai_Type> data;
		public AiUnit(Vector2 position, Point rectExtends, float speed, Dictionary<string, Vector3> states, Stats stats, Type_Data<Ai_Type> unit_types, Grid grid)
			: base(position, rectExtends, unit_types.Texture, unit_types.H_texture, unit_types.C_texture, speed, states)
		{
			this.stats = stats;
			real_health = this.stats.Health;
			waypoints = new List<Vector2>();
			ai_State = Ai_States.Target;
			corners = new Dictionary<Corner, Vector2>();
			
			pf_result = null;
			statusBar = new StatusBar(new Point(rectExtends.X, rectExtends.Y/ 5), (int)stats.Health, statusBarTexture);
			pathfinder = new OffloadThread[12];
			data = unit_types;
		}

		public void Update()
		{
			statusBar.Update(stats.Health);
			statusBar.UpdatePosition(Position);
            
			foreach (KeyValuePair<Guid, Projectile> g in projectile)
			{
				if(!g.Value.Alive)
					removeProj_queue.Add(g.Key);
			}
				
			foreach (Guid g in removeProj_queue)
				projectile.Remove(g);
		}

		public bool GetPath(Grid grid, Vector2 position, Point destination)
		{
			if (pathfinderID == -1)
			{
				for (int x = 0; x < pathfinder.Length; x++)
				{
					if (pathfinder[x] == null)
					{
						elasped = 0;

						pathfinder[x] = new OffloadThread();
						pathfinder[x].Th_Offload = new ThreadStart(() =>
						{
							this.pf_result = grid.Pathfinder(Grid.ToWorldPosition(destination, Grid.GetTileSize), Position);
						});

						pathfinder[x].Th_Child = new Thread(pathfinder[x].Th_Offload);
						Console.WriteLine("Pathfinder Starting " + x.ToString() + ": " + DateTime.Now.ToString("h:mm:ss"));
						pathfinder[x].Th_Child.Start();
						pathfinderID = x;
						return true;
					}
				}

			}

			else if (pathfinderID > -1)
			{
				if (pathfinder[pathfinderID] == null)
				{
					elasped = 0;

					pathfinder[pathfinderID] = new OffloadThread();
					pathfinder[pathfinderID].Th_Offload = new ThreadStart(() =>
					{
						this.pf_result = grid.Pathfinder(Grid.ToWorldPosition(destination, Grid.GetTileSize), Position);
					});
					Console.WriteLine("Pathfinder Starting " + pathfinderID.ToString() + ": " + DateTime.Now.ToString("h:mm:ss"));
					pathfinder[pathfinderID].Th_Child = new Thread(pathfinder[pathfinderID].Th_Offload);
					//Console.WriteLine("Pathfinder Starting " + DateTime.Now.ToString("h:mm:ss"));
					pathfinder[pathfinderID].Th_Child.Start();
					return true;
				}

				else {return false; }


			}

			pathfinderID = -1;
			return false;
		}

		public void CalculateCorners()
		{
			corners.Clear();
			corners.Add(Corner.TopLeft, new Vector2(Position.X, Position.Y));
			corners.Add(Corner.TopRight, new Vector2(Position.X + size.X, Position.Y));
			corners.Add(Corner.BottomLeft, new Vector2(Position.X, Position.Y + size.Y));
			corners.Add(Corner.BottomRight, new Vector2(Position.X + size.X, Position.Y + size.Y));

		}

		protected void GetFreeCorners(Grid grid)
		{
			freeCorners.Clear();
			foreach (Vector2 v in Corners.Values)
			{
				if (grid.GetTile(Grid.ToGridPosition(v, Grid.GetTileSize)).State == TileStates.Blocked)
				{
				}

				else
				{
					freeCorners.Add(v);
				}
			}

		}

		public void Draw_Status(SpriteBatch sb, Texture2D[] textures)
		{
			statusBar.Draw(sb, textures);
		}

		protected List<Vector2> CalculateWayPoints(object grid, Random rng)
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


				//check all around the tile and assign a counter
				Vector2 max = new Vector2(current.X + 1, current.Y + 1);
				Vector2 min = new Vector2(current.X - 1, current.Y - 1);
				int maxx = _grid.GetLength(0);
				int maxy = _grid.GetLength(1);
				if (current.X + 1 >= maxx)
					max.X = maxx - 1;
				if (current.Y + 1 >= maxy)
					max.Y = maxy - 1;
				if (current.X - 1 < 0)
					min.X = 0;
				if (current.Y - 1 < 0)
					min.Y = 0;


				
				int trueMaxX =	MathHelper.Clamp(current.X + 1, current.X, maxx);
				
				int trueMaxY = MathHelper.Clamp(current.Y + 1, current.Y, maxy);

				List<Vector2> lfound = new List<Vector2>();
				List<Point> pfound = new List<Point>();

				for (int x = (int)min.X; x <= max.X; x++)
				{
					for (int y = (int)min.Y; y <= max.Y; y++)
					{
						if (x != current.X || y != current.Y)
						{
							if (_grid[x, y].Item3 < _grid[current.X, current.Y].Item3)
							{
								next = new Vector2(_grid[x, y].Item1.X + (size.X/2), _grid[x, y].Item1.Y + (size.Y/2));
								pfound.Add(new Point(x, y));
								lfound.Add(next);
							}
						}
					}
				}

				var rnd = rng.Next(0, lfound.Count);

				if (isZombie)
					points.Add(lfound[0]);
				else
					points.Add(lfound[rnd]);
				current = pfound[rnd];
			}

			return points;
		}

		new public void Draw(SpriteBatch sb)
		{
			base.Draw(sb);

			foreach (var proj in projectile.Values)
			{
				proj.Draw(sb);
			}
		}

		protected void CheckActivity()
		{

			if (pathfinderID < 0)
				return;
			if (pathfinder[pathfinderID] != null)
			{
				if (!pathfinder[pathfinderID].Th_Child.IsAlive)
				{
					pathfinder[pathfinderID] = null;
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

		public void Ressurect(Overseer os)
		{
			stats = os.GetStats[stats.Type];
			
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

		public OffloadThread[] Pathfinder
		{
			get { return pathfinder; }
			set { pathfinder = value; }
		}

		public OffloadThread[] Pathfinder_
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

		public ref Stats GetStats()
		{
			return ref stats;
		}

		public Guid AID
		{
			get { return ID; }
		}

		public Dictionary<Corner, Vector2> Corners
		{
			get { return corners; }
		}
	}
}
