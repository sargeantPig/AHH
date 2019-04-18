using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.User
{
	static class Options
	{
		const float tick = 0.5f;
		static float tick_elasped = 0;
		static bool newTick = false;


		static public bool GetTick
		{
			get { return newTick; }
		}

		public static void Update(GameTime gt) //update ticks
		{
			if (tick_elasped >= tick)
			{
				newTick = true;
				tick_elasped = 0;
			}

			else newTick = false;

			tick_elasped += gt.ElapsedGameTime.Milliseconds;
		}
	}
}
