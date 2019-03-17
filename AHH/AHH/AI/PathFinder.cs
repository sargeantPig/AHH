using AHH.Base;
using Microsoft.Xna.Framework;
using System.Threading;

namespace AHH.AI
{
	class Pathfinder
	{
		ThreadStart th_pathfinder { get; set; }
		Thread th_pathchild { get; set; }

		public Pathfinder(Grid grid, Vector2 position)
		{

		}

		public ThreadStart Th_Pathfinder
		{
			get { return th_pathfinder; }
			set { th_pathfinder = value; }
		}

		public Thread Th_PathChild
		{
			get { return th_pathchild; }
			set { th_pathchild = value; }
		}

	}
}
