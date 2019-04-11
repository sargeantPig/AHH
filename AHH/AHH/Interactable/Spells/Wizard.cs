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

namespace AHH.Interactable.Spells
{
	class Wizard
	{
		Dictionary<SpellType, List<Spell_Stats>> spell_data { get; set; }
		Dictionary<SpellType, Type_Data<SpellType>> spell_types { get; set; }
		Dictionary<int, Spell> castedSpells;
		List<int> deadSpells = new List<int>();

		string selectedSpell = "";
		public Wizard(ContentManager cm)
		{
			spell_data = Parsers.Parsers.Parse_Stats<SpellType, Spell_Stats>(@"Content\spells\spell_descr.txt");
			spell_types = Parsers.Parsers.Parse_Types<SpellType, Type_Data<SpellType>>(@"Content\spells\spell_types.txt", cm);

			castedSpells = new Dictionary<int, Spell>();
		}

		public void Update(GameTime gt, Architech arch, Overseer os, UiMaster master, Grid grid, Player player)
		{
			foreach (KeyValuePair<int, Spell> kv in castedSpells)
			{
				kv.Value.Update(gt, arch, os, grid);

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
				}

			}

			if (player.Cursor.isLeftPressed && grid.IsHighlighted && selectedSpell != "" && player.Mode == Player_Modes.Spells && player.Cursor.GetState != player.Cursor.prevState )
			{
				CastSpell(grid, (SpellType)Enum.Parse(typeof(SpellType), selectedSpell));
			}
			
		}

		public void CastSpell(Grid grid, SpellType spell)
		{
			castedSpells.Add(Guid.NewGuid().GetHashCode(), new Spell(grid.GetTile(grid.SelectedTiles[0]).Position, spell_data[spell][0], spell_types[spell]));

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
