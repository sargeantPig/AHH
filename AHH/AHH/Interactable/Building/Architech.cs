using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Base;
using AHH.Interactable.Spells;
using AHH.UI;
using AHH.User;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AHH.Extensions;
using AHH.AI;
using AHH.Research;
using AHH.UI.Elements;
using AHH.Functions;

namespace AHH.Interactable.Building
{
	class Architech : BaseObject
	{
		Dictionary<Point, Building> buildings { get; set; }
		Dictionary<BuildingTypes, BuildingData> building_data { get; }
		Dictionary<BuildingTypes, Type_Data<BuildingTypes>> building_types { get; }
		List<Point> remove_queue = new List<Point>();
		string selectedBuilding;
		bool buildingPlaced { get; set; }
        bool buildingDestroyed { get; set; }

		Point home { get; }
        float orig_cost;
		public Architech(ContentManager cm, Grid grid)
		{
			selectedBuilding = "";
			buildings = new Dictionary<Point, Building>();
			building_data = Parsers.Parsers.Parse_Stats<BuildingTypes, BuildingData>(@"Content\buildings\building_descr.txt");
			building_types = Parsers.Parsers.Parse_Types<BuildingTypes, Type_Data<BuildingTypes>>(@"Content\buildings\building_types.txt", cm);
			home = new Point(25, 7);
			PlaceBuilding(home, grid, BuildingTypes.NTower);

            RefreshData(false);
            orig_cost = building_data[BuildingTypes.EnergyConduit].Cost;
		}

        public void RefreshData(bool statChange)
        {
            BuildingTypes[] bts = new BuildingTypes[] { BuildingTypes.EnergyConduit, BuildingTypes.Grave, BuildingTypes.NecroticOrrery,
            BuildingTypes.NTower, BuildingTypes.Wall, BuildingTypes.Demolish };

            foreach (BuildingTypes bt in bts)
            {
                if (statChange && bt == BuildingTypes.EnergyConduit)
                {
                    var building = new BuildingData(building_data[bt]);
                    var empty = BuildingData.Empty();
                    empty.Cost = building_data[bt].OriginalCost + (float)((GetCountByType(BuildingTypes.EnergyConduit) * (building_data[bt].OriginalCost * 0.20)));
                    building = BuildingData.SetCost(building, empty);
                    building_data[bt] = new BuildingData(building);
                    building_data[bt].Info.Picture = building_types[bt].Texture;
                }

                if (statChange && bt == BuildingTypes.Grave)
                {
                    var building = new BuildingData(building_data[bt]);
                    var empty = BuildingData.Empty();
                    empty.Cost = building_data[bt].OriginalCost + (float)((GetCountByType(BuildingTypes.Grave) * (building_data[bt].OriginalCost * 0.10)));
                    building = BuildingData.SetCost(building, empty);
                    building_data[bt] = new BuildingData(building);
                    building_data[bt].Info.Picture = building_types[bt].Texture;
                }

                if (statChange && bt == BuildingTypes.Wall)
                {
                    var building = new BuildingData(building_data[bt]);
                    var empty = BuildingData.Empty();
                    empty.Cost = building_data[bt].OriginalCost + (float)((GetCountByType(BuildingTypes.Wall) * (building_data[bt].OriginalCost * 0.05)));
                    building = BuildingData.SetCost(building, empty);
                    building_data[bt] = new BuildingData(building);
                    building_data[bt].Info.Picture = building_types[bt].Texture;
                }

                if (statChange && bt == BuildingTypes.NecroticOrrery)
                {
                    var building = new BuildingData(building_data[bt]);
                    var empty = BuildingData.Empty();
                    empty.Cost = building_data[bt].OriginalCost + (float)((GetCountByType(BuildingTypes.NecroticOrrery) * (building_data[bt].OriginalCost * 0.10)));
                    building = BuildingData.SetCost(building, empty);
                    building_data[bt] = new BuildingData(building);
                    building_data[bt].Info.Picture = building_types[bt].Texture;
                }

                else
                {
                    building_data[bt] = new BuildingData(building_data[bt]);
                    building_data[bt].Info.Picture = building_types[bt].Texture;
                }
            }

        }

