﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.Base;
using AHH.UI.Elements;
using AHH.UI.Elements.Messages;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AHH.User;
using Microsoft.Xna.Framework;

namespace AHH.UI
{
    class UiMaster : BaseObject, IElement
    {
        Dictionary<Player_Modes, List<IElement>> elements { get; set; }
        Dictionary<ButtonFunction, string> functions { get; }
        List<ButtonFunction> action_queue { get; set; }
        InfoPanelManager infoManager;
        Messenger messenger { get; set; }
        ButtonFunction highlighted { get; set; }
        int selectedPanel = 0;
        bool isActive { get; set; }
        const int infoX = 1200;
        const int infoY = 920;
        Vector2 infoPosition = new Vector2(infoX, infoY);
        ButtonFunction prev_action;
        float uiActionTime = 10000;
        float elasped = 0;

        public UiMaster(ContentManager cm)
        {
            functions = Parsers.Parsers.Parse_InternalGridMenu(@"Content/UI/internal_ui.txt");
            elements = Parsers.Parsers.Parse_Ui_Master(@"Content/UI/ui_master.txt", cm);
            action_queue = new List<ButtonFunction>();
            isActive = true;
            messenger = new Messenger(new Vector2(1920 / 2, 864));
            infoManager = new InfoPanelManager(infoPosition, 100);
        }

        public void Update(Player p, GameTime gt)
        {
            foreach (KeyValuePair<Player_Modes, List<IElement>> kv in elements)
            {
                foreach (IElement ie in kv.Value)
                {
                    if (ie.IsActive && p.Mode == kv.Key || (kv.Key == Player_Modes.All && p.Mode != Player_Modes.MainMenu && p.Mode != Player_Modes.End_Screen))
                    {
                        ie.Update(p.Cursor);
                        if (ie is Strip)
                        {
                            string action = ((Strip)ie).GetClicked();
                            if (action != null)
                                ParseAction(action, gt);

                            string highlight = ((Strip)ie).GetHighlighted();
                            if (highlight != null)
                                ParseHighlight(highlight);
                        }

                        if (ie is Button)
                        {
                            if (((Button)ie).IsClicked)
                                ParseAction(((Button)ie).Name, gt);

                        }
                    }
                }
            }

            infoManager.Update(gt, this, p);

            if ((elasped += gt.ElapsedGameTime.Milliseconds) >= uiActionTime)
            {
                prev_action = ButtonFunction.Nan;
            }

            messenger.Update(gt);
        }

        public void RecieveInfo(KeyValuePair<Guid, InfoPanel> panel)
        {
            infoManager.Add(panel);
        }

        public void RemoveInfo(Guid id)
        {
            //infoManager.RemovePanel(id);
        }

        public void Update(Cursor ms)
        { }

        public void Draw(SpriteBatch sb, Player p)
        {
            foreach (KeyValuePair<Player_Modes, List<IElement>> kv in elements)
            {
                foreach (IElement ie in kv.Value)
                {
                    if (ie.IsActive && p.Mode == kv.Key || (kv.Key == Player_Modes.All && p.Mode != Player_Modes.MainMenu && p.Mode != Player_Modes.End_Screen))
                        ie.Draw(sb);
                }
            }

            infoManager.Draw(sb);

            messenger.Draw(sb);

        }

        public void Draw(SpriteBatch sb)
        { }

        void ParseAction(string action, GameTime gt)
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

        void ParseHighlight(string action)
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

            highlighted = func;

        }

        public void Pop_Action()
        {
            action_queue.RemoveAt(0);
        }

        public void ManipulateElements(object value, Player_Modes pm, string button = "", int index = 0)
        {
            if (elements.ContainsKey(pm))
            {
                if(elements[pm][index].GetType() == typeof(Strip))
                    ((Strip)elements[pm][index]).Manipulate<Button>(button, (string)value);
                if (elements[pm][index].GetType() == typeof(TextBox))
                    ((TextBox)elements[pm][index]).AddLine((Text)value);
            }
        }

        public ButtonFunction NextAction
        {
            get {

                if (action_queue.Count != 0)
                    return action_queue[0];
                else return ButtonFunction.Nan;

            }
        }

        public ButtonFunction Highlight
        {
            get
            {

                return highlighted;

            }
        }


        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }

        }

        public Messenger Messenger
        {
            get { return messenger; }
        }
            
    }
}
