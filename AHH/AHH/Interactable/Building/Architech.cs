using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Base;
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
        Dictionary<BuildingTypes, Building_Type> building_types { get; }
        public Architech(ContentManager cm)
        {
            
            building_data = Parsers.Parsers.Parse_Stats<BuildingTypes, BuildingData>(@"Content\unit\unit_descr.txt");
            building_types = Parsers.Parsers.Parse_Types<BuildingTypes, Building_Type>(@"Content\unit\unit_types.txt", cm);
        }

        public List<Building> NearBuildings(Vector2 position)
        {
            List<Building> nearBuildings = new List<Building>();
            foreach (Building b in buildings.Values)
            {
                float distanceTo = Vector2.Distance(position, t.Position);

                if (distanceTo < (64 * 5))
                {
                    nearBuildings.Add(b);
                }
                
            }

            return nearBuildings;
        }

        public bool PlaceBuilding(Grid grid, BuildingTypes buildingID)
        {
            //get tiles the building will encompass
            grid.SelectTiles(grid.SelectedTiles[0], new Point(building_data[buildingID][0].Size.X / 64, building_data[buildingID][0].Size.Y / 64));
            //check no tile is blocked return if true
            if (grid.CheckBlocked())
            {
                //set tiles found to blocked
                grid.ChangeSelectedTilesState(TileStates.Blocked);
                //add building to list
                Building newBuilding = new Building(new Building(grid.GetTile(grid.SelectedTiles[0]).Position, building_data[buildingID][0], building_types[buildingID]));
                newBuilding.CalculateCorners();
                newBuilding.InitAdjacent(grid);
                buildings.Add(grid.GetTile(grid.SelectedTiles[0]).Order, newBuilding);
                return true; //success
            }
            else return false; // cannot build

            
        }

        public void Examine()
        {


        }

        public bool Dismantle(Grid grid, Building b)
        {
            grid.SelectedTiles = b.GetChildren;

            bool done = false;

            foreach (Point p in grid.SelectedTiles)
            {
                if (buildings.ContainsKey(p))
                {
                    buildings.Remove(p);
                    done = true;
                }
            }

            if(done)
                grid.ChangeSelectedTilesState(TileStates.Active);

            return done;
        }

        public void Update(Grid grid, Cursor ms, UiMaster ui)
        {
            foreach (Building b in buildings)
                b.Update(ms);
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Building b in buildings)
                b.Draw(sb);

        }

    }
}
