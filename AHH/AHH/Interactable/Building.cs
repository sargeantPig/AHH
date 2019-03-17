using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.Base;
using AHH.UI;
namespace AHH.Interactable
{
	class Building : InteractableStaticSprite
	{
        int cost;


		public Building(Vector2 position, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, Point size)
			: base (position, size, texture, t_highlighted, t_clicked)
		{

		}

		new public void Update(Cursor ms)
		{
			base.Update(ms);
		}

        new public Building DeepCopy()
        {
            InteractableStaticSprite iss = base.DeepCopy();
            Building b = (Building)this.MemberwiseClone();
            b.Box = new Rectangle((int)Position.X, (int)Position.Y, iss.Box.Width, iss.Box.Height);
            b.cost = cost;
            b.SetID = iss.ID;
            b.Position = new Vector2(Position.X, Position.Y);
            return b;

        }

	}
}
