using AHH.User;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.UI.Elements;
using AHH.UI;
using AHH.Base;
using AHH.Interactable.Building;

namespace AHH.Research
{
	class Research : BaseObject
	{
		ResearchState state { get; set; }
		ResearchData data { get; set; }
		float currentProgress { get; set; }

		public Research(ResearchData rd)
		{
            data = new ResearchData(rd);
			state = ResearchState.Waiting;
            currentProgress = 0;
            Dictionary<Text, Text> temp = new Dictionary<Text, Text>();
            temp.Add(new Text(Vector2.Zero, "", Color.White), new Text(Vector2.Zero, data.Name, Color.White));

            foreach (var a in rd.Modifiers)
            {
                temp.Add(new Text(Vector2.Zero, "Increase ", Color.White), new Text(Vector2.Zero, a.Key.ToString() + " by " + (a.Value * 100).ToString() + "%", Color.Green));
            }
            

            Info = new InfoPanel(temp, rd.Texture, Vector2.Zero);


		}

		public void Update(GameTime gt, Researcher rs, Player p, Architech arch)
		{
			if (state == ResearchState.Done)
				return;
            if (state == ResearchState.Researching)
                Produce(gt, p, rs, arch);
		}

        void Produce(GameTime gt, Player p, Researcher rs, Architech arch)
        {
            if (Options.GetTick && currentProgress < data.ResearchTime && p.Energy > arch.GetOrrery())
            {
                p.IncreaseEnergy -= arch.GetOrrery() * 2;
                currentProgress += arch.GetOrrery() * 2;
            }

            else if (currentProgress >= data.ResearchTime)
            {
                state = ResearchState.Done;
            }
        }

		public ResearchData Data
		{
			get { return this.data; }
		}

        public ResearchState State
        {
            get { return state; }
            set { state = value; }
        }

        public float CurrentProgress
        {
            get { return currentProgress; }
        }

        public void Start()
        {
            state = ResearchState.Researching;
        }

	}
}