		public Dictionary<Point, Building> NearBuildings(Vector2 position, float range)
		{
			Dictionary<Point, Building> nearBuildings = new Dictionary<Point, Building>();
			foreach (KeyValuePair<Point, Building> kv in buildings)
			{
				float distanceTo = Vector2.Distance(position, kv.Value.Position);

				if (distanceTo < (32 * range))
				{
					nearBuildings.Add(kv.Key, kv.Value);
				}

			}

			return nearBuildings;
		}

		public KeyValuePair<Point, Building> NearBuilding(Vector2 position, float range)
		{
			Dictionary<Point, Building> nearBuildings = NearBuildings(position, range);

			if (nearBuildings.Count == 0 && buildings.Count == 0)
				return new KeyValuePair<Point, Building>(home, GetBuilding(home));


			KeyValuePair<Point, Building> closest = nearBuildings.First();
			float tempdis = int.MaxValue;

			foreach (KeyValuePair<Point, Building> kv in nearBuildings)
			{
				float distanceTo = Vector2.Distance(position, kv.Value.Position);

				if (distanceTo < tempdis)
				{
					tempdis = distanceTo;
					closest = kv;
				}

			}

			return closest;
		}

		public bool IsInRange(Dictionary<Corner, Vector2> corners, Point bID, float range)
		{
        
			foreach (Vector2 v in corners.Values)
			{
                if (CheckDistanceY(buildings[bID].Corners[Corner.TopLeft], buildings[bID].Corners[Corner.BottomLeft], v, range))
                    return true;
                if (CheckDistanceX(buildings[bID].Corners[Corner.TopLeft], buildings[bID].Corners[Corner.TopRight], v, range))
                    return true;
                if (CheckDistanceY(buildings[bID].Corners[Corner.TopRight], buildings[bID].Corners[Corner.BottomRight], v, range))
                    return true;
                if (CheckDistanceX(buildings[bID].Corners[Corner.BottomLeft], buildings[bID].Corners[Corner.BottomRight], v, range))
                    return true;
            }

			return false;
		}

        public bool CheckDistanceY(Vector2 min, Vector2 max, Vector2 value, float range)
        {
            for (int y = (int)min.Y; y < max.Y; y++)
            {
                    var temp = Vector2.Distance(value, new Vector2(min.X, y));
                    if (temp <= (range) * 20)
                        return true;
            }

            return false;
        }
        public bool CheckDistanceX(Vector2 min, Vector2 max, Vector2 value, float range)
        {
            for (int y = (int)min.X; y < max.X; y++)
            {
                var temp = Vector2.Distance(value, new Vector2(y, min.Y));
                if (temp <= (range) * 20)
                    return true;
            }

            return false;
        }

        public Point GetGrave(Overseer os)
		{
			Point grave = new Point();

			Dictionary<Point, WTuple<Point, Building, int>> temp = new Dictionary<Point, WTuple<Point, Building, int>>();

			foreach (KeyValuePair<Point, Building> kv in buildings)
			{
				if (kv.Value.GetBuildingData().Type == BuildingTypes.Grave)
					temp.Add(kv.Key, new WTuple<Point, Building, int>(kv.Key, kv.Value, 0));
			}

			if (temp.Count == 0)
				return home;

			foreach (Zombie zom in os.Zombies.Values)
			{
				if (temp.ContainsKey(zom.Home))
					temp[zom.Home].Item3++;

			}

			int lowest = 100;

			foreach (var t in temp)
			{
				if (t.Value.Item3 < lowest)
				{
					grave = t.Key;
					lowest = t.Value.Item3;
				}
			}

			return grave;
		}

