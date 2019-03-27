using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Base;
using AHH.UI.Elements;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AHH.User;
namespace AHH.UI
{
    class UiMaster : BaseObject, IElement
    {
        Dictionary<Player_Modes, List<IElement>> elements { get; set; }
        Dictionary<ButtonFunction, string> functions { get; }
		List<ButtonFunction> action_queue { get; set; }

		bool isActive { get; set; }

        public UiMaster(ContentManager cm)
        {
            functions = Parsers.Parsers.Parse_InternalGridMenu(@"Content/UI/internal_ui.txt");
			elements = Parsers.Parsers.Parse_Ui_Master(@"Content/UI/ui_master.txt", cm);
			action_queue = new List<ButtonFunction>();
			isActive = true;
        }

        public void Update(Player p)
        {
            foreach (KeyValuePair<Player_Modes, List<IElement>> kv in elements)
            {
				foreach (IElement ie in kv.Value)
				{
					if (ie.IsActive && p.Mode == kv.Key)
					{
						ie.Update(p.Cursor);
						if (ie is Strip)
						{
							string action = ((Strip)ie).GetClicked();
							if (action != null)
								ParseAction(action);
						}
					}
				}
            }
        }

		public void Update(Cursor ms)
		{ }

        public void Draw(SpriteBatch sb, Player p)
        {
			foreach (KeyValuePair<Player_Modes, List<IElement>> kv in elements)
			{
				foreach (IElement ie in kv.Value)
				{
					if (ie.IsActive && p.Mode == kv.Key)
						ie.Draw(sb);
				}
			}
        }

		public void Draw(SpriteBatch sb)
		{ }

        void ParseAction(string action)
        {
            ButtonFunction func = ButtonFunction.Examine;
            bool match = false;
            foreach (KeyValuePair<ButtonFunction, string> kv in functions)
            {
                if (kv.Value == action)
                {
                    func = kv.Key;
                    match = true;
                    break;
                }
            }

            if (!match)
                return;

			action_queue.Add(func);

        }

		public void Pop_Action()
		{
			action_queue.RemoveAt(0);
		}

		public ButtonFunction NextAction
		{
			get {

				if (action_queue.Count != 0)
					return action_queue[0];
				else return ButtonFunction.Nan;

			}
		}

		public bool IsActive
		{
			get { return isActive; }
			set { isActive = value; }

		}
    }
}
