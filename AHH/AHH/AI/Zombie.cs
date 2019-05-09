using AHH.Base;
using AHH.Interactable.Building;
using AHH.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Extensions;
using AHH.UI.Elements;

namespace AHH.AI
{
	class Zombie : Grounded
	{
		Point home { get; set; }
		Guid target;
		float hitElasped = 0;

		const float stuckAttempts = 100;
        int attempts = 0;

        float temp_speed;
		public Zombie(Vector2 position, Point rectExtends, Type_Data<Ai_Type> types, Stats stats, Grid grid, Architech arch)
			: base(position, rectExtends, types, stats, grid)
		{
			Ai_States = Ai_States.Target;
			IsZombie = true;
			home = arch.Home;
            temp_speed = stats.Speed;
            RefreshInfo();
		}

        public void Update(Cursor ms, GameTime gt, Architech arch, Grid grid, Random rng, Overseer os, UiMaster ui)
        {
            if (Ai_States == Ai_States.Marching)
            {
                Ai_States = Ai_States.Target;
                this.Speed = temp_speed;
            }
            if (home == arch.Home || !arch.GetBuildings.ContainsKey(home))
                SetHome(arch, os);
                    
            base.Update(ms, gt);
            base.Update(gt);
            base.Update();
            CalculateCorners();
            GetFreeCorners(grid);

            RefreshInfo();

            if (IsHighlighted)
                ui.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(this.ID, Info));
            else ui.RemoveInfo(this.ID);

            foreach (var proj in projectile.Values)
            {
                if (os.Ais.ContainsKey(target))
                    proj.Update();
            }

            if (Ai_States == Ai_States.Dead)
                return;

            CheckTarget(os);

            switch (Ai_States)
            {
                case Ai_States.Idle:
                    if (wait)
                        Think_Pathing(grid, rng);
                    else if (PFResult != null)
                        Think_Pathing(grid, rng);
                    return;
                case Ai_States.Target:
                    EasyGetTarget(arch, os, grid, rng); //can go to idle or move
                    break;
                case Ai_States.Moving:
                    Moving(os, grid); // can go to idle or attacking
                    break;
                case Ai_States.Attacking:
                    Attacking(os, arch, gt, rng, grid); // can go to idle or target
                    break;
                case Ai_States.Dead:
                    return;

            }

            prevPos = Position;
        }

        void EasyGetTarget(Architech arch, Overseer os, Grid grid, Random rng)
        {
            if (!keepTarget)
            {
                var units = os.GetUnitsInRange(arch.GetBuilding(home).Center, 264 * 2, true);
                var focus = Think(rng);
                int index = 0;

                if (units.Count <= 0)
                {
                    GraveHover(arch, os, grid, rng);
                    return;
                }

                switch (focus)
                {
                    case Focus.Focused:
                        index = rng.Next((int)(units.Count * 0.8), units.Count);
                        break;
                    case Focus.Hyper:
                        index = rng.Next(0, (int)(units.Count * 0.3));
                        break;
                    case Focus.Aggressive:
                        index = rng.Next((int)(units.Count * 0.3), units.Count);
                        break;
                    case Focus.Violent:
                        index = rng.Next(0, (int)(units.Count * 0.1));
                        break;
                }

                var unit = units.ToList()[index];
                var Nunit = os.Ais[unit];

                destination = Grid.ToGridPosition(Nunit.Position, Grid.GetTileSize);
                target = unit;
            }

            else
            {
                if (!os.Ais.ContainsKey(target))
                {
                    keepTarget = !keepTarget;
                    return;
                }

                var Nunit = os.Ais[target];

                destination = Grid.ToGridPosition(Nunit.Position, Grid.GetTileSize);
            }

            Ai_States = Ai_States.Idle;
            Think_Pathing(grid, rng);
        }

		protected void Moving(Overseer os, Grid grid)
		{
			if (WayPoints.Count > 0)
				Moving();
			else
				Ai_States = Ai_States.Target;

			if (CheckTarget(os))
				return;
		}

        new protected void Moving()
        {
            bool reached = MoveTo(WayPoints[0]);
            if (reached)
            {
                WayPoints.RemoveAt(0);
            }
        }

        //idle around a grave checks surroundings for enemies constantly 
        void GraveHover(Architech arch, Overseer os, Grid grid, Random rnd)
        {
            var grave = arch.GetBuilding(home);

            if (grave == null)
                SetHome(arch, os);

            var b = arch.GetBuilding(home);
            var ts = b.AdjacentTiles;
            destination = Grid.ToGridPosition(ts[rnd.Next(0, ts.Count)], Grid.GetTileSize);

            Ai_States = Ai_States.Idle;
            Think_Pathing(grid, rnd);

        }

		//sets a home grave for the zombie a grave it will return to in less violent times
		void SetHome(Architech arch, Overseer os)
		{
			home = arch.GetGrave(os);
		}

        bool CheckTarget(Overseer os)
        {
            if (os.Ais.ContainsKey(target) )
            {
                if (os.IsInRange(Corners, target, (float)Stats.Range) && os.Ais[target].Ai_States != Ai_States.Dead)
                {
                    Ai_States = Ai_States.Attacking;
                    return true;
                }
                else if (os.Ais[target].Ai_States == Ai_States.Dead )
                { Ai_States = Ai_States.Target; keepTarget = !keepTarget; target = new Guid(); }
                else if (WayPoints.Count == 0 && Ai_States != Ai_States.Moving)
                { Ai_States = Ai_States.Target; keepTarget = !keepTarget; }

            }

            else if(Ai_States != Ai_States.Moving) { Ai_States = Ai_States.Target; target = new Guid(); WayPoints.Clear(); keepTarget = false; }

            return false;
        }

        //attack target once in range, go back to pursue if not in range
        void Attacking(Overseer os, Architech arch, GameTime gt, Random rng, Grid grid)
		{
			hitElasped += gt.ElapsedGameTime.Milliseconds;

			if (hitElasped >= Stats.HitDelay)
			{
				hitElasped = 0;
				os.Combat(this, (Grounded)os.Ais[target], rng);
				var p = new Projectile(Position, new Point(16, 16), data.Projectile, 5, ((Grounded)os.Ais[target]).Center);
				projectile.Add(p.ID, p);
			}
			
		}

		public Point Home
		{
			get { return home; }
			set { home = value; }
		}

	}
}
