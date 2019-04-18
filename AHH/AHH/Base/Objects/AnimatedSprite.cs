using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.UI;
namespace AHH.Base
{
	class AnimatedSprite : StaticSprite
	{
		private Rectangle Source { get; set; }
		private int Frame { get; set; }

		private float elasped = 0;

		Dictionary<string, Vector3> States { get; set; }

		string currentState { get; set; }

		bool Active { get; set; }
		bool active_mode = false;

		public AnimatedSprite(Vector2 position, Point RectExtents, Texture2D texture, Dictionary<string, Vector3> states = null, bool active_mode = false)
			: base(position, texture, RectExtents)
		{
			this.Source = new Rectangle(0, 0, RectExtents.X, RectExtents.Y);
			this.Active = false;
			this.active_mode = active_mode;
			if (states == null && texture != null)
			{
				this.States = new Dictionary<string, Vector3>();
				this.States.Add("default", new Vector3(0f, (texture.Width / Source.Width), 100));//sets a default state of going through the entire sheet
			}
			else if (states != null)
			{
				this.States = new Dictionary<string, Vector3>(states);
				this.States.Add("default", new Vector3(0f, (texture.Width / Source.Width), 100));//sets a default state of going through the entire sheet

			}
			currentState = "default";

			Frame = 0;


		}

		virtual public void Update(GameTime gt)
		{
			elasped += gt.ElapsedGameTime.Milliseconds;

			if (elasped > States[currentState].Z)
			{

				if (Frame >= States[currentState].Y)
				{
					if (active_mode)
						Active = false;

					Frame = (int)States[currentState].X;
				}
				else Frame++;

				elasped = 0;
				FrameChange();
			}
		}

		new public void Draw(SpriteBatch sb)
		{
			sb.Draw(base.Texture, base.Position, Source, Color.White);
		}

		public void Draw_Debug(SpriteBatch sb)
		{
			sb.Draw(Texture, Box, Color.Red);

		}

		public void AddState(string key, Vector3 newState)
		{
			States.Add(key, newState);
		}

		public bool IsActive
		{
			get { return Active; }
			set
			{
				Frame = (int)States[currentState].X;
				Active = value;

			}
		}

		public Dictionary<string, Vector3> AnimationStates
		{
			get { return States; }
			set { States = value; }
		}

		public string CurrentState
		{
			get { return currentState; }
			set { currentState = value; }

		}

		public int CurrentFrame
		{
			get { return Frame; }
			set { Frame = value; }
		}

		private void FrameChange()
		{
			Source = new Rectangle(Frame * Source.Width, 0, Source.Width, Source.Height);
		}

		public Rectangle TextureSource
		{
			get { return Source; }
			set { Source = value; }
		}

	}

}
