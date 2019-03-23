using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AHH.UI;
using AHH.Interactable.Building;
namespace AHH.Base
{
	class Tile : InteractableStaticSprite
	{
		Point order { get; }
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
		}

		new public void Update(Cursor ms)
		{
			base.Update(ms);
		}

		public void PlaceBuilding(Building b, Grid grid)
		{
			b.Position = Position;
			b.Box = new Rectangle((int)Position.X, (int)Position.Y, b.Box.Width, b.Box.Height);
			b.CalculateCorners();
			b.InitAdjacent(grid);
		}

		public void Draw_Debug(SpriteBatch sb)
		{
			sb.DrawString(DebugFont, state == TileStates.Active ? "A" : "B", Position, Color.White);


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

	}

}
