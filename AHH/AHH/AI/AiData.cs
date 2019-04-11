﻿using System.Collections.Generic;
using AHH.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AHH.Interactable;
using AHH.Interactable.Building;
using System;

namespace AHH.AI
{
	enum Ai_States
	{
		Moving,
		Thinking,
		Attacking,
        Retaliating,
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
        Reinforced,
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
		void Update(Cursor ms, GameTime gt);
		void Draw(SpriteBatch sb);
		Vector2 Position { get; set; }
		bool IsZombie { get; set; }
		Ai_States Ai_States { get; set; }
		Guid AID { get; }
		ref Stats GetStats();
		Dictionary<Corner, Vector2> Corners { get; }
	}

	struct Stats
	{
		string name { get; set; }
		Ai_Type type { get; set; }
		float health { get; set; }
		WeaponType weaponType { get; set; }
		ArmourType armourType { get; set; }
		Luck luck { get; set; }
		Focus focus { get; set; }
		int baseDamage { get; set; }
		double range { get; set; }
		float hitDelay { get; set; }
		float speed { get; set; }

		public Stats(Stats stats)
		{
			this.name = stats.name;
			this.type = stats.type;
			this.armourType = stats.armourType;
			this.baseDamage = stats.baseDamage;
			this.health = stats.health;
			this.hitDelay = stats.hitDelay;
			this.luck = stats.luck;
			this.range = stats.range;
			this.weaponType = stats.weaponType;
			this.focus = stats.focus;
			this.speed = stats.speed;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Ai_Type Type
		{
			get { return type; }
			set { type = value; }
		}

		public float Health
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
			get { return luck; }
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

		public double Range
		{
			get { return range; }
			set { range = value; }
		}

		public float HitDelay
		{
			get { return hitDelay; }
			set { hitDelay = value; }
		}
		public float Speed
		{
			get { return speed; }
			set { speed = value; }
		}

	}
}
