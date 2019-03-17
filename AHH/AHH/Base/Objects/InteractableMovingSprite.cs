using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.UI;
namespace AHH.Base
{
	class InteractableMovingSprite : MovingSprite
	{
		bool isHighlighted;
		bool isClicked;

		Texture2D t_highlighted;
		Texture2D t_clicked;

		public InteractableMovingSprite(Vector2 position, Point RectExtents, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, float speed, Dictionary<string, Vector3> states = null, bool active_mode = false)
			: base(position, RectExtents, texture, speed, states, active_mode)
		{
			isHighlighted = false;
			isClicked = false;

			this.t_highlighted = t_highlighted;
			this.t_clicked = t_clicked;
		}

		public void Update(Cursor mouse)
		{
			if (Box.Contains(mouse.GetRealPosition))
			{
				isHighlighted = true;
				if (mouse.GetState.LeftButton == ButtonState.Pressed)
					isClicked = true;
			}
			else { isHighlighted = false; isClicked = false; }


		}

		new public void Draw(SpriteBatch sb)
		{
			sb.Draw(Texture, Position, TextureSource, Color.White);

			if (isHighlighted)
				sb.Draw(t_highlighted, Position, TextureSource, Color.White);
			if (isClicked)
				sb.Draw(t_clicked, Position, TextureSource, Color.White);
		}
	}
}
