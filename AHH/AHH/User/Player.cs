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
using AHH.Interactable.Building;
using AHH.Functions;
namespace AHH.User
{
    public enum Player_Modes
    {
        Building,
        Research,
        Spells,
        Tools,
        Pause,
        MainMenu,
        Tutorial,
        ES_Death,
        ES_God,
        ES_Passive,
        End_Screen,
        All
    }

    public enum Sub_Player_Modes
    {
        Demolish,
        none
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
		float energyIncrease { get; set; }

		Text text;
        float persistance;
        KeyValuePair<Text, Text> populationDisplay;
        int population { get; set; }
        int pop_cap = 40;

#if DEBUG
        Vector2 rect_dimensions = new Vector2(10, 10);
#endif
        bool pop_changed { get; set; }

        bool finished = false;
        public Player(Texture2D cursor, Point size, Texture2D[] statusBar, ContentManager cm)
            : base()
        {
#if DEBUG
            buildingID = "Nan";
#endif
            persistance = (float)(Statistics.TotalEnergyGained * 0.01);
            mode = Player_Modes.MainMenu;
            controls = new ControlMapper("Content/settings/controls.txt");
			this.cursor = new Cursor(cm.Load<Texture2D>("texture/cursor_sheet"), new Dictionary<string, Vector3>() {
                {"M_Build", new Vector3(0, 1, 500) },
                {"M_Research", new Vector3(2, 7, 100)},
                {"M_Spells", new Vector3(8, 13, 200) },
                {"M_Tools", new Vector3(14, 17, 150) },
                {"M_Pause", new Vector3(18, 26, 300) }

            });
			this.energy = 2000;
			max_energy = this.energy;
			status_bar = new StatusBar(size, energy, statusBar, new Text(Vector2.One, "", Color.White));
			status_bar.UpdatePosition(new Vector2(10, 850));
			text = new Text( status_bar.Position + status_bar.GetText.Position + new Vector2(128, -22), "", Color.White);
            status_bar.GetText.Colour = Color.DarkSlateGray;
            populationDisplay = new KeyValuePair<Text, Text>(new Text(Vector2.Zero, "Zombies: ", Color.DarkKhaki), new Text(Vector2.Zero, "", Color.White));
            pop_changed = false;
        }

        public void Update(UiMaster master, MouseState ms, Architech arch, GameTime gt )
        {
			cursor.Update(ms, gt);
			status_bar.Update(energy);
            UpdatePopulation();
            pop_changed = false;
            pop_cap = arch.GetCountByType(BuildingTypes.Grave) * 5;

            if (!arch.IsHomeAlive)
                mode = Player_Modes.End_Screen;

            if ((mode == Player_Modes.End_Screen || Statistics.Ending != Endings.Passive) && !finished)
            {
                finished = true;
                switch (Statistics.Ending)
                {
                    case Endings.Death:
                        master.ManipulateElements(new Text(Vector2.Zero, "You have destroyed all those who opposed you, death stalks the land at your bidding.", Color.Khaki), Player_Modes.End_Screen, "", 0);
                        master.ManipulateElements(new Text(Vector2.Zero, Statistics.Output, Color.AntiqueWhite), Player_Modes.End_Screen, "", 0);
                        mode = Player_Modes.End_Screen;
                        break;
                    case Endings.God:
                        master.ManipulateElements(new Text(Vector2.Zero, "Humanity kneel at your feet, you are their new prophet. Peace finally.", Color.PaleVioletRed), Player_Modes.End_Screen, "", 0);
                        master.ManipulateElements(new Text(Vector2.Zero, Statistics.Output, Color.AntiqueWhite), Player_Modes.End_Screen, "", 0);
                        mode = Player_Modes.End_Screen;
                        break;
                    case Endings.Passive:
                        master.ManipulateElements(new Text(Vector2.Zero, "Either through your own fruition or your enemies, you have been defeated.", Color.PaleVioletRed), Player_Modes.End_Screen, "", 0);
                        master.ManipulateElements(new Text(Vector2.Zero, Statistics.Output, Color.AntiqueWhite), Player_Modes.End_Screen, "", 0);
                        mode = Player_Modes.End_Screen;
                        break;
                }
            }

			if (controls.IsPressed(Ctrls.HotKey_Build))
				mode = Player_Modes.Building;
			if (controls.IsPressed(Ctrls.HotKey_Research))
				mode = Player_Modes.Research;
			if (controls.IsPressed(Ctrls.HotKey_Spells))
				mode = Player_Modes.Spells;
            if (controls.IsPressed(Ctrls.HotKey_Pause))
                mode = Player_Modes.Pause;
            switch (master.NextAction)
            {
                case ButtonFunction.M_Build:
                    master.Pop_Action();
                    mode = Player_Modes.Building;
                    break;
                case ButtonFunction.M_Research:
                    master.Pop_Action();
                    mode = Player_Modes.Research;
                    break;
                case ButtonFunction.M_Spells:
                    master.Pop_Action();
                    mode = Player_Modes.Spells;
                    break;
                case ButtonFunction.M_Tools:
                    master.Pop_Action();
                    mode = Player_Modes.Tools;
                    break;
                case ButtonFunction.M_Pause:
                    master.Pop_Action();
                    mode = Player_Modes.Pause;
                    break;
                case ButtonFunction.MM_Start:
                    master.Pop_Action();
                    mode = Player_Modes.Tutorial;
                    break;
                case ButtonFunction.MM_Hiscores:
                    master.Pop_Action();
                    break;
                case ButtonFunction.Guide:
                    master.Pop_Action();
                    mode = Player_Modes.Tutorial;
                    break;
                case ButtonFunction.ES_End:
                    master.Pop_Action();
                    mode = Player_Modes.End_Screen;
                    break;
            }

            switch (this.mode)
            {
                case Player_Modes.Building:
                    this.cursor.CurrentState = ButtonFunction.M_Build.ToString();
                    break;
                case Player_Modes.Pause:
                    this.cursor.CurrentState = ButtonFunction.M_Pause.ToString();
                    break;
                case Player_Modes.Research:
                    this.cursor.CurrentState = ButtonFunction.M_Research.ToString();
                    break;
                case Player_Modes.Tools:
                    this.cursor.CurrentState = ButtonFunction.M_Tools.ToString();
                    break;
                case Player_Modes.Spells:
                case Player_Modes.MainMenu:
                    this.cursor.CurrentState = ButtonFunction.M_Spells.ToString();
                    break;


            }

		}

