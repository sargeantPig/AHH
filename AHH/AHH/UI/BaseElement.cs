using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Base;
using Microsoft.Xna.Framework;

namespace AHH.UI
{
	class BaseElement : BaseObject
	{
		bool isActive { get; set; }

		public BaseElement(Vector2 position, bool active)
			: base(position)
		{ isActive = active; }

		public bool IsActive
		{
			get { return isActive; }
			set { isActive = value; }
		}

	}
}
