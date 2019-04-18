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
using AHH.UI.Elements;
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
		int energy { get; set; }
		
		int max_energy;
		StatusBar status_bar;
		int energyIncrease { get; set; }

		Text text;


#if DEBUG
        Vector2 rect_dimensions = new Vector2(10, 10);
#endif

        public Player(Texture2D cursor, Point size, Texture2D[] statusBar)
            : base()
        {
#if DEBUG
            buildingID = "Nan";
#endif
			mode = Player_Modes.Building;
            controls = new ControlMapper("Content/settings/controls.txt");
			this.cursor = new Cursor(cursor);
			this.energy = 2000;
			max_energy = this.energy;
			status_bar = new StatusBar(size, energy, statusBar, new Text(Vector2.One, "", Color.White));
			status_bar.UpdatePosition(new Vector2(10, 850));
			text = new Text(new Vector2(1800, 900), "", Color.White);
        }

        public void Update(UiMaster master, MouseState ms )
        {
			cursor.Update(ms);
			status_bar.Update(energy);

			if (controls.IsPressed(Ctrls.HotKey_Build))
				mode = Player_Modes.Building;
			if (controls.IsPressed(Ctrls.HotKey_Research))
				mode = Player_Modes.Research;
			if (controls.IsPressed(Ctrls.HotKey_Spells))
				mode = Player_Modes.Spells;
		}

		public void UpdateEnergy()
		{
			energy += energyIncrease;
			energy = MathHelper.Clamp(energy, 0, max_energy);
			text.Value = energyIncrease.ToString();
			energyIncrease = 0;
		}

		public void Draw(SpriteBatch sb)
		{
			cursor.Draw(sb);
			status_bar.Draw(sb, null);
			text.Draw(sb);
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

		public int Energy
		{
			get { return energy; }
			set { energy = value; }
		}

		public int IncreaseEnergy
		{
			get { return energyIncrease; }
			set { energyIncrease = value; }
		}
	}
}
