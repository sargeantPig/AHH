using System.Collections.Generic;
using AHH.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AHH.Interactable;
using AHH.Interactable.Building;

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

	enum Focus
	{
		Focused,
		Aggressive,
		Hyper,
		Violent
	}

	interface Ai
	{
		float Health { get; set; }
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
		Focus focus { get; set; }
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
			this.focus = stats.focus;
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

		public Focus Focus
		{
			get { return focus; }
			set { focus = value; }
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
}
