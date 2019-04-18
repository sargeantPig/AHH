using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AHH.Extensions;
namespace AHH.AI
{
	class Projectile : MovingSprite
	{
		bool alive { get; set; }
		float rotation = 1;
		public Projectile(Vector2 position, Point size, Texture2D texture, float speed)
			: base(position, size, texture, speed)
		{
			alive = true;
		}

		public void Update(Vector2 destination, AiUnit unit)
		{
			if (MoveTo(destination))
				alive = false;

			var dir = Position.DirectionTo(destination);

			rotation = -(float)Math.Atan2(dir.Y, dir.X);;
		}

		public bool Alive
		{
			get { return alive; }
			set { alive = value; }
		}

		new public void Draw(SpriteBatch sb)
		{
			sb.Draw(Texture, Box, null, Color.White, rotation, new Vector2(8f, 8f), SpriteEffects.None, 1f);
		}
	}
}
