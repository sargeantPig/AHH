using AHH.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.UI;
using AHH.UI.Elements;
using AHH.User;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using AHH.Interactable.Building;
using AHH.Extensions;
namespace AHH.Research
{
    class Researcher
    {

        Dictionary<ButtonFunction, Dictionary<string, Research>> research;
        Dictionary<string, Research> currentResearch { get; set; }
        int[] stage;
        List<ButtonFunction> functions = new List<ButtonFunction>()
        {
            ButtonFunction.R1,
            ButtonFunction.R2,
            ButtonFunction.R3
        };
        StatusBar[] sb;

        Stager<ButtonFunction, int> stager;

        public Researcher(ContentManager cm)
        {
            research = Parsers.Parsers.ParseResearch(@"Content/research/research_tree.txt", cm);
            stage = new int[3];

            stage[0] = 0;
            stage[1] = 0;
            stage[2] = 0;

            sb = new StatusBar[3];

            stager = new Stager<ButtonFunction, int>(new List<WTuple<ButtonFunction, bool, int>>());

            stager.Add(new WTuple<ButtonFunction, bool, int>(ButtonFunction.R1, false, 0));
            stager.Add(new WTuple<ButtonFunction, bool, int>(ButtonFunction.R2, false, 0));
            stager.Add(new WTuple<ButtonFunction, bool, int>(ButtonFunction.R3, false, 0));
        }

