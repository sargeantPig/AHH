using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.UI;
namespace AHH.Base
{
	class InteractableStaticSprite : StaticSprite
	{
		bool isHighlighted { get; set; }
		bool isClicked { get; set; }

		protected Texture2D t_highlighted;
		protected Texture2D t_clicked;

		public InteractableStaticSprite(Vector2 position, Point size, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked)
			: base(position, texture, size)
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
                if (mouse.GetState.LeftButton == ButtonState.Pressed && mouse.GetState != mouse.prevState)
                    isClicked = true;
                else IsClicked = false;
			}
			else { isHighlighted = false; isClicked = false; }

		}

		new public void Draw(SpriteBatch sb)
		{
			sb.Draw(Texture, Box, Color.White);

			if (isHighlighted)
				sb.Draw(t_highlighted, Box, Color.White);
			if (isClicked)
				sb.Draw(t_clicked, Box, Color.White);
		}

		new public InteractableStaticSprite DeepCopy()
		{
			StaticSprite s = base.DeepCopy();
			InteractableStaticSprite iss = (InteractableStaticSprite)this.MemberwiseClone();
			iss.Box = new Rectangle(s.Box.X, s.Box.Y, s.Box.Width, s.Box.Height);
			iss.SetID = s.ID;
			iss.size = new Point(size.X, size.Y);
			return iss;
		}

		public bool IsHighlighted
		{
			get { return isHighlighted; }
		}

		public bool IsClicked
		{
			get { return isClicked; }
			set { isClicked = value; }
		}
	}

}
