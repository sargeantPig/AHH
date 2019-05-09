using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.UI;
using AHH.UI.Elements;
namespace AHH.Base
{
	class BaseObject
	{
		static SpriteFont debugFont { get; set; }
		Guid id { get; set; }
		Vector2 position { get; set; }

		InfoPanel info { get; set; }

		public BaseObject() { this.id = Guid.NewGuid(); }


		public BaseObject(Vector2 position) { this.id = Guid.NewGuid(); this.position = position; }

		static public SpriteFont DebugFont
		{
			set { debugFont = value; }
			get { return debugFont; }
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

		public InfoPanel Info
		{
			get { return info; }
			set { info = value; }
		}

		public BaseObject DeepCopy()
		{
			BaseObject b = (BaseObject)this.MemberwiseClone();
			b.id = new Guid();
			b.position = new Vector2(position.X, position.Y);
			return b;
		}
	}

}
