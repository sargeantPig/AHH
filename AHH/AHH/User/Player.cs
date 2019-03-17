using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using AHH.Base;
using AHH.UI;
namespace AHH.User
{
    public enum Player_Modes
    {
        Building,
        Dismantle,
        Neutral
    }

    class Player : BaseObject
    {
        ControlMapper controls { get; }
        string buildingID { get; set; } //currently selected building to place
		Player_Modes mode { get; set; }

#if DEBUG
        Vector2 rect_dimensions = new Vector2(10, 10);
#endif

        public Player()
            : base()
        {
#if DEBUG
            buildingID = "Test";
#endif
			mode = Player_Modes.Neutral;
            controls = new ControlMapper("Content/settings/controls.txt");
        }

        public void Update()
        {
			if (controls.IsPressed(Ctrls.HotKey_BuildMode))
				mode = Player_Modes.Building;

#if DEBUG



#endif


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

    }
}
