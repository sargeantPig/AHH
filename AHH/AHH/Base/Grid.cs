using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.Interactable;
namespace AHH.Base
{
	class Tile : InteractableStaticSprite
	{
		Building building { get; set; }

		public Tile(Vector2 position, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, Point size) 
			: base(position, size, texture, t_highlighted, t_clicked)
		{
			building = null;
		}

		new public void Update(MouseState ms)
		{
			if (building != null)
			{
				building.Update(ms);

			}
			else base.Update(ms);
		}

		public Building Building
		{
			get { return building; }
			set { building = value; }
		}

		public void RemoveBuilding()
		{
			building = null;
		}


	}

	class Grid : BaseObject
	{
		Point dimensions;
		Tile[,] tiles;



		public Grid(Point dimensions, Vector2 position, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, Point tileSize ) 
			: base(position)
		{
			tiles = new Tile[dimensions.X, dimensions.Y];
			this.dimensions = dimensions;

			for (int y = 0; y < dimensions.Y; y++)
			{
				for (int x = 0; x < dimensions.X; x++)
				{
					tiles[x, y] = new Tile(new Vector2(position.X + x * tileSize.X, position.Y + y * tileSize.Y), texture, t_highlighted, t_clicked, tileSize);
				}
			}
		}

		public void Update(MouseState ms)
		{
			foreach (Tile tile in tiles)
			{
				tile.Update(ms);
			}

		}

		public void Draw(SpriteBatch sb)
		{
			for (int y = 0; y < dimensions.Y; y++)
			{
				for (int x = 0; x < dimensions.X; x++)
				{
					tiles[x, y].Draw(sb);
				}
			}


		}
	}
}
