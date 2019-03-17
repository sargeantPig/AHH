﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.UI;
namespace AHH.Base
{
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
			set { Box = new Rectangle((int)value.X, (int)value.Y, Box.X, Box.Y); }
		}

		public Vector2 GetVelocity
		{
			get { return Velocity; }
		}
	}
}
