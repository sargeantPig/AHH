using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.UI.Elements
{
    class Strip : BaseObject, IElement
    {
        Dictionary<string, IElement> elements { get; set; }

        public Strip(Vector2 position, string filepath, ContentManager cm)
            :base(position)
        {
            elements = Parsers.Parsers.Parse_UiElements(filepath, cm);
        }

        public string GetClicked()
        {
            foreach (KeyValuePair<string, IElement> kv in elements)
            {
                if (kv.Value is Button)
                {
                    if (((Button)kv.Value).IsClicked)
                    {
                        return kv.Key;
                    }
                }
            }

            return null;

        }

        public void Update(Cursor ms)
        {
            foreach (IElement e in elements.Values)
                e.Update(ms);
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (IElement e in elements.Values)
                e.Draw(sb);
        }
    }
}
