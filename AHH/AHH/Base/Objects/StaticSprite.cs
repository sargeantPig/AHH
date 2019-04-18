using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.UI;
namespace AHH.Base
{
	class StaticSprite : BaseObject
	{
		Texture2D texture { get; set; }
		Rectangle box { get; set; }

		protected Point size;

		public StaticSprite(Vector2 position, Texture2D texture, Point size)
			: base(position)
		{
			this.size = size;
			this.texture = texture;
			this.box = new Rectangle((int)position.X, (int)position.Y, size.X, size.Y);
		}

		public Texture2D Texture
		{
			get { return texture; }
			set { texture = value; }
		}

		public Rectangle Box
		{
			get { return box; }
			set { box = value; }
		}


		public void Draw(SpriteBatch sb)
		{
			sb.Draw(texture, base.Position, Color.White);
		}

		public Point GetSize()
		{
			return size;
		}

		public Vector2 Center
		{
			get { return new Vector2(Position.X + (size.X / 2), Position.Y + (size.Y / 2)); }
		}

		new public StaticSprite DeepCopy()
		{
			BaseObject b = base.DeepCopy();
			StaticSprite s = (StaticSprite)this.MemberwiseClone();
			s.Box = new Rectangle((int)b.Position.X, (int)b.Position.Y, size.X, size.Y);
			s.size = new Point(size.X, size.Y);
			s.SetID = b.ID;
			s.Position = new Vector2(b.Position.X, b.Position.Y);
			return s;
		}


	}

}