        public void Update(GameTime gt, Overseer os, Architech arch, UiMaster master, Player player)
        {
            int i = 0;
         
            foreach (ButtonFunction bf in functions)
            {
                ResearchUpdate(gt, os, arch, master, player, i, bf);
                NextResearch(0, master, bf);
                i++;
            }

            if (master.NextAction != ButtonFunction.Nan)
            {
                switch (master.NextAction)
                {
                    case ButtonFunction.R1:
                        master.Pop_Action();
                        if (CheckResearchActivity(ButtonFunction.R1, stager[ButtonFunction.R1].Item3))
                            ChangeState(ButtonFunction.R1);
                        break;
                    case ButtonFunction.R2:
                        master.Pop_Action();
                        if (CheckResearchActivity(ButtonFunction.R2, stager[ButtonFunction.R2].Item3))
                            ChangeState(ButtonFunction.R2);
                        break;
                    case ButtonFunction.R3:
                        master.Pop_Action();
                        if (CheckResearchActivity(ButtonFunction.R3, stager[ButtonFunction.R3].Item3))
                            ChangeState(ButtonFunction.R3);
                        break;
                }
            }

            if (master.Highlight != ButtonFunction.Nan)
            {
                switch (master.Highlight)
                {
                    case ButtonFunction.R1:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(research[master.Highlight].ToList()[stager[ButtonFunction.R1].Item3].Value.ID,
                            research[master.Highlight].ToList()[stager[ButtonFunction.R1].Item3].Value.Info));
                        RemoveInfo(master, new ButtonFunction[] { ButtonFunction.R2, ButtonFunction.R3 });
                        break;
                    case ButtonFunction.R2:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(research[master.Highlight].ToList()[stager[ButtonFunction.R2].Item3].Value.ID,
                            research[master.Highlight].ToList()[stager[ButtonFunction.R2].Item3].Value.Info));
                        RemoveInfo(master, new ButtonFunction[] { ButtonFunction.R1, ButtonFunction.R3 });
                        break;
                    case ButtonFunction.R3:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(research[master.Highlight].ToList()[stager[ButtonFunction.R3].Item3].Value.ID,
                            research[master.Highlight].ToList()[stager[ButtonFunction.R3].Item3].Value.Info));
                        RemoveInfo(master, new ButtonFunction[] { ButtonFunction.R2, ButtonFunction.R1 });
                        break;
                }
            }

            if(player.Mode != Player_Modes.Research) RemoveInfo(master, new ButtonFunction[] { ButtonFunction.R1, ButtonFunction.R2, ButtonFunction.R3 });
        }

        void RemoveInfo(UiMaster master, ButtonFunction[] buttonFunctions)
        {
            foreach (ButtonFunction bf in buttonFunctions)
            {
                master.RemoveInfo(research[bf].ToList()[stager[bf].Item3].Value.ID);
            }

        }

        void ResearchUpdate(GameTime gt, Overseer os, Architech arch, UiMaster master, Player player, int ind, ButtonFunction b)
        {
            if (research[b].ToList()[stager[b].Item3].Value.State == ResearchState.Researching)
                research[b].ToList()[stager[b].Item3].Value.Update(gt, this, player, arch);
            else if (research[b].ToList()[stager[b].Item3].Value.State == ResearchState.Done && !stager[b].Item2)
            {
                ApplyResearch(research[b].ToList()[stager[b].Item3].Value.Data, os, arch);
                stager[b].Item3++;
                if (stager[b].Item3 >= research[b].ToList().Count)
                {
                    stager[b].Item2 = true;
                    stager[b].Item3 = research[b].ToList().Count - 1;
                }
                //NextResearch(ind, master, b, );
            }
        }

        void ChangeState(ButtonFunction bf)
        {
            research[bf].ToList()[stager[bf].Item3].Value.State = ResearchState.Researching;
        }

        bool CheckResearchActivity(ButtonFunction bf, int stage)
        {
            if (research[bf].ToList()[stage].Value.State == ResearchState.Waiting)
                return true;
            else return false;

        }

        void NextResearch(int ind, UiMaster master, ButtonFunction b)
        {
            if (stager[b].Item2 == true)
            { 
                master.ManipulateElements("Tree Complete", Player_Modes.Research, b.ToString(), ind);
                return;
            }

            master.ManipulateElements(research[b].ToList()[stager[b].Item3].Value.Data.Name + "\r\n" + 
                research[b].ToList()[stager[b].Item3].Value.CurrentProgress.PercentAofB(research[b].ToList()[stager[b].Item3].Value.Data.ResearchTime) + "%", Player_Modes.Research, b.ToString(), ind);
        }

        public void ApplyResearch(ResearchData rd, Overseer os, Architech arch)
        {
            List<Ai_Type> zombies = new List<Ai_Type>()
            {
                Ai_Type.Z_Archer,
                Ai_Type.Z_Horseman,
                Ai_Type.Z_Knight,
                Ai_Type.Z_Priest
            };
            List<Ai_Type> ais = new List<Ai_Type>()
            {
                Ai_Type.Archer,
                Ai_Type.Horseman,
                Ai_Type.Knight,
                Ai_Type.Priest
            };
            List<BuildingTypes> buildings = new List<BuildingTypes>()
            {
                BuildingTypes.EnergyConduit,
                BuildingTypes.Grave,
                BuildingTypes.Wall
            };

            foreach (KeyValuePair<Researchables, float> rf in rd.Modifiers)
            {
                if (rf.Key == Researchables.ZSpeed)
                    os.ChangeStats(rf.Value, zombies, rf.Key);
                else if (rf.Key == Researchables.ZHealth)
                    os.ChangeStats(rf.Value, zombies, rf.Key);
                else if (rf.Key == Researchables.ZDamage)
                    os.ChangeStats(rf.Value, zombies, rf.Key);
                else if (rf.Key == Researchables.WCost)
                    arch.ChangeStats(rf.Value, buildings, rf.Key);
                else if (rf.Key == Researchables.WHealth)
                    arch.ChangeStats(rf.Value, buildings, rf.Key);
                else if (rf.Key == Researchables.WProduct)
                    arch.ChangeStats(rf.Value, buildings, rf.Key);

            }

        }

		public void ResearchComplete(Research rs, Overseer os)
		{
			currentResearch.Remove(rs.Data.Name);
		}

	}
}
