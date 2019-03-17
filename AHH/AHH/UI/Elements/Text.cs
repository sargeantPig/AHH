using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace AHH.UI
{
	class Text : BaseObject
	{
		string value { get; set; }
		Color colour { get; set; }

		public Text(Vector2 position, string value = null, Color colour = new Color())
			: base(position)
		{
			this.colour = colour;
			this.value = value;
		}

		public void Draw(SpriteBatch sb)
		{
			sb.DrawString(DebugFont, value, Position, Color.Black);
		}

		public void Draw(SpriteBatch sb, Vector2 position)
		{
			sb.DrawString(DebugFont, value, position, Color.Black);
		}

		public string Value
		{
			get { return value; }
			set { this.value = value; }
		}

		public Color Colour
		{
			get { return colour; }
			set { colour = value; }
		}

	}

}
