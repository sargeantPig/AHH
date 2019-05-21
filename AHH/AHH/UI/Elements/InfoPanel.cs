using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.UI.Elements
{
	class InfoPanel : BaseObject, IElement
	{
		Texture2D picture { get; set; }
	 	Dictionary<Text, Text> details { get; set; }

		Text fullString;

		bool isActive = true;
		public InfoPanel(InfoPanel infoPanel)
			: base(infoPanel.Position)
		{
			this.picture = infoPanel.picture;
			this.details = infoPanel.details;
			Refresh();
		}

		public InfoPanel(Dictionary<Text, Text> details, Texture2D picture, Vector2 position)
			: base(position)
		{
			this.details = details;
			this.picture = picture;
			Refresh();
		}

		public void Update(Cursor cursor)
		{ }

		public void Refresh()
		{
			fullString = new Text(Position + new Vector2(90, 0));
			fullString.Colour = Color.White;
			fullString.Value = "";
		}

		public void Draw(SpriteBatch sb)
		{
            if(picture != null)
			    sb.Draw(picture, new Rectangle((int)Position.X, (int)Position.Y, 64, 64), new Rectangle(0,0, 64, 64), Color.White);

			fullString.Draw(sb, fullString.Position);

            int i = 0;
            Text prevkey;
            foreach (var deets in details)
            {
                prevkey = deets.Key;
                deets.Key.Draw(sb, fullString.Position + new Vector2(0, (i * DebugFont.MeasureString(prevkey.Value).Y)));
                deets.Value.Draw(sb, fullString.Position + new Vector2(DebugFont.MeasureString(deets.Key.Value).X, (i * DebugFont.MeasureString(prevkey.Value).Y)));
                i++;
            }

		}


		public Dictionary<Text, Text> Details
		{
			get { return details; }
			set { details = value; }
		}

		bool IElement.IsActive {
			get { return isActive; }
			set { isActive = value; }
		}

        public Texture2D Picture
        {
            get { return picture; }
            set { picture = value; }
        }
	}
}
