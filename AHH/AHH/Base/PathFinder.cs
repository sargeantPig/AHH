using AHH.Base;
using Microsoft.Xna.Framework;
using System.Threading;

namespace AHH.Base
{
	class OffloadThread
	{
		ThreadStart th_offload { get; set; }
		Thread th_child { get; set; }

	
		public OffloadThread()
		{

		}

		public ThreadStart Th_Offload
		{
			get { return th_offload; }
			set { th_offload = value; }
		}

		public Thread Th_Child
		{
			get { return th_child; }
			set { th_child = value; }
		}

	}
}
