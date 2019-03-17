using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Base;
using AHH.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AHH.Interactable;
using System.Threading;
using AHH.Extensions;
namespace AHH.AI
{

	enum Ai_States
	{
		Moving,
		Thinking,
		Attacking,
		Resurrecting,
		Dead
	}

	enum Ai_Type
	{
		Knight,
		Horseman,
		Archer,
		Priest,
		SkeletalRemains,
		Z_Knight,
		Z_Horseman,
		Z_Priest,
		Z_Archer
	}

	enum ArmourType
	{
		Light,
		Medium,
		Heavy,
		None
	}

	enum WeaponType
	{
		Spear,
		Sword,
		Bow,
		Bone,
		Voice
	}

	enum Luck
	{
		Trained,
		Holy,
		Cowardly,
		Zombie
	}

	interface Ai
	{
		int Health { get; set; }
		void SetAttacker(ref AiUnit attacker);
		AiUnit GetAttacker();
		void SetDefender(ref Building defender);
		Building GetDefender();
		void Update(Cursor ms);
		void Draw(SpriteBatch sb);
		Vector2 Position { get; set; }
		bool IsZombie { get; set; }
		Ai_States Ai_States { get; set; }
	}

	struct Stats
	{
		string name { get; set; }
		Ai_Type ai_type { get; set; }
		int health { get; set; }
		WeaponType weaponType { get; set; }
		ArmourType armourType { get; set; }
		Luck luck { get; set; }
		int baseDamage { get; set; }
		int range { get; set; }
		float hitDelay { get; set; }

		public Stats(Stats stats)
		{
			this.name = stats.name;
			this.ai_type = stats.ai_type;
			this.armourType = stats.armourType;
			this.baseDamage = stats.baseDamage;
			this.health = stats.health;
			this.hitDelay = stats.hitDelay;
			this.luck = stats.luck;
			this.range = stats.range;
			this.weaponType = stats.weaponType;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Ai_Type AiType
		{
			get { return ai_type; }
			set { ai_type = value; }
		}

		public int Health
		{
			get { return health; }
			set { health = value; }
		}

		public WeaponType WeaponType
		{
			get { return weaponType; }
			set { weaponType = value; }
		}

		public ArmourType ArmourType
		{
			get { return armourType; }
			set { armourType = value; }
		}

		public Luck Luck
		{
			get { return Luck; }
			set { luck = value; }
		}

		public int BaseDamage
		{
			get { return baseDamage; }
			set { baseDamage = value; }
		}

		public int Range
		{
			get { return range; }
			set { range = value; }
		}

		public float HitDelay
		{
			get { return hitDelay; }
			set { hitDelay = value; }
		}

	}

	struct Unit_Types
	{
		Ai_Type type { get; set; }
		Texture2D texture { get; set; }
		Texture2D h_texture { get; set; }
		Texture2D c_texture { get; set; }
		Texture2D projectile { get; set; }
		Dictionary<string, Vector3> animations { get; set; }

		public Unit_Types(Unit_Types ut)
		{
			this.type = ut.Type;
			this.texture = ut.texture;
			this.h_texture = ut.h_texture;
			this.c_texture = ut.c_texture;
			this.projectile = ut.projectile;
			this.animations = ut.animations;
		}

		public Ai_Type Type
		{
			get { return type; }
			set { type = value; }
		}

		public Texture2D Texture
		{
			get { return texture; }
			set { texture = value; }
		}

		public Texture2D H_texture
		{
			get { return h_texture; }
			set { h_texture = value; }
		}
		public Texture2D C_texture
		{
			get { return c_texture; }
			set { c_texture = value; }
		}
		public Texture2D Projectile
		{
			get { return projectile; }
			set { projectile = value; }
		}

		public Dictionary<string, Vector3> Animations
		{
			get { return animations; }
			set { animations = value; }
		}
	}

	class AiUnit : InteractableMovingSprite, Ai
	{
		AiUnit attacker;
		Building defender;
		Stats stats = new Stats();
		Ai_States ai_State { get; set; }
		ThreadStart th_pathfinder { get; set; }
		Thread th_pathchild { get; set; }
		bool isZombie { get; set; }
		List<Vector2> waypoints { get; set; }

