using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AHH.UI;
using AHH.Parsers;
using Microsoft.Xna.Framework.Graphics;
using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using AHH.Interactable.Building;
using AHH.Interactable.Spells;
using AHH.User;

namespace AHH.AI
{
	//sets up and manages AIs, including creation and deleting.
	class Overseer : BaseObject
	{
		Dictionary<Guid, Ai> ais { get; }
		Dictionary<Guid, Zombie> zombies { get; }
		List<Guid> remove_queue = new List<Guid>();
		Dictionary<Ai_Type, List<Stats>> ai_stats { get; }
		Dictionary<Ai_Type, Type_Data<Ai_Type>> unit_types { get; }
		float s_elasped = 100000;
		float s_target = 10000;

		Texture2D[] statusBarTextures;

		public Overseer(ContentManager cm, Texture2D[] statusBars)
		{
			ais = new Dictionary<Guid, Ai>();
			zombies = new Dictionary<Guid, Zombie>();
			ai_stats = Parsers.Parsers.Parse_Stats<Ai_Type, Stats>(@"Content\unit\unit_descr.txt");
			unit_types = Parsers.Parsers.Parse_Types<Ai_Type, Type_Data<Ai_Type>>(@"Content\unit\unit_types.txt", cm);
			statusBarTextures = statusBars;
		}

		public void Update(GameTime gt, Cursor ms, Architech arch, Grid grid, Player p, Random rng)
		{

			if (arch.BuildingPlaced)
			{
				foreach (Ai ai in ais.Values)
				{
					if (ai.Ai_States != Ai_States.Attacking && ai.Ai_States != Ai_States.Dead)
					{
						if (ai is Grounded)
						{
							((Grounded)ai).WayPoints.Clear();
							((Grounded)ai).Wait = false;
							((Grounded)ai).PFResult = null;
						}
						ai.Ai_States = Ai_States.Target;
					}
					

				}
			}

			foreach (Ai ai in ais.Values)
			{
				if (ai is Grounded)
					((Grounded)ai).Update(ms, gt, arch, grid, rng, this);
				else if (ai is AiUnit)
					ai.Update(ms, gt);

				if (ai is AiUnit)
				{
					if (ai.GetStats().Health <= 0 && ai.Ai_States != Ai_States.Dead)
					{
						ai.Ai_States = Ai_States.Dead;
					}
					else if (ai.Ai_States == Ai_States.Dead)
					{
						if (Options.GetTick)
							p.IncreaseEnergy += 1;
					}
					/*if (ai.IsZombie && ai.Health <= 0)
					remove_queue.Add(ai.AID);*/
				}
			}

			foreach (Zombie Z in zombies.Values)
			{
				Z.Update(ms, gt, arch, grid, rng, this);

				if (Z.GetStats().Health <= 0)
					remove_queue.Add(Z.ID);

			}

			foreach (Guid id in remove_queue)
			{
				if(ais.ContainsKey(id))
					ais.Remove(id);
				if (zombies.ContainsKey(id))
					zombies.Remove(id);
			}

			s_elasped += gt.ElapsedGameTime.Milliseconds;
			if (s_elasped > s_target)
			{
				Spawn(grid, rng);
				s_elasped = 0;
			}
		}

