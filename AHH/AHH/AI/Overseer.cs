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
using AHH.Research;
using AHH.Functions;
namespace AHH.AI
{
	//sets up and manages AIs, including creation and deleting.
	class Overseer : BaseObject
	{
		Dictionary<Guid, Ai> ais { get; }
		Dictionary<Guid, Zombie> zombies { get; }
		List<Guid> remove_queue = new List<Guid>();
        public Dictionary<Ai_Type, Stats> ai_stats;
		Dictionary<Ai_Type, Type_Data<Ai_Type>> unit_types { get; }
		float s_elasped = 200000;
		float s_target = 0;

		Texture2D[] statusBarTextures;

        List<Ai_Type[,]> formations = new List<Ai_Type[,]>();
        List<int> ys = new List<int>();
        bool spawning = false;
        float spawn_target = 1000;
        float spawn_elasped = 0;
        int spawnY = 0;
        int spawnX = 0;
        int posy = 0;

        int rtile = 16;
		public Overseer(ContentManager cm, Texture2D[] statusBars)
		{
			ais = new Dictionary<Guid, Ai>();
			zombies = new Dictionary<Guid, Zombie>();
			ai_stats = Parsers.Parsers.Parse_Stats<Ai_Type, Stats>(@"Content\unit\unit_descr.txt");
			unit_types = Parsers.Parsers.Parse_Types<Ai_Type, Type_Data<Ai_Type>>(@"Content\unit\unit_types.txt", cm);
			statusBarTextures = statusBars;
            CalcTarget();
		}

        void CalcTarget()
        {
            s_target = (100000 / ((Options.Difficulty + 1000 / 1000))) + 10000;


            s_target = MathHelper.Clamp(s_target, 5000, 20000);
        }

		public void Update(GameTime gt, Cursor ms, Architech arch, UiMaster ui, Grid grid, Player p, Random rng)
		{
            p.Population = zombies.Count;

            foreach (Ai ai in ais.Values)
			{
				if (ai is Grounded)
					((Grounded)ai).Update(ms, gt, arch, grid, rng, this, ui);
				else if (ai is AiUnit)
					ai.Update(ms, gt);

				if (ai is AiUnit)
				{
					if (ai.GetStats().Health <= 0 && ai.Ai_States != Ai_States.Dead)
					{
						ai.Ai_States = Ai_States.Dead;
                        Statistics.Kills++;
					}

                    if (arch.BuildingPlaced)
                    {
                        if (ai.Ai_States != Ai_States.Attacking && ai.Ai_States != Ai_States.Dead)
                        {
                            ((Grounded)ai).CheckWaypoints = true;
                        }
                    }
                }
			}

			foreach (Zombie Z in zombies.Values)
			{
				Z.Update(ms, gt, arch, grid, rng, this, ui);

                if (arch.BuildingPlaced)
                {
                    if (Z.Ai_States != Ai_States.Attacking && Z.Ai_States != Ai_States.Dead)
                    {
                        Z.WayPoints.Clear();
                        Z.Wait = false;
                        Z.PFResult = null;
                        Z.Ai_States = Ai_States.Target;
                    }
                }

                if (arch.BuilingDestroyed)
                {
                    if (Z.Ai_States == Ai_States.Thinking)
                    {
                        Z.Ai_States = Ai_States.Target;
                    }

                }
                if (Z.Ai_States == Ai_States.Thinking && rng.Next(0, 10) == 5)
                {
                    Z.Ai_States = Ai_States.Target;
                }

                if (Z.GetStats().Health <= 0)
					remove_queue.Add(Z.ID);
			}

            if (ais.Count > 200)
                CleanUp();

            foreach (Guid id in remove_queue)
			{
                if (ais.ContainsKey(id))
                {
                    ais.Remove(id);
                    p.IncreaseEnergy += 100;
                }
				if (zombies.ContainsKey(id))
					zombies.Remove(id);
			}

			s_elasped += gt.ElapsedGameTime.Milliseconds;
			if (s_elasped > s_target && !spawning)
			{
                CreateNewFormation(rng, grid);
                spawning = true;
                CalcTarget();
				s_elasped = 0;
			}

            if (spawning)
            {
                spawn_elasped += gt.ElapsedGameTime.Milliseconds;
                if (spawn_elasped >= spawn_target)
                {
                    Spawn(grid, rng);
                    spawn_elasped = 0;
                }
            }

            if(Options.GetTick)
                p.IncreaseEnergy -= zombies.Count * 5;

            
		}