		public bool PlaceBuilding(Grid grid, BuildingTypes buildingID)
		{
			//get tiles the building will encompass
			grid.SelectTiles(grid.SelectedTiles[0], new Point(building_data[buildingID].Size.X / 64, building_data[buildingID].Size.Y / 64));

			//check no tile is blocked return if true
			if (grid.CheckBlocked())
			{
				//add building to list
				Building newBuilding = new Building(grid.GetTile(grid.SelectedTiles[0]).Position, building_data[buildingID], building_types[buildingID]);
				newBuilding.InitAdjacent(grid);
				newBuilding.GetChildren = new List<Point>();

				foreach (Point p in grid.SelectedTiles)
				{
					newBuilding.GetChildren.Add(new Point(p.X, p.Y));
				}

				if (buildings.ContainsKey(grid.GetTile(grid.SelectedTiles[0]).Order))
					return false;
				grid.ChangeSelectedTilesState(TileStates.Limbo);
				buildings.Add(grid.GetTile(grid.SelectedTiles[0]).Order, newBuilding);
                RefreshData(newBuilding.GetBuildingData().Type == BuildingTypes.EnergyConduit);
                Statistics.BuildingsBuilt++;
				//buildingPlaced = true;
				return true; //success
			}
			else return false; // cannot build
		}

		private bool PlaceBuilding(Point point, Grid grid, BuildingTypes buildingID)
		{
			//get tiles the building will encompass
			grid.SelectTiles(point, new Point(building_data[buildingID].Size.X / 64, building_data[buildingID].Size.Y / 64));

			//check no tile is blocked return if true
			if (grid.CheckBlocked())
			{
				//add building to list
				Building newBuilding = new Building(grid.GetTile(grid.SelectedTiles[0]).Position, building_data[buildingID], building_types[buildingID]);
				newBuilding.InitAdjacent(grid);
				newBuilding.GetChildren = new List<Point>();
				foreach (Point p in grid.SelectedTiles)
				{
					newBuilding.GetChildren.Add(new Point(p.X, p.Y));
				}

				if (buildings.ContainsKey(grid.GetTile(grid.SelectedTiles[0]).Order))
					return false;
				grid.ChangeSelectedTilesState(TileStates.Limbo);
				buildings.Add(grid.GetTile(grid.SelectedTiles[0]).Order, newBuilding);
				//buildingPlaced = true;
				return true; //success
			}
			else return false; // cannot build
		}

		public void BuildComplete(Building b, Grid grid, UiMaster ui)
		{
			grid.SelectedTiles = new List<Point>(b.GetChildren);
			//set tiles found to blocked
            if(b.GetBuildingData().Type != BuildingTypes.Grave)
			    grid.ChangeSelectedTilesState(TileStates.Blocked);
            ui.Messenger.AddMessage(new Text(Vector2.Zero, b.GetBuildingData().Name + " Constructed", Color.DodgerBlue));
			buildingPlaced = true;
		}

		public bool Dismantle(Grid grid, Building b, Player player)
		{
            RefreshData(true);

            player.IncreaseEnergy += 100;

			grid.SelectedTiles = new List<Point>(b.GetChildren);
			grid.ChangeSelectedTilesState(TileStates.Active);

			return true;
		}

        void Dismantle(Grid grid, Player player)
        {
            buildingDestroyed = true;

            if (grid.SelectedTiles[0] == home)
                return;

            if (buildings.ContainsKey(grid.SelectedTiles[0]))
            {
                Dismantle(grid, buildings[grid.SelectedTiles[0]], player);
                buildings.Remove(grid.SelectedTiles[0]);
            }
            else
            {
                Point p = FindParent(grid.SelectedTiles[0]);

                if (p == home)
                    return;

                if (buildings.ContainsKey(p))
                {
                    Dismantle(grid, buildings[p], player);
                    buildings.Remove(p);
                }

                else return;
            }
        }

        Point FindParent(Point p)
        {
            foreach (var building in buildings)
            {
                foreach (var po in building.Value.GetChildren)
                {
                    if (p == po)
                        return building.Value.GetChildren[0];
                }
            }

            return Point.Zero;
        }

