using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Extensions;
namespace AHH.UI.Elements
{
	class StatusBar: IElement 
	{
		Texture2D bottom;
		Texture2D top;
		Point b_size;
		Point t_size;

		public Vector2 Position { get; set; }
		public bool IsActive { get; set; }

		int currentValue;
		int maxValue;

		Text text;
		public StatusBar(Point size, int maxValue, Texture2D[] textures, Text text = null)
		{
			this.b_size = size;
			t_size = b_size;
			this.maxValue = maxValue;
			currentValue = maxValue;
			bottom = textures[0];
			top = textures[1];
			if(text != null)
				this.text = new Text(Position + new Vector2(bottom.Width /2, top.Height /2), text.Value, text.Colour);
		}

		public void Update(float value)
		{
			MathHelper.Clamp(value, 0, maxValue);

			currentValue = (int)value.PercentAofB(maxValue);


			if (currentValue == 100)
			{
				currentValue = t_size.X;
				if (text != null)
				{
					text.Value = value.ToString() + "/" + maxValue.ToString();
					text.Position = Position + new Vector2(bottom.Width / 2, top.Height / 2);
				}
				return;
			}

			currentValue = 100 - currentValue;
			if (currentValue > 0)
			{
				float temp = (t_size.X * ((float)currentValue/100));
				currentValue = t_size.X - (int)temp;
			}
			else currentValue = 0;

			if (text != null)
			{
				text.Value = value.ToString() + "/" + maxValue.ToString();
				text.Position = Position + new Vector2(bottom.Width / 2, top.Height / 2);
			}
		}

		public void UpdatePosition(Vector2 position)
		{
			Position = position;
		}

		public void Draw(SpriteBatch sb, Texture2D[] textures = null)
		{

			if (textures == null && bottom != null && top != null)
			{
				sb.Draw(bottom, new Rectangle(Position.ToPoint(), b_size), Color.White);
				sb.Draw(top, new Rectangle(Position.ToPoint(), new Point(currentValue, t_size.Y)), new Rectangle(0, 0, t_size.X, t_size.Y), Color.White);
			}

			if (textures != null)
			{
				sb.Draw(textures[0], new Rectangle(Position.ToPoint(), b_size), Color.White);
				sb.Draw(textures[1], new Rectangle(Position.ToPoint(), new Point(currentValue)), new Rectangle(0, 0, t_size.X, t_size.Y), Color.White);
				
			}

			if (text != null)
				text.Draw(sb);
		}

		public void Draw(SpriteBatch sb)
		{ }

		public void Update(Cursor ms)
		{
			throw new NotImplementedException();
		}

	}
}
