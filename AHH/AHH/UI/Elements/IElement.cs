using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace AHH.UI
{
	interface IElement
	{
		void Draw(SpriteBatch sb);
		void Update(Cursor ms);
		Vector2 Position { get; set; }
	}
}
