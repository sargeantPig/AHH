using System.Collections.Generic;
using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace AHH.UI
{
	class DropMenu : BaseObject, IElement
	{
		Dictionary<string, IElement> controls;
		KeyValuePair<string, IElement> clickedControl { get; set; }
		Point elementSize;
		public DropMenu(Vector2 position, Dictionary<string, IElement> controls, Point elementSize)
			: base(position)
		{
			this.elementSize = elementSize;
			this.controls = new Dictionary<string, IElement>(controls);
			this.clickedControl = new KeyValuePair<string, IElement>();
			OrganiseCtrl(elementSize);
		}

		public void Update(Cursor cursor)
		{
			foreach (KeyValuePair<string, IElement> kv in controls)
			{
				kv.Value.Update(cursor);
				if (kv.Value is InteractableStaticSprite)
				{
					if (((InteractableStaticSprite)kv.Value).IsClicked)
					{
						clickedControl = kv;
					}
				}
			}

		}

		public void Draw(SpriteBatch sb)
		{
			foreach (IElement ctrl in controls.Values)
			{
				ctrl.Draw(sb);
			}

		}

		public string ClickedCtrl()
		{
			foreach (KeyValuePair<string, IElement> kv in controls)
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

		public void Reset(Cursor cursor)
		{
			Position = cursor.GetRealPosition;
			OrganiseCtrl(elementSize);
		}

		void OrganiseCtrl(Point size)
		{
			int x = 0;
			int y = 0;

			foreach (IElement ctrl in controls.Values)
			{
				ctrl.Position = new Vector2(Position.X + (size.X * x), Position.Y + (size.Y * y));
				if (ctrl is InteractableStaticSprite)
				{
					((InteractableStaticSprite)ctrl).Box = new Rectangle((int)ctrl.Position.X, (int)ctrl.Position.Y, ((InteractableStaticSprite)ctrl).Box.Width, ((InteractableStaticSprite)ctrl).Box.Height);
				}
				y++;
			}
		}

		public string Action
		{
			get { return clickedControl.Key; }
		}

	}

}