		public void Combat<T>(AiUnit attacker, T defender, Random rng)
		{
			float final_dmg = 0;

			switch (attacker.Stats.WeaponType)
			{
				case WeaponType.Spear:
					final_dmg += 5;
					break;
				case WeaponType.Sword:
					final_dmg += 8;
					break;
				case WeaponType.Bow:
					final_dmg += 3;
					break;
				case WeaponType.Voice:
					final_dmg += 1;
					break;
			}

			switch (attacker.Stats.Focus)
			{
				case Focus.Aggressive:
					final_dmg += 5;
					break;
				case Focus.Violent:
					final_dmg += 10;
					break;
				case Focus.Focused:
					final_dmg += 7;
					break;
				case Focus.Hyper:
					final_dmg += 3;
					break;
			}

			switch (attacker.Stats.Luck)
			{
				case Luck.Cowardly:
					if (rng.Next(0, 100) <= 2) final_dmg *= 2;
					break;
				case Luck.Holy:
					if (rng.Next(0, 100) <= 50) final_dmg *= 2;
					break;
				case Luck.Trained:
					if (rng.Next(0, 100) <= 25) final_dmg *= 2;
					break;
				case Luck.Zombie:
					if (rng.Next(0, 100) <= 10) final_dmg *= 2;
					break;
			}

			if (typeof(T) == typeof(Building))
			{
				Building d = defender as Building;
				//calculate dmg


				switch (d.GetBuildingData().ArmourType)
				{
					case ArmourType.Reinforced:
						final_dmg /= 2;
						break;
				}

				if (d.State == BuildingStates.Building)
					final_dmg *= 2;

				final_dmg += attacker.Stats.BaseDamage;
				
				d.GetBuildingData().Health -= final_dmg;
			}

			if (typeof(T) == typeof(Grounded))
			{
				Grounded d = defender as Grounded;
				switch (d.Stats.ArmourType)
				{
					case ArmourType.Heavy:
						final_dmg /= 1.8f;
						break;
					case ArmourType.Medium:
						final_dmg /= 1.5f;
						break;
					case ArmourType.Light:
						final_dmg /= 1.2f;
						break;
					case ArmourType.None:
						final_dmg *= 1.1f;
						break;
				
				}

				switch (d.Stats.Luck)
				{
					case Luck.Cowardly:
						if (rng.Next(0, 100) <= 2) final_dmg /= 2;
						break;
					case Luck.Holy:
						if (rng.Next(0, 100) <= 50) final_dmg /= 2;
						break;
					case Luck.Trained:
						if (rng.Next(0, 100) <= 25) final_dmg /= 2;
						break;
					case Luck.Zombie:
						if (rng.Next(0, 100) <= 10) final_dmg /= 2;
						break;

				}

				final_dmg += attacker.Stats.BaseDamage;
				d.GetStats().Health -= final_dmg;

				d.Retaliate(attacker.ID);
			}



		}

		public Dictionary<Point, object> GetUnitPoints()
		{
			Dictionary<Point, object> points = new Dictionary<Point, object>();
			foreach (Ai ai in ais.Values)
			{
				foreach (Vector2 corn in ai.Corners.Values)
				{
					if(!points.ContainsKey(Grid.ToGridPosition(corn, Grid.GetTileSize)))
						points.Add(Grid.ToGridPosition(corn, Grid.GetTileSize), new object());

				}

			}

			return points;

		}

		public List<Rectangle> GetUnitRects()
		{
			List<Rectangle> rects = new List<Rectangle>();

			foreach (Ai ai in ais.Values)
			{
				rects.Add(ai.Box);
			}

			return rects;

		}

		public bool IsInRange(Dictionary<Corner, Vector2> corners, Guid id, float range)
		{

			if (Vector2.Distance(corners[Corner.TopLeft], ais[id].Corners[Corner.TopRight]) <= range * 32)
				return true;
			if (Vector2.Distance(corners[Corner.TopRight], ais[id].Corners[Corner.BottomLeft]) <= range * 32)
				return true;
			if (Vector2.Distance(corners[Corner.BottomLeft], ais[id].Corners[Corner.BottomRight]) <= range * 32)
				return true;
			if (Vector2.Distance(corners[Corner.BottomRight], ais[id].Corners[Corner.TopLeft]) <= range * 32)
				return true;
			return false;
		}

		public bool ZIsInRange(Dictionary<Corner, Vector2> corners, Guid id, float range)
		{

			if (Vector2.Distance(corners[Corner.TopLeft], zombies[id].Corners[Corner.TopLeft]) <= range * 32)
				return true;
			if (Vector2.Distance(corners[Corner.TopRight], zombies[id].Corners[Corner.TopRight]) <= range * 32)
				return true;
			if (Vector2.Distance(corners[Corner.BottomLeft], zombies[id].Corners[Corner.BottomLeft]) <= range * 32)
				return true;
			if (Vector2.Distance(corners[Corner.BottomRight], zombies[id].Corners[Corner.BottomRight]) <= range * 32)
				return true;
			return false;
		}

