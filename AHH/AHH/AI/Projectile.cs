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

        Vector2 destination;

        Vector2 origin;
		public Projectile(Vector2 position, Point size, Texture2D texture, float speed, Vector2 destination)
			: base(position, size, texture, speed)
		{
			alive = true;
            this.destination = destination;

            var dir = position.DirectionTo(destination);
            rotation = (float)Math.Atan2(dir.Y, dir.X);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

		public void Update()
		{
			if (MoveTo(destination))
				alive = false;

			
		}

		public bool Alive
		{
			get { return alive; }
			set { alive = value; }
		}

		new public void Draw(SpriteBatch sb)
		{
			sb.Draw(Texture, Box, null, Color.White, rotation, origin, SpriteEffects.None, 1f);
		}
	}
}