        public void CleanUp()
        {
            foreach (var u in ais)
            {
                if (u.Value.Ai_States == Ai_States.Dead)
                {
                    remove_queue.Add(u.Key);
                    return;
                }

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
					if (rng.Next(0, 100) <= 25) final_dmg *= 2;
					break;
				case Luck.Trained:
					if (rng.Next(0, 100) <= 10) final_dmg *= 2;
					break;
				case Luck.Zombie:
					if (rng.Next(0, 100) <= 5) final_dmg *= 2;
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
            foreach (Vector2 v in corners.Values)
            {
                if (CheckDistanceY(ais[id].Corners[Corner.TopLeft], ais[id].Corners[Corner.BottomLeft], v, range))
                    return true;
                if (CheckDistanceX(ais[id].Corners[Corner.TopLeft], ais[id].Corners[Corner.TopRight], v, range))
                    return true;
                if (CheckDistanceY(ais[id].Corners[Corner.TopRight], ais[id].Corners[Corner.BottomRight], v, range))
                    return true;
                if (CheckDistanceX(ais[id].Corners[Corner.BottomLeft], ais[id].Corners[Corner.BottomRight], v, range))
                    return true;
            }

            return false;
		}

		public bool ZIsInRange(Dictionary<Corner, Vector2> corners, Guid id, float range)
		{

            foreach (Vector2 v in corners.Values)
            {
                if (CheckDistanceY(zombies[id].Corners[Corner.TopLeft], zombies[id].Corners[Corner.BottomLeft], v, range))
                    return true;
                if (CheckDistanceX(zombies[id].Corners[Corner.TopLeft], zombies[id].Corners[Corner.TopRight], v, range))
                    return true;
                if (CheckDistanceY(zombies[id].Corners[Corner.TopRight], zombies[id].Corners[Corner.BottomRight], v, range))
                    return true;
                if (CheckDistanceX(zombies[id].Corners[Corner.BottomLeft], zombies[id].Corners[Corner.BottomRight], v, range))
                    return true;
            }
            return false;
		}

        public bool CheckDistanceY(Vector2 min, Vector2 max, Vector2 value, float range)
        {
            for (int y = (int)min.Y; y < max.Y; y++)
            {
                var temp = Vector2.Distance(value, new Vector2(min.X, y));
                if (temp <= (range) * 32)
                    return true;
            }

            return false;
        }
        public bool CheckDistanceX(Vector2 min, Vector2 max, Vector2 value, float range)
        {
            for (int y = (int)min.X; y < max.X; y++)
            {
                var temp = Vector2.Distance(value, new Vector2(y, min.Y));
                if (temp <= (range) * 32)
                    return true;
            }

            return false;
        }

        public List<Guid> GetUnitsInRange(Vector2 from, float range, bool ignoreDead = true)
		{
			List<Guid> units = new List<Guid>();
			foreach (KeyValuePair<Guid, Ai> ai in ais)
			{
				float dis = Vector2.Distance(from, ai.Value.Center);
				if (dis <= range && (ai.Value.Ai_States != Ai_States.Dead || !ignoreDead))
					units.Add(ai.Key);

			}

			return units;
		}

        public List<Guid> GetZombsInRange(Vector2 from, float range, bool ignoreDead = false)
        {
            List<Guid> units = new List<Guid>();
            foreach (KeyValuePair<Guid, Zombie> ai in zombies)
            {
                float dis = Vector2.Distance(from, ai.Value.Center);
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
			var inrange = GetUnitsInRange(attacker.Center, range, false);
            var zombs = GetZombsInRange(attacker.Center, range);
            
			switch (attacker.Stats.Type)
			{
				case SpellType.Ressurect:
					foreach (Guid id in inrange)
					{
						if (ais[id].GetStats().Health <= 0)
						{
							ais[id].IsZombie = true;
							ZombieSwap(id, grid, arch);
                            Statistics.Ressurections++;
						}
					}
					break;
                case SpellType.DeadAgain:
                    foreach (Guid id in zombs)
                    {
                        AiSwap(id, grid, arch);
                    }
                    break;
                case SpellType.RestoreEssence:
                    foreach (Guid id in zombs)
                    {
                        if (zombies[id].GetStats().Health < zombies[id].RHealth)
                        {
                            zombies[id].GetStats().Health += attacker.Stats.Damage;
                            zombies[id].GetStats().Health = MathHelper.Clamp(zombies[id].GetStats().Health, 0, zombies[id].RHealth);
                         
                        }
                    }
                    break;
                case SpellType.DrainEssence:
                    foreach (Guid id in inrange)
                    {
                        if (ais.ContainsKey(id))
                            ais[id].GetStats().Health -= attacker.Stats.Damage;
                    }
                    break;
                case SpellType.ClearDead:
                    var dead = GetUnitsInRange(attacker.Center, range, false);
                    foreach (Guid id in dead)
                    {
                        if (ais[id].Ai_States == Ai_States.Dead)
                            remove_queue.Add(id);
                    }
                    break;


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
                if(ai.Ai_States == Ai_States.Dead)
				    ai.Draw(sb);
			}

            foreach (Ai ai in ais.Values)
            {
                if (ai.Ai_States != Ai_States.Dead)
                {
                    ai.Draw(sb);
                    ((AiUnit)ai).Draw_Status(sb, null);

                }
                
            }

            foreach (Zombie z in zombies.Values)
			{
				((Ai)z).Draw(sb);
				z.Draw_Status(sb, null);
			}

			//sb.DrawString(DebugFont, "Total: " + ais.Count().ToString(), Position, Color.Black);
			//sb.DrawString(DebugFont, "Moving: " + moving.ToString(), Position + new Vector2(300, 0), Color.Black);
			//sb.DrawString(DebugFont, "Attacking: " + attack.ToString(), Position + new Vector2(600, 0), Color.Black);
			//sb.DrawString(DebugFont, "Thinking: " + think.ToString(), Position + new Vector2(900, 0), Color.Black);
		}

		void Spawn(Grid grid, Random rng)
		{
            if (spawnY < 0)
            {
                s_elasped = 0;
                spawnY = 0;
                formations.RemoveAt(0);
                ys.RemoveAt(0);
                spawning = false;
                return;
            }

            for (int x = formations.First().GetLength(0) - 1; x >= 0; x--)
            {
                int posy = ys.First();
                int posx = 5;
                Stats nstats = new Stats(ai_stats[formations.First()[x, spawnY]]);
                Type_Data<Ai_Type> nut = new Type_Data<Ai_Type>(unit_types[formations.First()[x, spawnY]]);
                var unit = new Grounded(new Vector2(posx, posy + (64*x)), new Point(32, 32), nut, nstats, grid);
                ais.Add(unit.AID, unit);
            }

            spawnY--;
            spawnX = 0;
		}

        

        void CreateNewFormation(Random rng, Grid grid)
        {
            int rank = rng.Next(1, (int)((Options.Difficulty + 3 )/ 100000) + 2); // deep
            int file = rng.Next(1, (int)((Options.Difficulty + 3 )/ 100000) + 2); //width

            rank = MathHelper.Clamp(rank, 1,12);
            file = MathHelper.Clamp(file, 1,6);

            spawnY = file - 1;

            formations.Add(new Ai_Type[rank, file]);

            //horseman at the side
            //swords at front
            //priests behind
            //archers behind

            //fill formation
            Ai_Type front;
            Ai_Type flank;
            Ai_Type range1;
            Ai_Type range2;

            int frontDepth = rng.Next(1, formations.Last().GetLength(1));
            int vanguardRankMin = (int)(rng.Next(0, formations.Last().GetLength(0)) /2);
            int vanguardRankMax = formations.Last().GetLength(0) - vanguardRankMin;
            int range1Depth = rng.Next(frontDepth, formations.Last().GetLength(1));
            int range2Depth = rng.Next(range1Depth, formations.Last().GetLength(1));

            for (int x = 0; x < rank; x++)
            {
                for (int y = 0; y < file; y++)
                {
                    if (x < vanguardRankMin && y < range1Depth)
                        formations.Last()[x, y] = Ai_Type.Horseman;
                    else if (x > vanguardRankMax && y < range1Depth)
                        formations.Last()[x, y] = Ai_Type.Horseman;
                    else if (y <= frontDepth)
                        formations.Last()[x, y] = Ai_Type.Knight;
                    else if (y <= range1Depth)
                        formations.Last()[x, y] = Ai_Type.Archer;
                    else if (y <= range2Depth)
                        formations.Last()[x, y] = Ai_Type.Priest;
                }
            }
            int fy = formations.Last().GetLength(0) * 64;
            int gy = grid.GetSize().Y * 64;
            int gf = gy - fy;
            int rfg = rng.Next(0, gf);
            ys.Add(rfg);
        }

        void ZombieSwap(Guid ai, Grid grid, Architech arch)  //swaps a unit to an equalivalent zombie unit
		{
			var old = ais[ai];
			var zombi = new Zombie(old.Position, new Point(32, 32), unit_types[ConvertToZombie(old.GetStats().Type)], ai_stats[ConvertToZombie(old.GetStats().Type)], grid, arch);
			zombies.Add(zombi.ID, zombi);
			ais.Remove(ai);
		}

        void AiSwap(Guid ai, Grid grid, Architech arch)  //swaps a unit to an equalivalent zombie unit
        {
            var old = zombies[ai];
            var unit = new Grounded(old.Position, new Point(32, 32), unit_types[ConvertToGrounded(old.GetStats().Type)], ai_stats[ConvertToGrounded(old.GetStats().Type)], grid);
            unit.GetStats().Health = 0;
            ais.Add(unit.ID, unit);
            zombies.Remove(ai);
        }

        public void ChangeStats(float percent, List<Ai_Type> ai_types, Researchables stat)
        {
            foreach (Ai_Type at in ai_types)
            {
                ApplyStat(at, stat, percent);
            }
        }

        void ApplyStat(Ai_Type ai, Researchables stat, float percent)
        {
            var temp_stat = Stats.Empty();
            switch (stat)
            {
                case Researchables.ZSpeed:
                    var mod = Extensions.Extensions.PercentT(ai_stats[ai].Speed, percent);
                       temp_stat.Speed += mod;
                       ai_stats[ai] += temp_stat;
                    break;
                case Researchables.ZHealth:
                    temp_stat.Health += Extensions.Extensions.PercentT(ai_stats[ai].Health, percent);
                    ai_stats[ai] += temp_stat;
                    break;
                case Researchables.ZDamage:
                    temp_stat.BaseDamage += (int)Extensions.Extensions.PercentT(ai_stats[ai].BaseDamage, percent);
                    ai_stats[ai] += temp_stat;
                    break;
                case Researchables.AiDamage:
                    temp_stat.BaseDamage += (int)Extensions.Extensions.PercentT(ai_stats[ai].BaseDamage, percent);
                    ai_stats[ai] += temp_stat;
                    break;
                case Researchables.AiHealth:
                    temp_stat.Health += (int)Extensions.Extensions.PercentT(ai_stats[ai].BaseDamage, percent);
                    ai_stats[ai] += temp_stat;
                    break;
                case Researchables.AiSpeed:
                    temp_stat.Speed += (int)Extensions.Extensions.PercentT(ai_stats[ai].BaseDamage, percent);
                    ai_stats[ai] += temp_stat;
                    break;

            }
        }

        public int GetAliveAis()
        {
            int count = 0;
            foreach (var ai in ais)
            {
                if (ai.Value.Ai_States != Ai_States.Dead)
                    count++;
            }

            return count;
        }

		public Dictionary<Guid, Ai> Ais
		{
			get { return ais; }
		}

		public Dictionary<Guid, Zombie> Zombies
		{
			get { return zombies; }
		}

        public Dictionary<Ai_Type, Stats> GetStats
        {
            get { return ai_stats; }
        }

		public Ai_Type ConvertToZombie(Ai_Type ai)
		{
			string temp = ai.ToString();

			return (Ai_Type)Enum.Parse(typeof(Ai_Type), "Z_" + temp);
		}

        public Ai_Type ConvertToGrounded(Ai_Type ai)
        {
            string temp = ai.ToString();

            return (Ai_Type)Enum.Parse(typeof(Ai_Type), temp.Substring(2));
        }
    }
}
