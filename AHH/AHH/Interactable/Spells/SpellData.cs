using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.Interactable.Spells
{
	enum SpellType
	{
		Ressurect,
		DrainEssence,
		RestoreEssence
	}

	struct Spell_Stats
	{
		SpellType type { get; set; }
		string name { get; set; }
		float range { get; set; }
		float duration { get; set; }
		float cost { get; set; }
		Point size { get; set; }
		float speed { get; set; }
		float tick { get; set; }
		float damage { get; set; }

		public Spell_Stats(Spell_Stats ss)
		{
			this.type = ss.type;
			this.name = ss.name;
			this.range = ss.range;
			this.duration = ss.duration;
			this.cost = ss.cost;
			this.size = ss.size;
			this.tick = ss.tick;
			this.speed = ss.speed;
			this.damage = ss.damage;
		}

		public SpellType Type
		{
			get { return type; }
			set { type = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public float Range
		{
			get { return range; }
			set { range = value; }
		}

		public float Cost
		{
			get { return cost; }
			set { cost = value; }
		}

		public float Duration
		{
			get { return duration; }
			set { duration = value; }
		}

		public Point Size
		{
			get { return size; }
			set { size = value; }
		}

		public float Speed
		{
			get { return speed; }
			set { speed = value; }
		}

		public float Tick
		{
			get { return tick; }
			set { tick = value; }
		}

		public float Damage
		{
			get { return damage; }
			set { damage = value; }
		}

	}
}
