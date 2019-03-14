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
namespace AHH.Base
{
	public enum TileStates
	{
		Active,
		Blocked,
		Immpassable,
	}

	class Tile : InteractableStaticSprite
	{
		Point order { get; }
		Building building { get; set; }
		TileStates state { get; set; }

		public Tile(Vector2 position, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, Point size, Point order) 
			: base(position, size, texture, t_highlighted, t_clicked)
		{
			this.order = order;
			state = TileStates.Active;
			building = null;
		}

		new public void Update(Cursor ms)
		{
			base.Update(ms);
		}

		public void UpdateBuilding(Cursor ms)
		{
			if (building != null)
			{
				building.Update(ms);
			}
		}

        public void PlaceBuilding(Building b)
        {
            b.Position = Position;
            b.Box = new Rectangle((int)Position.X, (int)Position.Y, b.Box.Width, b.Box.Height);
            Building = b;
        }


		public Building Building
		{
			get { return building; }
			set { building = value; }
		}

		public TileStates State
		{
			get { return state; }
			set { state = value; }
		}

		public Point Order
		{
			get { return order; }
		}

		public void RemoveBuilding()
		{
			building = null;
		}

	}

	class Grid : InteractableStaticSprite
	{

        Dictionary<string, Building> BuildingData;
		Point dimensions;
		Tile[,] tiles;
		ObservableCollection<Point> selectedTiles;

		public Grid(Point dimensions, Vector2 position, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, Point tileSize, string buildingFilePath, ContentManager cm ) 
			: base(position, new Point(dimensions.X * tileSize.X, dimensions.Y * tileSize.Y ), null, null, null)
		{
            BuildingData = Parsers.Parsers.Parse_Buildings(buildingFilePath, cm);
			selectedTiles = new ObservableCollection<Point>();
			selectedTiles.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.CollectionChanged);
			tiles = new Tile[dimensions.X, dimensions.Y];
			this.dimensions = dimensions;

			for (int y = 0; y < dimensions.Y; y++)
			{
				for (int x = 0; x < dimensions.X; x++)
				{
					tiles[x, y] = new Tile(new Vector2(x * tileSize.X,  (y * tileSize.Y) + position.Y), texture, t_highlighted, t_clicked, tileSize, new Point(x, y));
				}
			}
		}

		public void GetHighlightedTile()
		{
			
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

			if (IsClicked || IsHighlighted)
			{
				foreach (Tile tile in tiles)
				{
					tile.Update(ms);
					if (tile.IsClicked)
						SelectTile(tile.Order);
				}
			}

			foreach (Tile tile in tiles)
			{
				tile.UpdateBuilding(ms);
			}

            if (player.Input.IsPressed(Ctrls.HotKey_Build) && selectedTiles.Count > 0)
            {
                SelectTiles(selectedTiles[0], new Point(BuildingData[player.SelectedBuilding].GetSize().X / 64, BuildingData[player.SelectedBuilding].GetSize().Y / 64));

                PlaceBuilding(player.SelectedBuilding);

            }

		}

        public void PlaceBuilding(string buildingID)
        {
            foreach (Point p in selectedTiles)
            {
               if(tiles[p.X, p.Y].State == TileStates.Blocked)
                    return; //cannot build as one of the selected tiles is blocked
            }

            tiles[selectedTiles[0].X, selectedTiles[0].Y].PlaceBuilding(BuildingData[buildingID].DeepCopy());
            foreach (Point p in selectedTiles)
            {
                tiles[p.X, p.Y].State = TileStates.Blocked;
            }
        }

        new public void Draw(SpriteBatch sb)
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
	}
}
