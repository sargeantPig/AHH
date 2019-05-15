using AHH.Extensions;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AHH.Base;
namespace AHH.UI
{
	class Cursor : AnimatedSprite
	{
		Vector2 real_position { get; set; }
		MouseState ms { get; set; }
		MouseState prev_ms { get; set; }
		public Cursor(Texture2D texture, Dictionary<string, Vector3> states) : base(Vector2.One, new Point(16,16), texture, states, false, 32)
		{
		}

		public Point Update(MouseState ms, GameTime gt)
		{
            base.Update(gt);
			this.ms = ms;
			real_position = new Vector2(ms.Position.X, ms.Position.Y);
            
			Matrix mat = Resolution.getTransformationMatrix();

			if (mat.Determinant() > 1f)
				real_position = Vector2.Transform(real_position, mat);
			else real_position = Vector2.Transform(real_position, Matrix.Invert(mat));

            Position = real_position;
            return real_position.ToPoint();
		}

		public Vector2 GetRealPosition
		{
			get { return real_position; }

		}

		public bool isRightPressed
		{
			get
			{
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

		public MouseState prevState
		{
			get { return prev_ms; }
			set { prev_ms = value; }
		}

	}
}
