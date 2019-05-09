using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace AHH.UI
{
	class Text : BaseObject, IElement 
	{
		string value { get; set; }
		Color colour { get; set; }

		public Text(Vector2 position, string value = null, Color colour = new Color())
			: base(position)
		{
            if (colour == new Color())
            {
                this.colour = colour;
            }
			else this.colour = colour;
			this.value = value;
		}

		public void Draw(SpriteBatch sb)
		{
			sb.DrawString(DebugFont, value, Position, colour);
		}

		public void Draw(SpriteBatch sb, Vector2 position)
		{
			sb.DrawString(DebugFont, value, position, colour);
		}

        void IElement.Draw(SpriteBatch sb)
        {
            throw new System.NotImplementedException();
        }

        void IElement.Update(Cursor ms)
        {
            throw new System.NotImplementedException();
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

        new public Vector2 Position
        {
            set { base.Position = value - (DebugFont.MeasureString(this.value) / 2); }
            get { return base.Position; }
        }

        Vector2 IElement.Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        bool IElement.IsActive { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }

}
