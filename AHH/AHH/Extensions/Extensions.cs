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
		public static float PercentAofB(this float a, float b)
		{
			return (a / b) * 100;
		}
		public static float PercentDecrease(this float current, float original)
		{
			float decrease = original - current;
			return (decrease / original) * 100;
		}

		public static T RandomFlag<T>(Random rnd)
		{
			Array flags = Enum.GetValues(typeof(T));
			var a = (T)flags.GetValue(rnd.Next(flags.Length));

			return a;
		}

		public static T RandomFlag<T>(Random rnd, int min, int max)
		{
			Array flags = Enum.GetValues(typeof(T));
			var a = (T)flags.GetValue(rnd.Next(min, max));

			return a;
		}

	}



	public class WTuple<t1, t2, t3>
	{
		t1 Item_1 { get; set; }
		t2 Item_2 { get; set; }
		t3 Item_3 { get; set; }

		public WTuple(t1 Item_1, t2 Item_2, t3 Item_3)
		{
			this.Item_1 = Item_1;
			this.Item_2 = Item_2;
			this.Item_3 = Item_3;

		}

		public t1 Item1
		{
			get { return Item_1; }
			set { Item_1 = value; }
		}

		public t2 Item2
		{
			get { return Item_2; }
			set { Item_2 = value; }
		}

		public t3 Item3
		{
			get { return Item_3; }
			set { Item_3 = value; }
		}



	}
}
