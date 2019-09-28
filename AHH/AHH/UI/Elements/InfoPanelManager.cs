using AHH.Base;
using AHH.UI.Elements.Buttons;
using AHH.User;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.UI.Elements
{
    class InfoPanelManager :BaseObject
    {
        Dictionary<Guid, InfoPanel> infoPanels = new Dictionary<Guid, InfoPanel>();
        Dictionary<Guid, float> timers = new Dictionary<Guid, float>();
        float maxTime;
        List<Guid> remove_queue = new List<Guid>(); 
        int selectedPanel { get; set; }

        public InfoPanelManager(Vector2 position, float maxTime) : base(position)
        {
            this.maxTime = maxTime;
            selectedPanel = 0;
        }

        public void Update(GameTime gt, UiMaster master, Player p)
        {
            List<Guid> temp = timers.Keys.ToList();
            foreach(var id in temp)
            {
                timers[id]+= gt.ElapsedGameTime.Milliseconds;

                if (timers[id] >= maxTime)
                    remove_queue.Add(id);
            }

            foreach (var id in remove_queue)
                RemovePanel(id);

            remove_queue.Clear();

            if (p.Input.IsPressed(Ctrls.HotKey_Cycle_Forward, true))
                selectedPanel += 1;
            if (p.Input.IsPressed(Ctrls.HotKey_Cycle_Backward, true))
                selectedPanel -= 1;

            switch (master.NextAction)
            {
                case ButtonFunction.C_Forward:
                    selectedPanel += 1;
                    master.Pop_Action();
                    break;
                case ButtonFunction.C_Backward:
                    master.Pop_Action();
                    selectedPanel -= 1;
                    break;
            }


        }

        public void RemovePanel(Guid id)
        {
            infoPanels.Remove(id);
            timers.Remove(id);
        }

        public void Add(KeyValuePair<Guid, InfoPanel> panel)
        {
            if (!infoPanels.ContainsKey(panel.Key))
            {
                infoPanels.Add(panel.Key, panel.Value);
                timers.Add(panel.Key, 0);
            }
            else if (timers.ContainsKey(panel.Key))
                timers[panel.Key] = 0;

            infoPanels[panel.Key].Position = new Vector2(Position.X, Position.Y);
            infoPanels[panel.Key].Refresh();

           

        }

        public int ChangeSelectedPanel
        {
            set { selectedPanel = value; }
        }

        public void Draw(SpriteBatch sb)
        {
            if (infoPanels.Count > 0)
            {
                selectedPanel = MathHelper.Clamp(selectedPanel, 0, infoPanels.Count - 1);
                infoPanels.ToList()[selectedPanel].Value.Draw(sb);
                sb.DrawString(DebugFont, (selectedPanel + 1).ToString() + " / " + infoPanels.Count.ToString(), new Vector2(1110, 948), Color.White);
            }
        }

    }
}
