using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.Interactable;
using AHH.UI;
using AHH.User;
using AHH.Parsers;
using AHH.Extensions;
using System.Threading;

namespace AHH.Base
{
	class Grid : InteractableStaticSprite
	{
		static Point tileSize { get; set; }
		Dictionary<ButtonFunction, string> actions;
        Dictionary<string, Building> BuildingData;
		Point dimensions;
		Tile[,] tiles;
		ObservableCollection<Point> selectedTiles;
		DropMenu menu;
		Thread pathfinder_test; 
		bool buildingPlaced { get; set; }
		bool drawBuildingGhost = false;
		bool open_menu = false;
		public Grid(Point dimensions, Vector2 position, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, Point tileSize, string buildingFilePath, string gridUiFilePath, ContentManager cm) 
			: base(position, new Point(dimensions.X * tileSize.X, dimensions.Y * tileSize.Y ), null, null, null)
		{
			Grid.tileSize = tileSize;
            BuildingData = Parsers.Parsers.Parse_Buildings(buildingFilePath, cm);
			selectedTiles = new ObservableCollection<Point>();
			selectedTiles.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.CollectionChanged);
			tiles = new Tile[dimensions.X, dimensions.Y];
			this.dimensions = dimensions;
			buildingPlaced = false;
			for (int y = 0; y < dimensions.Y; y++)
			{
				for (int x = 0; x < dimensions.X; x++)
				{
					tiles[x, y] = new Tile(new Vector2(x * tileSize.X,  (y * tileSize.Y) + position.Y), texture, t_highlighted, t_clicked, tileSize, new Point(x, y));
				}
			}
			actions = Parsers.Parsers.Parse_InternalGridMenu(@"Content/UI/internal_ui_grid_menu.txt");
			menu = new DropMenu(Position, Parsers.Parsers.Parse_UiElements(gridUiFilePath, cm), new Point(256, 128));
			//pathfinder_test = new Thread(() => Pathfinder(new Vector2(500, 300)));
		}

		public Tile GetHighlightedTile()
		{
			foreach (Tile tile in tiles)
			{
				if (tile.IsHighlighted)
					return tile;
			}

			return null;
		}

		public void SelectTiles(Point from, Point dimensions)
		{
			selectedTiles.Clear();
			for (int x = from.X; x < (from.X + dimensions.X); x++)
			{
				for (int y = from.Y; y < (from.Y + dimensions.Y); y++)
				{
					selectedTiles.Add(new Point(x, y));
				}
			}

		}

		public void SelectTile(Point point)
		{
			selectedTiles.Clear();
			selectedTiles.Add(point);

		}

		public void ChangeSelectedTilesState(TileStates to)
		{
			foreach (Point p in selectedTiles)
			{
				tiles[p.X, p.Y].State = to;
			}
		}

		void CollectionChanged(object sender, EventArgs e)
		{



		}

		public void Update(Cursor ms, Player player)
		{
			base.Update(ms);

			buildingPlaced = false;

			if ((IsClicked || IsHighlighted || ms.isRightPressed) && !open_menu)
			{
				foreach (Tile tile in tiles)
				{
					tile.Update(ms);
					if (tile.IsClicked || (ms.isRightPressed && tile.IsHighlighted))
						SelectTile(tile.Order);
				}
			}

			if (ms.isRightPressed)
			{
				drawBuildingGhost = true;
				IsClicked = false;
				open_menu = true;
				menu.Reset(ms);
			}

			if (open_menu)
			{
				menu.Update(ms);
			}

			if (IsClicked)
			{
				if (menu.Action != null && open_menu)
				{
					ParseAction(menu.Action, player);
				}

				open_menu = false;
			}

			foreach (Tile tile in tiles)
			{
				tile.UpdateBuilding(ms);
			}

			if (player.Mode == Player_Modes.Building)
				drawBuildingGhost = true;
			else if(!open_menu) drawBuildingGhost = false;

			if (player.Input.IsPressed(Ctrls.HotKey_Build) && selectedTiles.Count > 0)
            {
                PlaceBuilding(player.SelectedBuilding, player);
            }

			//if (!pathfinder_test.IsAlive)
			//{
				//pathfinder_test = new Thread(() => Pathfinder(new Vector2(400, 300)));
				//pathfinder_test.Start();
			//}


		}

        public void PlaceBuilding(string buildingID, Player player)
        {
			SelectTiles(selectedTiles[0], new Point(BuildingData[buildingID].GetSize().X / 64, BuildingData[buildingID].GetSize().Y / 64));

			foreach (Point p in selectedTiles)
            {
               if(tiles[p.X, p.Y].State == TileStates.Blocked)
                    return; //cannot build as one of the selected tiles is blocked
            }

            tiles[selectedTiles[0].X, selectedTiles[0].Y].PlaceBuilding(BuildingData[buildingID].DeepCopy());
            foreach (Point p in selectedTiles)
            {
                tiles[p.X, p.Y].State = TileStates.Blocked;
				tiles[p.X, p.Y].Parent = selectedTiles[0];

				if (p != selectedTiles[0])
					tiles[selectedTiles[0].X, selectedTiles[0].Y].AddChild(p);
            }

			buildingPlaced = true;
        }

		public Tile GetTile(Point point)
		{
			if (point.X < dimensions.X && point.Y < dimensions.Y)
				return tiles[point.X, point.Y];
			else return null;
		}

		public List<Building> NearBuildings(Vector2 position)
		{
			List<Building> nearBuildings = new List<Building>();
			foreach (Tile t in tiles)
			{
				if (t.Building != null)
				{
					float distanceTo = Vector2.Distance(position, t.Position);

					if (distanceTo < (64*5))
					{
						nearBuildings.Add(t.Building);
					}
				}
			}

			return nearBuildings;
		}

