using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AHH.Interactable;
using AHH.UI;

namespace AHH.Base
{
	class Tile : InteractableStaticSprite
	{
		Point order { get; }
		Building building { get; set; }
		TileStates state { get; set; }
		Point parent { get; set; }
		List<Point> children { get; set; }


		public Tile(Vector2 position, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, Point size, Point order)
			: base(position, size, texture, t_highlighted, t_clicked)
		{
			children = new List<Point>();
			this.order = order;
			parent = order;
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

		public void AddChild(Point child)
		{
			children.Add(child);
		}

		public void ClearChildren()
		{
			children.Clear();
		}

		public void EndChildhood()
		{
			parent = order;
		}

		public List<Point> GetChildren
		{
			get { return children; }
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

		public Point Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public void RemoveBuilding()
		{
			building = null;
		}

	}

}
