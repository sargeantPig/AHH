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
namespace AHH.AI
{
	//sets up and manages AIs, including creation and deleting.
	class Overseer : BaseObject
	{
		HashSet<Ai> ais { get; }
		 
		Dictionary<Ai_Type, List<Stats>> ai_stats { get; }
		Dictionary<Ai_Type, Type_Data<Ai_Type>> unit_types { get; }
		float s_elasped = 100000;
		float s_target = 10000;
		public Overseer(ContentManager cm)
		{
			ais = new HashSet<Ai>();
			ai_stats = Parsers.Parsers.Parse_Stats<Ai_Type, Stats>(@"Content\unit\unit_descr.txt");
			unit_types = Parsers.Parsers.Parse_Types<Ai_Type, Type_Data<Ai_Type>>(@"Content\unit\unit_types.txt", cm);
		}

		public void Update(GameTime gt, Cursor ms, Architech arch,  Grid grid,  Random rng)
		{
     
			if (arch.BuildingPlaced)
			{
				if (AiUnit.Pathfinder_ != null)
				{
					if (!AiUnit.Pathfinder_.Th_Child.IsAlive)
						AiUnit.Pathfinder_ = null;
					else
					{
						AiUnit.Pathfinder_.Th_Child.Abort();
						AiUnit.Pathfinder_ = null;
					}
				}
				foreach (Ai ai in ais)
				{
					if (ai.Ai_States != Ai_States.Attacking)
					{
						if (ai is Grounded)
						{
							((Grounded)ai).WayPoints.Clear();
							((Grounded)ai).Wait = false;
							((Grounded)ai).PFResult = null;
						}
						ai.Ai_States = Ai_States.Thinking;
					}
				}
			}


			foreach (Ai ai in ais)
			{
				if (ai is Grounded)
					((Grounded)ai).Update(ms, gt, arch, grid, rng, this);
				else if (ai is AiUnit)
					ai.Update(ms);
			}

			s_elasped += gt.ElapsedGameTime.Milliseconds;
			if(s_elasped > s_target)
			{
				Spawn(grid, rng);
				s_elasped = 0;
			}
		}

		public void Combat<T>(AiUnit attacker, T defender, Random rng)
		{
			float final_dmg = 0;

			if (typeof(T) == typeof(Building))
			{
				Building d = defender as Building;
				//calculate dmg

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

				switch (d.GetBuildingData().ArmourType)
				{
					case ArmourType.Reinforced:
						final_dmg /= 2;
						break;
				}

				final_dmg += attacker.Stats.BaseDamage;

				d.GetBuildingData().Health -= final_dmg;
			}

		}

		public void Draw(SpriteBatch sb)
		{
			int think = 0;
			int attack = 0;
			int moving = 0;

			foreach (Ai ai in ais)
			{
				/*if (ai is Grounded)
					((Grounded)ai).Draw(sb);*/
			}

			foreach (Ai ai in ais)
			{
				ai.Draw(sb);
				switch (ai.Ai_States)
				{
					case Ai_States.Moving:
						moving++;
						break;
					case Ai_States.Attacking:
						attack++;
						break;
					case Ai_States.Thinking:
						think++;
						break;

				}

				

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
			Type_Data<Ai_Type> ut = new Type_Data<Ai_Type>(unit_types[t]) ;

			ais.Add(new Grounded(new Vector2(128, 64), new Point(64, 64), 1, ut, stats, grid));
		}

		public HashSet<Ai> Ais
		{
			get { return ais; }
		}

	}
}
