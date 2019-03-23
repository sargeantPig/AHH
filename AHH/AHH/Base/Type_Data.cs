using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.Base
{
	struct Type_Data<T>
	{
		T type { get; set; }
		Texture2D texture { get; set; }
		Texture2D h_texture { get; set; }
		Texture2D c_texture { get; set; }
		Texture2D projectile { get; set; }
		Dictionary<string, Vector3> animations { get; set; }

		public Type_Data(Type_Data<T> ut)
		{
			this.type = ut.Type;
			this.texture = ut.texture;
			this.h_texture = ut.h_texture;
			this.c_texture = ut.c_texture;
			this.projectile = ut.projectile;
			this.animations = ut.animations;
		}

		public T Type
		{
			get { return type; }
			set { type = value; }
		}

		public Texture2D Texture
		{
			get { return texture; }
			set { texture = value; }
		}

		public Texture2D H_texture
		{
			get { return h_texture; }
			set { h_texture = value; }
		}
		public Texture2D C_texture
		{
			get { return c_texture; }
			set { c_texture = value; }
		}
		public Texture2D Projectile
		{
			get { return projectile; }
			set { projectile = value; }
		}

		public Dictionary<string, Vector3> Animations
		{
			get { return animations; }
			set { animations = value; }
		}
	}
}
