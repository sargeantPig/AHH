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
using AHH.Interactable.Building;
namespace AHH.Base
{
	class Grid : InteractableStaticSprite
	{
		static Point tileSize { get; set; }
		static Vector2 _position { get; set; }
		Dictionary<ButtonFunction, string> actions;
		Point dimensions;
		Tile[,] tiles;
		List<Point> selectedTiles { get; set; }

		public Grid(Point dimensions, Vector2 position, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, Point tileSize, string buildingFilePath, string gridUiFilePath, ContentManager cm)
			: base(position, new Point(dimensions.X * tileSize.X, dimensions.Y * tileSize.Y), null, null, null)
		{
			Grid.tileSize = tileSize;
			selectedTiles = new List<Point>();
			tiles = new Tile[dimensions.X, dimensions.Y];
			this.dimensions = dimensions;
			Grid._position = position;
			for (int y = 0; y < dimensions.Y; y++)
			{
				for (int x = 0; x < dimensions.X; x++)
				{
					tiles[x, y] = new Tile(new Vector2(x * tileSize.X, (y * tileSize.Y) + position.Y), texture, t_highlighted, t_clicked, tileSize, new Point(x, y));
				}
			}
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

		public bool CheckBlocked() //returns true or false depending on whether or not a tile is blocked within the current selection
		{
			foreach (Point p in selectedTiles)
			{
				try
				{
					if (tiles[p.X, p.Y].State == TileStates.Blocked)
						return false;
				}

				catch
				{
					return false;
				}
			}
			return true;
		}

		public void ChangeSelectedTilesState(TileStates to)
		{
			foreach (Point p in selectedTiles)
			{
				tiles[p.X, p.Y].State = to;
			}
		}

		public List<Vector2> CheckPositions(List<Vector2> positions) // sanitize 
		{
			List<Vector2> sanitized = new List<Vector2>();
			foreach (Vector2 p in positions)
			{
				Point a = ToGridPosition(p, GetTileSize);
				try
				{
					if (tiles[a.X, a.Y].State == TileStates.Active)
					{
						sanitized.Add(p);
					}
				}
				catch { }
			}
			return sanitized;
		}

		public void Update(Player player)
		{
			base.Update(player.Cursor);

			foreach (Tile tile in tiles)
			{
				tile.Update(player.Cursor);
				if (tile.IsClicked || (player.Cursor.isRightPressed && tile.IsHighlighted))
					SelectTile(tile.Order);
			}
		}

		public Tile GetTile(Point point)
		{
			if (point.X < dimensions.X && point.Y < dimensions.Y)
				return tiles[point.X, point.Y];
			else return null;
		}

		public Point GetGridDimensions
		{
			get { return new Point(tiles.GetLength(0), tiles.GetLength(1)); }
		}

		public dynamic Pathfinder(Vector2 destination, Vector2 current)
		{
			Point start = ToGridPosition(destination, new Point(tiles[0, 0].Box.Width, tiles[0, 0].Box.Height));
			Point finish = ToGridPosition(current, new Point(tiles[0, 0].Box.Width, tiles[0, 0].Box.Height));
			//initialise grid with coordinates and counter
			WTuple<Vector2, TileStates, int>[,] grid = new WTuple<Vector2, TileStates, int>[dimensions.X, dimensions.Y];

			for (int x = 0; x < dimensions.X; x++)
			{
				for (int y = 0; y < dimensions.Y; y++)
				{
					grid[x, y] = new WTuple<Vector2, TileStates, int>(tiles[x, y].Position, tiles[x, y].State, 9999);
				}
			}

			List<Point> done = new List<Point>();
			List<Point> check = new List<Point>();

			if (this.GetTile(start).State == TileStates.Blocked)
				return TileStates.Blocked;
			if (this.GetTile(finish).State == TileStates.Blocked)
				return TileStates.Blocked;

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
					for (int y = (int)min.Y; y <= max.Y; y++)
					{
						int i = done.FindIndex(f => f.X == x && f.Y == y);
						if ((x == check[0].X && y == check[0].Y) || i > -1 || grid[x, y].Item3 != 9999)
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

			done.Clear();

			return null;
		}

		static public Point ToGridPosition(Vector2 position, Point size)
		{
			Point p = new Point();
			if (position.X == 0)
				p.X = 0;
			else p.X = (int)position.X / size.X;
			if (position.Y == 0)
				p.Y = 0;
			else p.Y = (int)(position.Y ) / size.Y;
			/*if (position.X % size.X != 0)
                p.X++;
            if (position.Y % size.X != 0)
                p.Y--;*/


			return p;
		}

		static public Vector2 ToWorldPosition(Point gridPos, Point size)
		{
			return new Vector2(gridPos.X * size.X, gridPos.Y * size.Y);
		}

		static public Point GetTileSize
		{
			get { return tileSize; }
		}

		static public Vector2 _Position
		{
			get { return _position; }
		}

        public void Draw(SpriteBatch sb, string buildingID)
        {
            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    tiles[x, y].Draw(sb);
                    tiles[x, y].Draw_Debug(sb);
                }
            }

        }

        public List<Point> SelectedTiles
        {
            get { return selectedTiles; }
            set { selectedTiles = value; }
        }

    }

}
