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
                switch (a.Key)
                {
                    case Researchables.ZHealth:
                        temp.Add(new Text(Vector2.Zero, "Increase ", Color.White), new Text(Vector2.Zero, "Zombie health" + " by " + (a.Value * 100).ToString() + "%", Color.Green));
                        break;
                    case Researchables.ZDamage:
                        temp.Add(new Text(Vector2.Zero, "Increase ", Color.White), new Text(Vector2.Zero, "Zombie damage" + " by " + (a.Value * 100).ToString() + "%", Color.Green));
                        break;
                    case Researchables.ZSpeed:
                        temp.Add(new Text(Vector2.Zero, "Increase ", Color.White), new Text(Vector2.Zero, "Zombie speed" + " by " + (a.Value * 100).ToString() + "%", Color.Green));
                        break;
                    case Researchables.WCost:
                        temp.Add(new Text(Vector2.Zero, "Decrease ", Color.White), new Text(Vector2.Zero, "Building cost" + " by " + (a.Value * 100).ToString() + "%", Color.Green));
                        break;
                    case Researchables.WHealth:
                        temp.Add(new Text(Vector2.Zero, "Increase ", Color.White), new Text(Vector2.Zero, "Building health" + " by " + (a.Value * 100).ToString() + "%", Color.Green));
                        break;
                    case Researchables.WProduct:
                        temp.Add(new Text(Vector2.Zero, "Increase ", Color.White), new Text(Vector2.Zero, "Energy production" + " by " + (a.Value * 100).ToString() + "%", Color.Green));
                        break;
                    case Researchables.SCost:
                        temp.Add(new Text(Vector2.Zero, "Decrease ", Color.White), new Text(Vector2.Zero, "Spell cost " + " by " + (a.Value * 100).ToString() + "%", Color.Green));
                        break;
                    case Researchables.SLength:
                        temp.Add(new Text(Vector2.Zero, "Increase ", Color.White), new Text(Vector2.Zero, "Spell length " + " by " + (a.Value * 100).ToString() + "%", Color.Green));
                        break;
                    case Researchables.SPower:
                        temp.Add(new Text(Vector2.Zero, "Increase ", Color.White), new Text(Vector2.Zero, "Spell damage " + " by " + (a.Value * 100).ToString() + "%", Color.Green));
                        break;
                }
                
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
