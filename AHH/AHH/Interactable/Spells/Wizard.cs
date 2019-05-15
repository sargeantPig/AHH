using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using AHH.Interactable.Building;
using AHH.AI;
using AHH.UI;
using Microsoft.Xna.Framework.Graphics;
using AHH.User;
using AHH.UI.Elements;
using AHH.Research;

namespace AHH.Interactable.Spells
{
	class Wizard
	{
		Dictionary<SpellType, Spell_Stats> spell_data { get; set; }
		Dictionary<SpellType, Type_Data<SpellType>> spell_types { get; set; }
		Dictionary<int, Spell> castedSpells;
		List<int> deadSpells = new List<int>();

		string selectedSpell = "";
		public Wizard(ContentManager cm)
		{
			spell_data = Parsers.Parsers.Parse_Stats<SpellType, Spell_Stats>(@"Content\spells\spell_descr.txt");
			spell_types = Parsers.Parsers.Parse_Types<SpellType, Type_Data<SpellType>>(@"Content\spells\spell_types.txt", cm);

			castedSpells = new Dictionary<int, Spell>();

            RefreshData(false);
        }

        public void RefreshData(bool statChange, Overseer os = null)
        {
            SpellType[] sts = new SpellType[] { SpellType.Ressurect, SpellType.DrainEssence, SpellType.DeadAgain, SpellType.RestoreEssence, SpellType.ClearDead };

            foreach (SpellType st in sts)
            {
                spell_data[st] = new Spell_Stats(spell_data[st]);
                spell_data[st].Info.Picture = spell_types[st].Texture;

                if (statChange && st != SpellType.ClearDead)
                {
                    var uspell = new Spell_Stats(spell_data[st]);
                    var nspell = Spell_Stats.Empty();
                    nspell.Cost = spell_data[st].OriginalCost + (float)((os.Zombies.Count * (spell_data[st].OriginalCost * 0.05)));
                    uspell = Spell_Stats.SetCost(uspell, nspell);
                    spell_data[st] = new Spell_Stats( uspell); 
                }
            }



        }


        public void Update(GameTime gt, Architech arch, Overseer os, UiMaster master, Grid grid, Player player)
		{
			foreach (KeyValuePair<int, Spell> kv in castedSpells)
			{
				kv.Value.Update(gt, arch, os, grid, player);

				if (kv.Value.State == Spell_States.Dead)
					deadSpells.Add(kv.Key);
			}

			foreach (int i in deadSpells)
				castedSpells.Remove(i);

			deadSpells.Clear();

			if (master.NextAction != ButtonFunction.Nan)
			{
				switch (master.NextAction)
				{
					case ButtonFunction.Ressurect:
						master.Pop_Action();
						selectedSpell = ButtonFunction.Ressurect.ToString();
						break;
					case ButtonFunction.RestoreEssence:
						master.Pop_Action();
						selectedSpell = ButtonFunction.RestoreEssence.ToString();
						break;
					case ButtonFunction.DrainEssence:
						master.Pop_Action();
						selectedSpell = ButtonFunction.DrainEssence.ToString();
						break;
                    case ButtonFunction.DeadAgain:
                        master.Pop_Action();
                        selectedSpell = ButtonFunction.DeadAgain.ToString();
                        break;
                    case ButtonFunction.ClearDead:
                        master.Pop_Action();
                        selectedSpell = ButtonFunction.ClearDead.ToString();
                        break;
				}

			}

            if (master.Highlight != ButtonFunction.Nan)
            {
                switch (master.Highlight)
                {
                    case ButtonFunction.DrainEssence:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(spell_data[SpellType.DrainEssence].ID,
                            spell_data[SpellType.DrainEssence].Info));
                        RemoveInfo(master, new SpellType[] { SpellType.Ressurect, SpellType.DeadAgain, SpellType.RestoreEssence});
                        break;
                    case ButtonFunction.Ressurect:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(spell_data[SpellType.Ressurect].ID,
                           spell_data[SpellType.Ressurect].Info));
                        RemoveInfo(master, new SpellType[] {SpellType.DrainEssence, SpellType.RestoreEssence, SpellType.DeadAgain });
                        break;
                    case ButtonFunction.RestoreEssence:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(spell_data[SpellType.RestoreEssence].ID,
                           spell_data[SpellType.RestoreEssence].Info));
                        RemoveInfo(master, new SpellType[] { SpellType.DrainEssence, SpellType.Ressurect, SpellType.DeadAgain });
                        break;
                    case ButtonFunction.DeadAgain:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(spell_data[SpellType.DeadAgain].ID,
                           spell_data[SpellType.DeadAgain].Info));
                        RemoveInfo(master, new SpellType[] { SpellType.DrainEssence, SpellType.Ressurect, SpellType.RestoreEssence });
                        break;
                    case ButtonFunction.ClearDead:
                        master.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(spell_data[SpellType.ClearDead].ID,
                          spell_data[SpellType.ClearDead].Info));
                        break;

                }
            }

            if (player.Mode != Player_Modes.Spells) RemoveInfo(master, new SpellType[] { SpellType.Ressurect, SpellType.DrainEssence, SpellType.DeadAgain, SpellType.RestoreEssence});

            if (player.Cursor.isLeftPressed && grid.IsHighlighted && selectedSpell != "" && (player.Mode == Player_Modes.Spells || (player.Mode == Player_Modes.Tools && selectedSpell == ButtonFunction.ClearDead.ToString())) && player.Cursor.GetState != player.Cursor.prevState)
			{
				CastSpell(grid, (SpellType)Enum.Parse(typeof(SpellType), selectedSpell), player, master);
			}

            if (player.HasPopChanged)
                RefreshData(true, os);
		}

        public void ChangeStats(float percent, List<SpellType> s_type, Researchables stat)
        {
            foreach (SpellType at in s_type)
            {
                ApplyStat(at, stat, percent);
            }
        }

        void ApplyStat(SpellType st, Researchables stat, float percent)
        {
            var temp_stat = Spell_Stats.Empty();
            switch (stat)
            {
                case Researchables.SPower:
                    var mod = Extensions.Extensions.PercentT(spell_data[st].Damage, percent);
                    temp_stat.Damage += mod;
                    spell_data[st] += temp_stat;
                    break;
                case Researchables.SCost:
                    temp_stat.Cost += Extensions.Extensions.PercentT(spell_data[st].Cost, percent);
                    spell_data[st] -= temp_stat;
                    break;
                case Researchables.SLength:
                    temp_stat.Duration += (int)Extensions.Extensions.PercentT(spell_data[st].Duration, percent);
                    spell_data[st] += temp_stat;
                    break;
            }
        }



        void RemoveInfo(UiMaster master, SpellType[] buttonFunctions)
        {
            foreach (SpellType st in buttonFunctions)
            {
                master.RemoveInfo(spell_data[st].ID);
            }

        }

        public void CastSpell(Grid grid, SpellType spell, Player p, UiMaster ui)
		{
            if ((spell == SpellType.Ressurect && !p.IsPopFull) || spell != SpellType.Ressurect)
                castedSpells.Add(Guid.NewGuid().GetHashCode(), new Spell(grid.GetTile(grid.SelectedTiles[0]).Position, spell_data[spell], spell_types[spell]));
            else ui.Messenger.AddMessage(new Text(Vector2.One, "Population Cap Reached", Color.DarkRed));
		}

		public void Draw(SpriteBatch sb)
		{
			foreach (Spell s in castedSpells.Values)
			{
				s.Draw(sb);
			}
		}
	}
}
