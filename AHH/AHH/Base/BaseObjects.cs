using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.Extensions;

namespace AHH.Base
{
	class BaseObject
	{
		static SpriteFont debugFont { get; set; }
		Guid id { get; }
		Vector2 position { get; set; }

		public BaseObject() { this.id = new Guid(); }

		public BaseObject(Vector2 position) { this.id = new Guid(); this.position = position; }

		public SpriteFont DebugFont
		{
			set { debugFont = value; }
			get { return debugFont;  }
		}

		public Guid ID
		{
			get { return id; }
		}

		public Vector2 Position
		{
			set { position = value; }
			get { return position; }
		}
	}

	class StaticSprite : BaseObject
	{
		Texture2D texture { get; set; }
		Rectangle box { get; set; }

		public StaticSprite(Vector2 position, Texture2D texture, Point size) 
			: base(position) {
			this.texture = texture;
			this.box = new Rectangle((int)position.X, (int)position.Y, size.X, size.Y);
		}

		public Texture2D Texture
		{
			get { return texture; }
			set { texture = value;  }
		}

		public Rectangle Box
		{
			get { return box; }
			set { box = value; }
		}


		public void Draw(SpriteBatch sb)
		{
			sb.Draw(texture, base.Position, Color.White);
		}
	}

	class AnimatedSprite : StaticSprite
	{
		private Rectangle Source { get; set; }
		private int Frame { get; set; }
		private float FrameTime { get; set; }

		private float elasped = 0;

		Dictionary<string, Vector3> States { get; set; }

		string currentState { get; set; }

		bool Active { get; set; }
		bool active_mode = false;

		public AnimatedSprite(Vector2 position, Point RectExtents, Texture2D texture, float frameTime, Dictionary<string, Vector3> states = null, bool active_mode = false)
			: base(position, texture, RectExtents)
		{
			this.Source = new Rectangle(0, 0, RectExtents.X, RectExtents.Y);
			this.FrameTime = frameTime;
			this.Active = false;
			this.active_mode = active_mode;
			if (states == null)
			{
				this.States = new Dictionary<string, Vector3>();
				this.States.Add("default", new Vector3(0f, (texture.Width / Source.Width), frameTime));//sets a default state of going through the entire sheet
			}
			else
			{
				this.States = new Dictionary<string, Vector3>(states);
				this.States.Add("default", new Vector3(0f, (texture.Width / Source.Width), frameTime));//sets a default state of going through the entire sheet

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

	class MovingSprite : AnimatedSprite
	{
		Vector2 Velocity { get; }
		float Speed { get; }

		public MovingSprite(Vector2 position, Point RectExtents, Texture2D texture, float frameTime, float speed, Dictionary<string, Vector3> states = null, bool active_mode = false)
			: base(position, RectExtents, texture, frameTime, states, active_mode)
		{
			this.Speed = speed;
			this.Velocity = new Vector2(1, 1);
		}

		public bool MoveTo(Vector2 destination)
		{
			float distance = Vector2.Distance(Position, destination);
			Vector2 direction = Extensions.Extensions.DirectionTo(Position, destination);
			if (Position != destination)
			{
			
				Position += direction * (Speed * Velocity);
				//Console.WriteLine(move);
				Console.WriteLine(Position);

				if (distance < 2)
				{
					Position = destination;
				}

				UpdateBox = Position;
				return false;
			}



			return true;
		}

		public Vector2 UpdateBox
		{
			set { Box = new Rectangle((int)value.X, (int)value.Y, Box.X, Box.Y);  }
		}

		public Vector2 GetVelocity
		{
			get { return Velocity; }
		}
	}

	class InteractableMovingSprite : MovingSprite
	{
		bool isHighlighted;
		bool isClicked;

		Texture2D t_highlighted;
		Texture2D t_clicked;

		public InteractableMovingSprite(Vector2 position, Point RectExtents, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, float frameTime, float speed, Dictionary<string, Vector3> states = null, bool active_mode = false)
			: base(position, RectExtents, texture, frameTime, speed, states, active_mode)
		{
			isHighlighted = false;
			isClicked = false;

			this.t_highlighted = t_highlighted;
			this.t_clicked = t_clicked;
		}

		public void Update(MouseState mouse)
		{
			if (Box.Contains(mouse.Position))
			{
				isHighlighted = true;
				if (mouse.LeftButton == ButtonState.Pressed)
					isClicked = true;
			}
			else { isHighlighted = false; isClicked = false; } 

			
		}

		new public void Draw(SpriteBatch sb)
		{
			sb.Draw(Texture, Position, TextureSource, Color.White);

			if (isHighlighted)
				sb.Draw(t_highlighted, Position, TextureSource, Color.White);
			if (isClicked)
				sb.Draw(t_clicked, Position, TextureSource, Color.White);
		}
	}

	class InteractableStaticSprite : StaticSprite
	{
		bool isHighlighted;
		bool isClicked;

		Texture2D t_highlighted;
		Texture2D t_clicked;

		public InteractableStaticSprite(Vector2 position, Point size, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked)
			: base(position, texture, size)
		{
			isHighlighted = false;
			isClicked = false;

			this.t_highlighted = t_highlighted;
			this.t_clicked = t_clicked;
		}

		public void Update(MouseState mouse)
		{
			if (Box.Contains(mouse.Position))
			{
				isHighlighted = true;
				if (mouse.LeftButton == ButtonState.Pressed)
					isClicked = true;
			}
			else { isHighlighted = false; isClicked = false; }


		}

		new public void Draw(SpriteBatch sb)
		{
			sb.Draw(Texture, Position, Box, Color.White);

			if (isHighlighted)
				sb.Draw(t_highlighted, Position, Box, Color.White);
			if (isClicked)
				sb.Draw(t_clicked, Position, Box, Color.White); 
		}


	}

}
