using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AHH.Extensions
{
	public static class Extensions
	{
		public static Vector2 DirectionTo(this Vector2 from, Vector2 to)
		{
			Vector2 direction = from - to;
			direction.Normalize();
			return -direction;
		}


	}
}