        void UpdatePopulation()
        {
            populationDisplay.Value.Value = population.ToString() + "/" + pop_cap.ToString();
            if (population >= pop_cap)
                populationDisplay.Value.Colour = Color.DarkGreen;
            else if (population <= pop_cap / 2)
                populationDisplay.Value.Colour = Color.PaleVioletRed;
            else if (population <= pop_cap)
                populationDisplay.Value.Colour = Color.DarkOrange;
        }

		public void UpdateEnergy()
		{
            energyIncrease += persistance;
			energy += (int)energyIncrease;
            if (energyIncrease >= 0)
                Statistics.TotalEnergyGained += (int)energyIncrease;
            if (energyIncrease < 0)
                Statistics.TotalEnergySpent += (int)energyIncrease;
			energy = MathHelper.Clamp(energy, 0, max_energy);
			text.Value = "(" + energyIncrease.ToString() + ")";

            if (energyIncrease < 0)
                text.Colour = Color.PaleVioletRed;
            else if (energyIncrease == 0)
                text.Colour = Color.DarkOrange;
            else if (energyIncrease > 0)
                text.Colour = Color.DarkGreen;

            energyIncrease = 0;

       
		}

		public void Draw(SpriteBatch sb)
		{
			cursor.Draw(sb);

            if (mode != Player_Modes.MainMenu && mode != Player_Modes.ES_Death &&
                mode != Player_Modes.ES_God && mode != Player_Modes.ES_Passive && mode != Player_Modes.End_Screen)
            {
                status_bar.Draw(sb, null);
                text.Draw(sb);
                populationDisplay.Key.Draw(sb, new Vector2(1675, 900));
                populationDisplay.Value.Draw(sb, new Vector2(1675 + DebugFont.MeasureString(populationDisplay.Key.Value).X, 900));
            }
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

		public float IncreaseEnergy
		{
			get { return energyIncrease; }
			set { energyIncrease = value; }
		}

        public int Population
        {
            get { return population; }
            set {
                if(value != population)
                    pop_changed = true;
                population = value; }
        }

        public bool HasPopChanged
        {
            get { return pop_changed; }

        }

        public bool IsPopFull
        {
            get
            {
                if (population >= pop_cap)
                    return true;
                else return false;
       
            }
        }
	}
}
