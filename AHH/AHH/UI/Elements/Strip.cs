using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.UI.Elements
{
    class Strip : BaseElement, IElement
    {
        Dictionary<string, IElement> elements { get; set; }
		Align align;

        public Strip(Vector2 position, bool active, Point size, Align align, Dictionary<string, IElement> elements)
            :base(position, active)
        {
			this.elements = elements;
			this.align = align;
			OrganiseCtrl(size);
        }

        public string GetClicked()
        {
            foreach (KeyValuePair<string, IElement> kv in elements)
            {
                if (kv.Value is Button)
                {
                    if (((Button)kv.Value).IsClicked)
                    {
                        return kv.Key;
                    }
                }
            }

            return null;

        }

		void OrganiseCtrl(Point size)
		{
			int x = 0;
			int y = 0;

			foreach (IElement ctrl in elements.Values)
			{
				ctrl.Position = new Vector2(Position.X + (size.X * x), Position.Y + (size.Y * y));
				if (ctrl is InteractableStaticSprite)
				{
					((InteractableStaticSprite)ctrl).Box = new Rectangle((int)ctrl.Position.X, (int)ctrl.Position.Y, ((InteractableStaticSprite)ctrl).Box.Width, ((InteractableStaticSprite)ctrl).Box.Height);
				}

				if (align == Align.Vertical)
					y++;
				else if (align == Align.Horizontal)
					x++;
			}
		}

		public void Update(Cursor ms)
        {
            foreach (IElement e in elements.Values)
                e.Update(ms);
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (IElement e in elements.Values)
                e.Draw(sb);
        }
    }
}
