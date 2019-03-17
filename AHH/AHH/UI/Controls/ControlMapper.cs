using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
namespace AHH.UI
{

    enum Ctrls
    {
        HotKey_BuildMode,
        HotKey_Build,
    }

    class ControlMapper
    {
        private Dictionary<Ctrls, Keys[]> controls { get; set; }

        KeyboardState kb { get; set; }
        KeyboardState kbp { get; set; }

        public ControlMapper(string filepath)
        {
            controls = new Dictionary<Ctrls, Keys[]>();
            Load(filepath);
        }

        public ControlMapper(KeyboardState kb, KeyboardState kbp, Dictionary<Ctrls, Keys[]> controls)
        {
            this.kb = kb;
            this.kbp = kbp;
            this.controls = controls;
        }

        private void Load(string filepath)
        {
            if (!File.Exists(filepath))
                return;  //skip loading


            StreamReader sr = new StreamReader(filepath);
            string line = "";

            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("#") || line == "")
                    continue;

                string[] split = line.Split('\t', ',');
                Ctrls ctrls = (Ctrls)Enum.Parse(typeof(Ctrls), split[0]);
                controls.Add(ctrls, new Keys[split.Count() - 1]);

                for (int i = 0; i < split.Count() - 1; i++)
                {
                    controls[ctrls][i] = (Keys)Enum.Parse(typeof(Keys), split[i + 1].Trim());
                }

            }

            sr.Close();
        }

        public Keys[] GetKey(Ctrls control)
        {
            return controls[control];
        }

        public bool IsPressed(Ctrls control, bool kbp_check = false)
        {
            foreach (Keys key in controls[control])
            {
                if ((kb.IsKeyDown(key) && !kbp_check) || kb.IsKeyDown(key) && kb != kbp)
                    return true;
            }


            return false;
        }

        public KeyboardState KB
        {
            get { return kb; }
            set { kb = value; }
        }

        public KeyboardState KBP
        {
            get { return kbp; }
            set { kbp = value; }
        }

        public Dictionary<Ctrls, Keys[]> Controls
        {
            get { return controls; }
        }
    }
}
