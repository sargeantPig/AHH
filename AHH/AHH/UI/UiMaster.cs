using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Base;
using AHH.UI.Elements;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AHH.UI
{
    class UiMaster : BaseObject, IElement
    {
        List<IElement> elements { get; set; }
        Dictionary<ButtonFunction, string> functions { get; }


        public UiMaster(ContentManager cm)
        {
            functions = Parsers.Parsers.Parse_InternalGridMenu(@"Content/UI/internal_ui.txt");
        }

        public void Update(Cursor ms)
        {
            foreach (IElement e in elements)
            {
                e.Update(ms);
                if (e is Strip)
                {
                    string action = ((Strip)e).GetClicked();
                    if (action != null)
                        ParseAction(action);

                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (IElement e in elements)
                e.Draw(sb);
        }

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

            switch (func)
            {
                case ButtonFunction.Build:
                    PlaceBuilding(player.SelectedBuilding, player);
                    break;
                case ButtonFunction.Dismantle:
                    Dismantle();
                    break;
                case ButtonFunction.Examine:
                    Examine();
                    break;

            }

        }
    }
}
