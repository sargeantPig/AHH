using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.Extensions;
using AHH.UI;
namespace AHH.Base
{
	class BaseObject
	{
		static SpriteFont debugFont { get; set; }
		Guid id { get; set; }
		Vector2 position { get; set; }

		public BaseObject() { this.id = new Guid(); }

		public BaseObject(Vector2 position) { this.id = new Guid(); this.position = position; }

		static public SpriteFont DebugFont
		{
			set { debugFont = value; }
			get { return debugFont;  }
		}

		public Guid ID
		{
			get { return id; }
		}

        protected Guid SetID
        {
            set { id = value; }
        }

		public Vector2 Position
		{
			set { position = value; }
			get { return position; }
		}

        public BaseObject DeepCopy()
        {
            BaseObject b = (BaseObject)this.MemberwiseClone();
            b.id = new Guid();
            b.position = new Vector2(position.X, position.Y);
            return b;
        }
	}

	class StaticSprite : BaseObject
	{
		Texture2D texture { get; set; }
		Rectangle box { get; set; }

        protected Point size;

		public StaticSprite(Vector2 position, Texture2D texture, Point size) 
			: base(position) {
            this.size = size;
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

        public Point GetSize()
        {
            return size;
        }

        new public StaticSprite DeepCopy()
        {
            BaseObject b = base.DeepCopy();
            StaticSprite s = (StaticSprite)this.MemberwiseClone();
            s.Box = new Rectangle((int)b.Position.X, (int)b.Position.Y, size.X, size.Y);
            s.size = new Point(size.X, size.Y);
            s.SetID = b.ID;
            s.Position = new Vector2(b.Position.X, b.Position.Y);
            return s;
        }
	}

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
			if (states == null)
			{
				this.States = new Dictionary<string, Vector3>();
				this.States.Add("default", new Vector3(0f, (texture.Width / Source.Width), 100));//sets a default state of going through the entire sheet
			}
			else
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
		float speed { get; set; }

		public MovingSprite(Vector2 position, Point RectExtents, Texture2D texture, float speed, Dictionary<string, Vector3> states = null, bool active_mode = false)
			: base(position, RectExtents, texture, states, active_mode)
		{
			this.speed = speed;
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

		public float Speed
		{
			get { return speed; }
			set { speed = value; }
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

		public InteractableMovingSprite(Vector2 position, Point RectExtents, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, float speed, Dictionary<string, Vector3> states = null, bool active_mode = false)
			: base(position, RectExtents, texture, speed, states, active_mode)
		{
			isHighlighted = false;
			isClicked = false;

			this.t_highlighted = t_highlighted;
			this.t_clicked = t_clicked;
		}

		public void Update(Cursor mouse)
		{
			if (Box.Contains(mouse.GetRealPosition))
			{
				isHighlighted = true;
				if (mouse.GetState.LeftButton == ButtonState.Pressed)
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
		bool isHighlighted { get; set; }
		bool isClicked { get; set; }

		protected Texture2D t_highlighted;
		protected Texture2D t_clicked;

		public InteractableStaticSprite(Vector2 position, Point size, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked)
			: base(position, texture, size)
		{
			isHighlighted = false;
			isClicked = false;

			this.t_highlighted = t_highlighted;
			this.t_clicked = t_clicked;
		}

		public void Update(Cursor mouse)
		{
			if (Box.Contains(mouse.GetRealPosition))
			{
				isHighlighted = true;
				if (mouse.GetState.LeftButton == ButtonState.Pressed)
					isClicked = true;
			}
			else { isHighlighted = false; isClicked = false; }


		}

		new public void Draw(SpriteBatch sb)
		{
			sb.Draw(Texture, Box, Color.White);

			if (isHighlighted)
				sb.Draw(t_highlighted, Box, Color.White);
			if (isClicked)
				sb.Draw(t_clicked, Box, Color.White); 
		}

        new public InteractableStaticSprite DeepCopy()
        {
            StaticSprite s = base.DeepCopy();
            InteractableStaticSprite iss = (InteractableStaticSprite)this.MemberwiseClone();
            iss.Box = new Rectangle(s.Box.X, s.Box.Y, s.Box.Width, s.Box.Height);
            iss.SetID = s.ID;
            iss.size = new Point(size.X, size.Y);
            return iss;
        }

		public bool IsHighlighted
		{
			get { return isHighlighted; }
		}

		public bool IsClicked
		{
			get { return isClicked; }
			set { isClicked = value; }
		}
	}

}