		public AiUnit(Vector2 position, Point rectExtends, float speed, Dictionary<string, Vector3> states, Stats stats,  Texture2D texture, Texture2D texture_h, Texture2D texture_c)
			:base(position, rectExtends, texture, texture_h, texture_c, speed, states)
		{
			this.stats = stats;
			waypoints = new List<Vector2>();
			ai_State = Ai_States.Thinking;
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

				for (int x = current.X - 1 < 0 ? 0 : current.X - 1; current.X + 1 > maxx ? x < maxx : x < current.X + 2; x++)
				{
					if (found)
						break;

					for (int y = current.Y - 1 < 0 ? 0 : current.Y - 1; current.Y + 1 > maxy  ? y < maxy : y < current.Y + 2  ; y++)
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

		public AiUnit GetAttacker()
		{
			return attacker;
		}

		public void SetAttacker(ref AiUnit attacker)
		{
			this.attacker = attacker;
		}

		public int Health
		{
			get { return stats.Health; }
			set { stats.Health = value; }
		}

		public bool IsZombie
		{
			get { return isZombie; }
			set { isZombie= value; }
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

		public ThreadStart Th_Pathfinder
		{
			get { return th_pathfinder; }
			set { th_pathfinder = value; }
		}

		public Thread Th_PathChild
		{
			get { return th_pathchild; }
			set { th_pathchild = value; }
		}

		public List<Vector2> WayPoints
		{
			get { return waypoints; }
			set { waypoints = value; }
		}
	}

	class Mounted : AiUnit
	{

		public Mounted(Vector2 position, Point rectExtends, float speed, Dictionary<string, Vector3> states, Stats stats, Texture2D texture, Texture2D texture_h, Texture2D texture_c)
			: base(position, rectExtends, speed, states, stats, texture, texture_h, texture_c)
		{

		}

		new public void Update(Cursor ms)
		{

		}

		new public void Draw(SpriteBatch sb)
		{


		}

	}

	class Grounded : AiUnit
	{
		object pf_result = new object();
		bool th_wait = false;

		public Grounded(Vector2 position, Point rectExtends, float speed, Unit_Types types, Stats stats, Grid grid)
			: base(position, rectExtends, speed, types.Animations, stats, types.Texture, types.H_texture, types.C_texture)
		{
			Th_Pathfinder = new ThreadStart(() => {
				pf_result = grid.Pathfinder(Grid.ToWorldPosition(new Point(15, 6), Grid.GetTileSize), Position);
			});

			Th_PathChild = new Thread(Th_Pathfinder);
		}

		public void Update(Cursor ms, GameTime gt, Grid grid)
		{
			switch (Ai_States)
			{
				case Ai_States.Thinking: CurrentState = "Think"; Think(grid); break;
				case Ai_States.Resurrecting: Ressurect(); break;
				case Ai_States.Moving: CurrentState = "Move"; Moving(); break;
				case Ai_States.Attacking: CurrentState = "Attack"; Attacking(); break;
			}

			base.Update(gt);
		}

		void Attacking()
		{


		}

		void Moving()
		{
			bool reached = MoveTo(WayPoints[0]);
			if (reached)
				WayPoints.RemoveAt(0);
			if (WayPoints.Count == 0)
				Ai_States = Ai_States.Thinking;
		}

		void Think(Grid grid)
		{
			WayPoints.Clear();
			//check if can move forward
			Vector2 a_nextLocation = Position;
			Vector2 closestBuildingPos = new Vector2();
			if (!Th_PathChild.IsAlive && !th_wait)
			{
				Point gridLocation = Grid.ToGridPosition(Position, Grid.GetTileSize);
				Tile nextTile = grid.GetTile(new Point(gridLocation.X + 1, gridLocation.Y));
				
				if (nextTile != null)
				{
					if (nextTile.State != TileStates.Blocked && nextTile.State != TileStates.Immpassable)
						a_nextLocation = nextTile.Position;
				}

				//check if buildings are nearby if path is blocked
				if (a_nextLocation == Position)
				{
					List<Building> near = grid.NearBuildings(Position);
					
					float min = 99999;

					foreach (Building b in near)
					{
						float dis = Vector2.Distance(Position, b.Position);

						if (dis < min)
						{
							min = dis;
							closestBuildingPos = b.Position;
						}
					}
				}

			}

			//attempt to find a path to main objective
			if (!Th_PathChild.IsAlive && !th_wait)
			{
				th_wait = true;

				Th_PathChild = new Thread(Th_Pathfinder);
				Th_PathChild.Start();
			}
			
			if (!Th_PathChild.IsAlive && th_wait)
			{
				th_wait = false;
				if (pf_result != null)
				{
					WayPoints = CalculateWayPoints(pf_result);
					Ai_States = Ai_States.Moving;
				}
				//pathing failed resort to continuing forward
				else
				{
					if (a_nextLocation != Position)
					{
						WayPoints.Add(a_nextLocation);
						Ai_States = Ai_States.Moving;
					}
				}
			}


		}

		void Ressurect()
		{

		}

		new public void Draw(SpriteBatch sb)
		{


		}

	}
}
