using AHH.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AHH.UI.Elements
{
    class TextBox : BaseObject, IElement
    {
        public List<Text> lines { get; set; }

        bool isActive = true;
        public TextBox(Vector2 position) : base(position)
        {
            lines = new List<Text>();
           
        }

        public bool IsActive { get { return isActive; } set { isActive = value; } }

        public void Draw(SpriteBatch sb)
        {
            int x = 0;
            foreach (var line in lines)
            {
                sb.DrawString(DebugFont, line.Value, Position + new Vector2(0, DebugFont.MeasureString(line.Value).Y * x), line.Colour);
                x++;
            }
        }

        public void Update(Cursor ms)
        {
           
        }

        public void AddLine(Text txt)
        {
            lines.Add(txt);
        }

        public String Value
        {
            get
            {
                string str = "";

                foreach (var a in lines)
                {
                    str += a + "\r\n";
                }

                return str;
            }
        }
    }
}
