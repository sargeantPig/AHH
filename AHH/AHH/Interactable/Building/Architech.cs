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

namespace AHH.Interactable.Building
{
    class Architech : BaseObject
    {
        Dictionary<Point, Building> buildings { get; set; }
        Dictionary<BuildingTypes, List<BuildingData>> building_data { get; }
        Dictionary<BuildingTypes, Type_Data<BuildingTypes>> building_types { get; }
        List<Point> remove_queue = new List<Point>();
        string selectedBuilding;
        bool buildingPlaced { get; set; }

        public Architech(ContentManager cm)
        {
            selectedBuilding = "";
            buildings = new Dictionary<Point, Building>();
            building_data = Parsers.Parsers.Parse_Stats<BuildingTypes, BuildingData>(@"Content\buildings\building_descr.txt");
            building_types = Parsers.Parsers.Parse_Types<BuildingTypes, Type_Data<BuildingTypes>>(@"Content\buildings\building_types.txt", cm);
        }

        public Dictionary<Point, Building> NearBuildings(Vector2 position, float range)
        {
            Dictionary<Point, Building> nearBuildings = new Dictionary<Point, Building>();
            foreach (KeyValuePair<Point, Building> kv in buildings)
            {
                float distanceTo = Vector2.Distance(position, kv.Value.Position);

                if (distanceTo < (64 * range))
                {
                    nearBuildings.Add(kv.Key, kv.Value);
                }

            }

            return nearBuildings;
        }

        public bool IsInRange(Dictionary<Corner, Vector2> corners, Point bID, float range)
        {

            if (Vector2.Distance(corners[Corner.TopLeft], buildings[bID].Corners[Corner.TopLeft]) <= range * 64)
                return true;
            if (Vector2.Distance(corners[Corner.TopRight], buildings[bID].Corners[Corner.TopRight]) <= range * 64)
                return true;
            if (Vector2.Distance(corners[Corner.BottomLeft], buildings[bID].Corners[Corner.BottomLeft]) <= range * 64)
                return true;
            if (Vector2.Distance(corners[Corner.BottomRight], buildings[bID].Corners[Corner.BottomRight]) <= range * 64)
                return true;
            return false;
        }

        public bool PlaceBuilding(Grid grid, BuildingTypes buildingID)
        {
            //get tiles the building will encompass
            grid.SelectTiles(grid.SelectedTiles[0], new Point(building_data[buildingID][0].Size.X / 64, building_data[buildingID][0].Size.Y / 64));

            //check no tile is blocked return if true
            if (grid.CheckBlocked())
            {
                //add building to list
                Building newBuilding = new Building(grid.GetTile(grid.SelectedTiles[0]).Position, building_data[buildingID][0], building_types[buildingID]);
                newBuilding.InitAdjacent(grid);
                newBuilding.GetChildren = new List<Point>(grid.SelectedTiles);
                if (buildings.ContainsKey(grid.GetTile(grid.SelectedTiles[0]).Order))
                    return false;
                grid.ChangeSelectedTilesState(TileStates.Limbo);
                buildings.Add(grid.GetTile(grid.SelectedTiles[0]).Order, newBuilding);
                //buildingPlaced = true;
                return true; //success
            }
            else return false; // cannot build
        }

        public void BuildComplete(Building b, Grid grid)
        {
            grid.SelectedTiles = b.GetChildren;
            //set tiles found to blocked
            grid.ChangeSelectedTilesState(TileStates.Blocked);

            buildingPlaced = true;
        }

		public bool Dismantle(Grid grid, Building b)
		{
			grid.SelectedTiles = b.GetChildren;

			
			grid.ChangeSelectedTilesState(TileStates.Active);

			return true;
		}

		public void SpellEffect(Spell spell)
		{
			// get buildings in radius


			// apply spell effect
		}

		public void Update(Grid grid, UiMaster master, Player player, GameTime gt)
		{
			buildingPlaced = false;

			foreach (KeyValuePair<Point, Building> kv in buildings)
			{
				kv.Value.Update(player.Cursor, gt, this, grid);
				if (kv.Value.GetBuildingData().Health <= 0)
				{
					remove_queue.Add(kv.Key);
				}
			}

			foreach (Point id in remove_queue)
			{
				Dismantle(grid, GetBuilding(id));
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
				}

			}

			if (player.Cursor.isLeftPressed && grid.IsHighlighted && selectedBuilding != "" && player.Mode == Player_Modes.Building)
			{
				PlaceBuilding(grid, (BuildingTypes)Enum.Parse(typeof(BuildingTypes), selectedBuilding));
			}
		}

		public void Draw(SpriteBatch sb, Player player, Grid grid)
		{
			foreach (Building b in buildings.Values)
				b.Draw(sb);

			//draw ghost of building if in buildmode 
			if (player.Mode == Player_Modes.Building)
			{
				if (selectedBuilding != "")
				{
					if (grid.GetHighlightedTile() != null)
					{
						Vector2 pos = grid.GetHighlightedTile().Position;
                        var td = building_types[(BuildingTypes)Enum.Parse(typeof(BuildingTypes), selectedBuilding)];
                        var bd = building_data[(BuildingTypes)Enum.Parse(typeof(BuildingTypes), selectedBuilding)][0];
                        sb.Draw(td.Texture, pos, new Rectangle(0, 0, bd.Size.X, bd.Size.Y), Color.White);
					}
				}
			}
		}

		public bool BuildingPlaced
		{
			get { return buildingPlaced; }
		}

		public Building GetBuilding(Point id)
		{
			if (buildings.ContainsKey(id))
				return buildings[id];
			else return null;
		}

    }
}