		public dynamic Pathfinder(Vector2 destination, Vector2 current)
		{
			Point start = ToGridPosition(destination, new Point(tiles[0, 0].Box.Width, tiles[0, 0].Box.Height));
			Point finish = ToGridPosition(current, new Point(tiles[0, 0].Box.Width, tiles[0, 0].Box.Height));
			//initialise grid with coordinates and counter
			WTuple<Vector2, TileStates, int>[,] grid = new WTuple<Vector2, TileStates, int>[dimensions.X,dimensions.Y];

			for (int x = 0; x < dimensions.X; x++)
			{
				for (int y = 0; y < dimensions.Y; y++)
				{
					grid[x, y] = new WTuple<Vector2, TileStates, int>(tiles[x, y].Position, tiles[x, y].State, 9999);
				}
			}

			List<Point> done = new List<Point>();
			List<Point> check = new List<Point>();
			check.Add(start);
			int counter = 0;
			grid[check[0].X, check[0].Y].Item3 = counter;
			counter++;
			while (check.Count > 0)
			{
				//check all around the tile and assign a counter
				Vector2 max = new Vector2(check[0].X + 1, check[0].Y + 1);
				Vector2 min = new Vector2(check[0].X - 1, check[0].Y - 1);


				if (check[0].X + 1 >= dimensions.X)
					max.X = dimensions.X - 1;
				if (check[0].Y + 1 >= dimensions.Y)
					max.Y = dimensions.Y - 1;
				if (check[0].X - 1 < 0)
					min.X = 0;
				if (check[0].Y - 1 < 0)
					min.Y = 0;

				for (int x = (int)min.X; x <= max.X; x++)
				{
					for (int y = (int)min.Y;  y <= max.Y; y++)
					{
						int i = done.FindIndex(f => f.X == x && f.Y == y);
						if ((x == check[0].X && y == check[0].Y) || i > -1  || grid[x, y].Item3 != 9999)
						{
							//Console.WriteLine(done.Count());
						}
						else
						{
							if (tiles[x, y].State == TileStates.Blocked || tiles[x, y].State == TileStates.Immpassable)
							{
								grid[x, y].Item2 = tiles[x, y].State;

							}
							else if (tiles[x, y].State == TileStates.Active)
							{
								grid[x, y].Item2 = TileStates.Active;
								grid[x, y].Item3 = grid[check[0].X, check[0].Y].Item3 + 1;
								check.Add(new Point(x, y));
							}
						}
					}
				}
					
				done.Add(check[0]);

				if (check[0] == finish)
				{
					check.Clear();
					return grid;
				}
				else check.RemoveAt(0);

				//counter++;
			}

#if DEBUG
			/*for (int y = 0; y < dimensions.Y; y++)
			{
				for (int x = 0; x < dimensions.X; x++)
				{
					Console.Write(grid[x, y].Item3 + ",");
				}

				Console.Write('\n');
			}*/
#endif
			done.Clear();

			return null;
		}

		static public Point ToGridPosition(Vector2 position, Point size)
		{
			return new Point((int)position.X / size.X, (int)position.Y / size.Y);
		}

		static public Vector2 ToWorldPosition(Point gridPos, Point size)
		{
			return new Vector2(gridPos.X * size.X, gridPos.Y * size.Y);
		}

		static public Point GetTileSize
		{
			get { return tileSize; }
		}

		public bool BuildingPlaced
		{
			get { return buildingPlaced; }
		}

        new public void Draw(SpriteBatch sb, string buildingID)
        {
            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    tiles[x, y].Draw(sb);
#if DEBUG
                    sb.DrawString(DebugFont, x.ToString(), new Vector2(x * 64, (y * 64) + Position.Y), Color.Black);
#endif
                }
            }

            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    if (tiles[x, y].Building != null)
                        tiles[x, y].Building.Draw(sb);
                }
            }
	
        }

		public void Ui_Draw(SpriteBatch sb, string buildingID)
		{
			if (drawBuildingGhost)
			{
				Tile tile = GetHighlightedTile();
				if (tile != null)
				{
					Rectangle newBox = new Rectangle(tile.Box.X, tile.Box.Y, BuildingData[buildingID].Box.Width, BuildingData[buildingID].Box.Height);
					sb.Draw(BuildingData[buildingID].Texture, newBox, Color.White);
				}
			}

			if (open_menu)
			{
				menu.Draw(sb);
			}
		}

		void Dismantle()
		{
			Point p = selectedTiles[0];
			Tile t = tiles[p.X, p.Y];

			Action<Point> clear = (parent) =>
			{
				tiles[parent.X, parent.Y].RemoveBuilding();

				foreach (Point child in tiles[parent.X, parent.Y].GetChildren)
				{
					tiles[child.X, child.Y].State = TileStates.Active;
					tiles[child.X, child.Y].EndChildhood();
				}
				 
				tiles[parent.X, parent.Y].ClearChildren();
				tiles[parent.X, parent.Y].State = TileStates.Active;
			};

			if (t.Building == null)
				clear(t.Parent);
			else
				clear(p);
		}

		void Examine()
		{

		}

		void ParseAction(string action, Player player)
		{
			ButtonFunction func = ButtonFunction.Examine;
			bool match = false;
			foreach (KeyValuePair<ButtonFunction, string> kv in actions)
			{
				if (kv.Value == action)
				{
					func = kv.Key;
					match = true;
					break;
				}
			}

			if (!match)
				return;

			switch (func)
			{
				case ButtonFunction.Build:
					PlaceBuilding(player.SelectedBuilding, player);
					break;
				case ButtonFunction.Dismantle:
					Dismantle();
					break;
				case ButtonFunction.Examine:
					Examine();
					break;

			}

		}
	}
}