		public List<Point> CompileBuildingTiles()
		{
			List<Point> points = new List<Point>();
			foreach (Building b in buildings.Values)
			{
				foreach (Point p in b.GetChildren)
				{
					points.Add(p);
				}
			}
			return points;
		}
        public void ChangeStats(float percent, List<BuildingTypes> b_types, Researchables stat)
        {
            foreach (BuildingTypes at in b_types)
            {
                ApplyStat(at, stat, percent);
            }

            RefreshData(false);
        }

        void ApplyStat(BuildingTypes ai, Researchables stat, float percent)
        {
            var temp_stat = BuildingData.Empty();
            switch (stat)
            {
                case Researchables.WHealth:
                    var mod = Extensions.Extensions.PercentT(building_data[ai].Health, percent);
                    temp_stat.Health += mod;
                    building_data[ai] += temp_stat;
                    break;
                case Researchables.WCost:
                    temp_stat.OriginalCost += Extensions.Extensions.PercentT(building_data[ai].Cost, percent);
                    building_data[ai] += temp_stat;
                    break;
                case Researchables.WProduct:
                    if (ai == BuildingTypes.EnergyConduit)
                    {
                        temp_stat.Production += (int)Extensions.Extensions.PercentT(building_data[ai].Production, percent);
                        building_data[ai] += temp_stat;
                    }
                    break;
            }
        }

        public void SpellEffect(Spell spell)
		{
			// get buildings in radius


			// apply spell effect
		}

