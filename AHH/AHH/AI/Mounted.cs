using System.Collections.Generic;
using AHH.Base;
using AHH.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AHH.AI
{
	class Mounted : AiUnit
	{

		public Mounted(Vector2 position, Point rectExtends, float speed, Dictionary<string, Vector3> states, Stats stats, Unit_Types types, Grid grid)
			: base(position, rectExtends, speed, states, stats, types, grid)
		{

		}

		new public void Update(Cursor ms)
		{

		}

		new public void Draw(SpriteBatch sb)
		{


		}

	}
}
