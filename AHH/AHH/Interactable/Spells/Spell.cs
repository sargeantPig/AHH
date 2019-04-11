using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.AI;
using AHH.Interactable.Building;
namespace AHH.Interactable.Spells
{
	enum Spell_States
	{
		Alive,
		Dead
	}

	class Spell : MovingSprite
	{
		Spell_Stats stats;
		Type_Data<SpellType> data;
		Spell_States state { get; set; }
		float elaspedTick = 0;
		float elaspedAlive = 0;

		public Spell(Vector2 position, Spell_Stats stats, Type_Data<SpellType> data)
			: base(position, stats.Size, data.Texture, stats.Speed, data.Animations, true)
		{
			this.stats = stats;
			this.data = data;
			state = Spell_States.Alive;
			CurrentState = "Main";
		}

		public void Update(GameTime gt, Architech arch, Overseer os, Grid grid)
		{
			elaspedAlive += gt.ElapsedGameTime.Milliseconds;
			elaspedTick += gt.ElapsedGameTime.Milliseconds;

			if (elaspedTick >= stats.Tick && state == Spell_States.Alive)
			{
				arch.SpellEffect(this);
				os.SpellEffect(this, grid, arch );
				elaspedTick = 0;
			}

			if (elaspedAlive >= stats.Duration)
				state = Spell_States.Dead;

			base.Update(gt);
		}

		public Spell_States State
		{
			get { return state; }
			set { state = value; }
		}

		public Spell_Stats Stats
		{
			get { return stats; }
		}

		public Vector2 Center
		{
			get { return new Vector2(Position.X + (size.X / 2), Position.Y + (size.Y / 2)); }
		}
	}
}