		public void Update(Grid grid, UiMaster master, Overseer os, Player player, GameTime gt, List<Rectangle> units)
		{
			buildingPlaced = false;
            buildingDestroyed = false;

            

			foreach (KeyValuePair<Point, Building> kv in buildings)
			{
				kv.Value.Update(player, gt, this, grid, units, master);

                if (kv.Value.State == BuildingStates.Production)
                    kv.Value.Produce(player, gt, os);

				if (kv.Value.GetBuildingData().Health <= 0)
				{
					remove_queue.Add(kv.Key);
                    
				}
			}

			foreach (Point id in remove_queue)
			{
				Dismantle(grid, GetBuilding(id), player);
				buildings.Remove(id);
			}
			remove_queue.Clear();

			if (master.NextAction != ButtonFunction.Nan)
			{
				switch (master.NextAction)
				{
					case ButtonFunction.Wall:
						master.Pop_Action();
						selectedBuilding = ButtonFunction.Wall.ToString();
						break;
					case ButtonFunction.Grave:
						master.Pop_Action();
						selectedBuilding = ButtonFunction.Grave.ToString();
						break;
					case ButtonFunction.EnergyConduit:
						master.Pop_Action();
						selectedBuilding = ButtonFunction.EnergyConduit.ToString();
						break;
                    case ButtonFunction.NecroticOrrery:
                        master.Pop_Action();
                        selectedBuilding = ButtonFunction.NecroticOrrery.ToString();
                        break;
                    case ButtonFunction.Demolish:
                        master.Pop_Action();
                        selectedBuilding = ButtonFunction.Demolish.ToString();
                        break;
				}

			}

            if (master.Highlight != ButtonFunction.Nan)
            {
                switch (master.Highlight)
                {
                    case ButtonFunction.EnergyConduit:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(building_data[BuildingTypes.EnergyConduit].Id, 
                            building_data[BuildingTypes.EnergyConduit].Info));
                        RemoveInfo(master, new BuildingTypes[] { BuildingTypes.Grave, BuildingTypes.Wall, BuildingTypes.NecroticOrrery });
                        break;
                    case ButtonFunction.NecroticOrrery:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(building_data[BuildingTypes.NecroticOrrery].Id,
                           building_data[BuildingTypes.NecroticOrrery].Info));
                        RemoveInfo(master, new BuildingTypes[] { BuildingTypes.Grave, BuildingTypes.Wall, BuildingTypes.EnergyConduit });
                        break;
                    case ButtonFunction.Wall:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(building_data[BuildingTypes.Wall].Id,
                             building_data[BuildingTypes.Wall].Info));
                        RemoveInfo(master, new BuildingTypes[] { BuildingTypes.Grave, BuildingTypes.NecroticOrrery, BuildingTypes.EnergyConduit });
                        break;
                    case ButtonFunction.Grave:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(building_data[BuildingTypes.Grave].Id,
                             building_data[BuildingTypes.Grave].Info));
                        RemoveInfo(master, new BuildingTypes[] { BuildingTypes.Wall, BuildingTypes.NecroticOrrery, BuildingTypes.EnergyConduit });
                        break;
                    case ButtonFunction.Demolish:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(building_data[BuildingTypes.Demolish].Id,
                            building_data[BuildingTypes.Demolish].Info));
                        break;


                }
            }

           if(player.Mode != Player_Modes.Building) RemoveInfo(master, new BuildingTypes[] { BuildingTypes.Wall, BuildingTypes.NTower, BuildingTypes.Grave, BuildingTypes.EnergyConduit });


            if (player.Cursor.isLeftPressed && player.Cursor.GetState != player.Cursor.prevState &&
                grid.IsHighlighted && selectedBuilding != "" && player.Mode == Player_Modes.Building)
            {
                PlaceBuilding(grid, (BuildingTypes)Enum.Parse(typeof(BuildingTypes), selectedBuilding));
            }

            else if (player.Cursor.isLeftPressed && player.Cursor.GetState != player.Cursor.prevState &&
                grid.IsHighlighted &&  (player.Mode == Player_Modes.Tools && selectedBuilding == ButtonFunction.Demolish.ToString()))
            {
                Dismantle(grid, player);
            }
		}

        void RemoveInfo(UiMaster master, BuildingTypes[] buttonFunctions)
        {
            foreach (BuildingTypes bf in buttonFunctions)
            {
                master.RemoveInfo(building_data[bf].Id);
            }

        }

        public void Draw(SpriteBatch sb, Player player, Grid grid)
		{
			foreach (Building b in buildings.Values)
			{
				b.Draw(sb);

                if(player.SubMode.HasFlag(Sub_Player_Modes.Display_FHB))
				    b.Draw_Status(sb);
				//b.Draw_Debug(sb);
			}
			//draw ghost of building if in buildmode 
			if (player.Mode == Player_Modes.Building)
			{
				if (selectedBuilding != "")
				{
					if (grid.GetHighlightedTile() != null)
					{
						Vector2 pos = grid.GetHighlightedTile().Position;
						var td = building_types[(BuildingTypes)Enum.Parse(typeof(BuildingTypes), selectedBuilding)];
						var bd = building_data[(BuildingTypes)Enum.Parse(typeof(BuildingTypes), selectedBuilding)];
						sb.Draw(td.Texture, pos, new Rectangle(0, 0, bd.Size.X, bd.Size.Y), Color.White);
					}
				}
			}
		}

        public int GetOrrery()
        {
            int count = 0;
            foreach (var b in buildings)
            {
                if(b.Value.GetBuildingData().Type == BuildingTypes.NecroticOrrery)
                count++;
            }
            return count;
        }

        public int GetCountByType(BuildingTypes type)
        {
            int count = 0;
            foreach (var b in buildings)
            {
                if (b.Value.GetBuildingData().Type == type)
                    count++;
            }
            return count;
        }

		public bool BuildingPlaced
		{
			get { return buildingPlaced; }
		}
        public bool BuilingDestroyed
        {
            get { return buildingDestroyed; }
        }
		public Building GetBuilding(Point id)
		{
			if (buildings.ContainsKey(id))
				return buildings[id];
			else return null;
		}

		public int GetBuildingsBuilding()
		{
			int count = 0;

			foreach (var building in buildings.Values)
			{
				if (building.State == BuildingStates.Building)
					count += 1;
			}

			return count;
		}

		public Dictionary<Point, Building> GetBuildings
		{
			get { return buildings; }
		}

		public Point Home
		{
			get { return home; }
		}

        public bool IsHomeAlive
        {
            get
            {
                if(buildings.ContainsKey(home))
                    return true;
                else return false;
            }
        }
    }
}
