﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.Base;

namespace AHH.Interactable
{
	class Building : InteractableStaticSprite
	{

		public Building(Vector2 position, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, Point size)
			: base (position, size, texture, t_highlighted, t_clicked)
		{

		}

	}
}