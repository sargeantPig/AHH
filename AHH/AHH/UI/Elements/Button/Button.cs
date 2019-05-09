using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AHH.UI
{
	class Button : InteractableStaticSprite, IElement
	{
		Text text;
		bool isActive { get; set; }
		public Button(Vector2 position, Point size, bool active, Texture2D texture, Texture2D texture_h, Texture2D texture_c, string text)
			: base(position, size, texture, texture_h, texture_c)
		{
			this.text = new Text(new Vector2(Box.X + (Box.Width / 2), Box.Y + (Box.Width / 2)), text, Color.White);
			active = isActive;
		}

		new public void Draw(SpriteBatch sb)
		{
			base.Draw(sb);
			text.Draw(sb, new Vector2(Box.X + (Box.Width / 2) - (DebugFont.MeasureString(text.Value).X / 2), Box.Y + (Box.Height / 2) - (DebugFont.MeasureString(text.Value).Y / 2)));
		}

		public bool IsActive
		{
			get { return isActive; }
			set { isActive = value; }
		}

        public void Manipulate(string value)
        {
            text.Value = value;
        }
	}
}
