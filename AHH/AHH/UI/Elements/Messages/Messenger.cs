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
    class Messenger : BaseObject
    {
        List<Message> messages { get; set; }
        int messageSpeed = 1;
        public Messenger(Vector2 position) : base(position)
        {
            
            messages = new List<Message>();
        }

        public void Update(GameTime gt)
        {
            for (int x = 0; x < messages.Count; x++)
            {
                messages[x].Update(gt);
                if (!messages[x].IsAlive)
                    messages.RemoveAt(x);
            }
        }

        public void AddMessage(Text text)
        {
            messages.Add(new Message(text, messageSpeed, Position));
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var message in Messages)
            {
                message.Draw(sb);
            }
        }

        public List<Message> Messages
        {
            get { return messages; }
            set { messages = value; }
        }
    }
}