		public List<Guid> GetUnitsInRange(Vector2 from, float range, bool ignoreDead = false)
		{
			List<Guid> units = new List<Guid>();
			foreach (KeyValuePair<Guid, Ai> ai in ais)
			{
				float dis = Vector2.Distance(from, ai.Value.Position);
				if (dis <= range && (ai.Value.Ai_States != Ai_States.Dead || !ignoreDead))
					units.Add(ai.Key);

			}

			return units;
		}

		public void SpellEffect(Spell attacker, Grid grid, Architech arch)
		{
			float range = attacker.Stats.Range * 64;
			//get all units in spell radius
			//apply spell effect
			var inrange = GetUnitsInRange(attacker.Center, range);
			switch (attacker.Stats.Type)
			{
				case SpellType.Ressurect:

					foreach (Guid id in inrange)
					{
						if (ais[id].GetStats().Health <= 0)
						{
							ais[id].IsZombie = true;
							ZombieSwap(id, grid, arch);
						}
					}
					break;
			}
			foreach (Guid id in inrange)
			{
				if(ais.ContainsKey(id))
					ais[id].GetStats().Health -= attacker.Stats.Damage;
			}
		}

		public void Draw(SpriteBatch sb)
		{
			int think = 0;
			int attack = 0;
			int moving = 0;

			foreach (Ai ai in ais.Values)
			{
				//if (ai is Grounded)
					//((Grounded)ai).Draw(sb);
			}

			foreach (Ai ai in ais.Values)
			{
				ai.Draw(sb);
				//ai.Draw_Debug(sb);
				switch (ai.Ai_States)
				{
					case Ai_States.Moving:
						moving++;
						break;
					case Ai_States.Attacking:
						attack++;
						break;
					case Ai_States.Idle:
						think++;
						break;

				}

				((AiUnit)ai).Draw_Status(sb, null);

			}

			foreach (Zombie z in zombies.Values)
			{
				((Ai)z).Draw(sb);
				z.Draw_Status(sb, null);
			}

			sb.DrawString(DebugFont, "Total: " + ais.Count().ToString(), Position, Color.Black);
			sb.DrawString(DebugFont, "Moving: " + moving.ToString(), Position + new Vector2(300, 0), Color.Black);
			sb.DrawString(DebugFont, "Attacking: " + attack.ToString(), Position + new Vector2(600, 0), Color.Black);
			sb.DrawString(DebugFont, "Thinking: " + think.ToString(), Position + new Vector2(900, 0), Color.Black);
		}

		void Spawn(Grid grid, Random rng)
		{
			Ai_Type t = Extensions.Extensions.RandomFlag<Ai_Type>(rng, 0, 4);
			Stats stats = new Stats(ai_stats[t][0]);
			Type_Data<Ai_Type> ut = new Type_Data<Ai_Type>(unit_types[t]);
			var newg = new Grounded(new Vector2(128, 64), new Point(32, 32), ut, stats, grid);


			ais.Add(newg.AID, newg);
		}

		void ZombieSwap(Guid ai, Grid grid, Architech arch)  //swaps a unit to an equalivalent zombie unit
		{
			var old = ais[ai];
			var zombi = new Zombie(old.Position, new Point(32, 32), unit_types[ConvertToZombie(old.GetStats().Type)], ai_stats[ConvertToZombie(old.GetStats().Type)][0], grid, arch);
			zombies.Add(zombi.ID, zombi);
			ais.Remove(ai);
		}

		public Dictionary<Guid, Ai> Ais
		{
			get { return ais; }
		}

		public Dictionary<Guid, Zombie> Zombies
		{
			get { return zombies; }
		}

		public Dictionary<Ai_Type, List<Stats>> GetStats
		{
			get { return ai_stats; }
		}

		public Ai_Type ConvertToZombie(Ai_Type ai)
		{
			string temp = ai.ToString();

			return (Ai_Type)Enum.Parse(typeof(Ai_Type), "Z_" + temp);
		}
	}
}
