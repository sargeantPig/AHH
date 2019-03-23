using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using AHH.Base;
using AHH.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AHH.User
{
    public enum Player_Modes
    {
        Building,
        Research,
        Spells
    }

    class Player : BaseObject
    {
        ControlMapper controls { get; }
        string buildingID { get; set; } //currently selected building to place
		Player_Modes mode { get; set; }
		Cursor cursor { get; set; }


#if DEBUG
        Vector2 rect_dimensions = new Vector2(10, 10);
#endif

        public Player(Texture2D cursor)
            : base()
        {
#if DEBUG
            buildingID = "Nan";
#endif
			mode = Player_Modes.Building;
            controls = new ControlMapper("Content/settings/controls.txt");
			this.cursor = new Cursor(cursor);
        }

        public void Update(UiMaster master, MouseState ms )
        {
			cursor.Update(ms);
			if (controls.IsPressed(Ctrls.HotKey_Build))
				mode = Player_Modes.Building;
			if (controls.IsPressed(Ctrls.HotKey_Research))
				mode = Player_Modes.Research;
			if (controls.IsPressed(Ctrls.HotKey_Spells))
				mode = Player_Modes.Spells;
		}

		public void Draw(SpriteBatch sb)
		{
			cursor.Draw(sb);
		}

        public ControlMapper Input
        {
            get { return controls; }
        }

        public string SelectedBuilding
        {
            get { return buildingID; }
            set { buildingID = value; }
        }

		public Player_Modes Mode
		{
			get { return mode; }
			set { mode = value; }
		}

		public Cursor Cursor
		{
			get { return cursor; }
			set { cursor = value; }
		}

    }
}
