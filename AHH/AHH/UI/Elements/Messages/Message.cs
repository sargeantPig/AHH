using AHH.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.UI.Elements.Messages
{
    class Message : BaseObject
    {
        Text text;
        Text back;
        public static float DisplayTime;
        float elasped = 0;
        bool alive { get; set; }
        float speed;
        public Message(Text text, float speed, Vector2 position) : base(position)
        {
            this.text = text;
            this.speed = speed;
            back = new Text(Vector2.Zero, text.Value, Color.Black);
            alive = true;
        }

        public void Update(GameTime gt)
        {
            if (!alive)
                return;
            elasped += gt.ElapsedGameTime.Milliseconds;
            if (elasped >= DisplayTime)
                alive = false;

            Position += new Vector2(0, -speed);
        }

        public void Draw(SpriteBatch sb)
        {
            back.Draw(sb, (Position - new Vector2(DebugFont.MeasureString(back.Value).X , 0)) + new Vector2(1, 1));
            text.Draw(sb, Position - new Vector2(DebugFont.MeasureString(back.Value).X, 0));
        }

        public bool IsAlive
        {
            get { return alive; }
        }

    }
}
