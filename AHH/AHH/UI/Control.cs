using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Base;
using AHH.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace AHH.UI
{
	enum ButtonFunction
	{
		Build,
		Examine,
		Dismantle
	}

	interface Control
	{
		void Draw(SpriteBatch sb);
		void Update(Cursor ms);
		Vector2 Position { get; set; }
	}

	class DropMenu : BaseObject, Control
	{
		Dictionary<string, Control> controls;
		KeyValuePair<string, Control> clickedControl { get; set; }
		Point elementSize;
		public DropMenu(Vector2 position, Dictionary<string, Control> controls, Point elementSize)
			: base(position)
		{
			this.elementSize = elementSize;
			this.controls = new Dictionary<string, Control>(controls);
			this.clickedControl = new KeyValuePair<string, Control>();
			OrganiseCtrl(elementSize);
		}

		public void Update(Cursor cursor)
		{
			foreach (KeyValuePair<string, Control> kv in controls)
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
			foreach (Control ctrl in controls.Values)
			{
				ctrl.Draw(sb);
			}

		}

		public string ClickedCtrl()
		{
			foreach (KeyValuePair<string, Control> kv in controls)
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

			foreach (Control ctrl in controls.Values)
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
			get { return clickedControl.Key;}
		}
		
	}

	class Button : InteractableStaticSprite, Control
	{
		Text text;

		public Button(Vector2 position, Point size, Texture2D texture, Texture2D texture_h, Texture2D texture_c, string text)
			:base(position, size, texture, texture_h, texture_c)
		{
			this.text = new Text(new Vector2(Box.X + (Box.Width / 2), Box.Y + (Box.Width / 2)), text, Color.White);
		}

		new public void Draw(SpriteBatch sb)
		{
			base.Draw(sb);
			text.Draw(sb, new Vector2(Box.X + (Box.Width / 2) - (DebugFont.MeasureString(text.Value).X/2), Box.Y + (Box.Height / 2) - (DebugFont.MeasureString(text.Value).Y/2)));
		}
	}

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

	class Cursor
	{
		Texture2D texture;
		Vector2 real_position { get; set; }
		MouseState ms { get; set; }

		public Cursor(Texture2D texture)
		{
			this.texture = texture;
		}

		public Point Update(MouseState ms)
		{
			this.ms = ms;
			real_position = new Vector2(ms.Position.X, ms.Position.Y);
			Matrix mat = Resolution.getTransformationMatrix();

			if (mat.Determinant() > 1f)
				real_position = Vector2.Transform(real_position, mat);
			else real_position = Vector2.Transform(real_position, Matrix.Invert(mat));

			return real_position.ToPoint();
		}

		public void Draw(SpriteBatch sb)
		{
			sb.Draw(texture, new Rectangle((int)real_position.X, (int)real_position.Y, 16, 16), Color.White);
		}

		public Vector2 GetRealPosition
		{
			get { return real_position; }

		}

		public bool isRightPressed
		{
			get {
				if (ms.RightButton == ButtonState.Pressed)
					return true;
				else return false;
			}
		}

		public bool isLeftPressed
		{
			get
			{
				if (ms.LeftButton == ButtonState.Pressed)
					return true;
				else return false;
			}
		}

		public MouseState GetState
		{
			get { return ms; }
		}

	}
}
