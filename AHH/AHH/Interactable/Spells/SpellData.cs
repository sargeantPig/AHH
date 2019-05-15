using AHH.UI;
using AHH.UI.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHH.Interactable.Spells
{
	enum SpellType
	{
		Ressurect,
		DrainEssence,
		RestoreEssence,
        DeadAgain,
        ClearDead
	}

    struct Spell_Stats
    {
        Guid id { get; set; }
        SpellType type { get; set; }
        string name { get; set; }
        float range { get; set; }
        float duration { get; set; }
        float cost { get; set; }
        float orig_cost { get; set; }
        Point size { get; set; }
        float speed { get; set; }
        float tick { get; set; }
        float damage { get; set; }

        string descr { get; set; }
        InfoPanel info { get; set; }
        public Spell_Stats(Spell_Stats ss)
        {
            this.type = ss.type;
            this.name = ss.name;
            this.range = ss.range;
            this.duration = ss.duration;
            this.cost = ss.cost;
            this.size = ss.size;
            this.tick = ss.tick;
            this.speed = ss.speed;
            this.damage = ss.damage;
            this.id = Guid.NewGuid();
            this.descr = ss.descr;
            this.orig_cost = ss.orig_cost;
            Dictionary<Text, Text> items = new Dictionary<Text, Text>();

            items.Add(new Text(Vector2.Zero, "", Color.White), 
                new Text(Vector2.Zero, this.name.ToString(), Color.White));
            items.Add(new Text(Vector2.Zero, "Cost per Tick: ", Color.White), 
                new Text(Vector2.Zero, this.cost.ToString() + " (" + ((this.duration/ this.tick) * cost).ToString() + " total)", Color.White));
            items.Add(new Text(Vector2.Zero, "Descr: ", Color.White), new Text(Vector2.Zero, this.descr, Color.White));
         
            this.info = new InfoPanel(items, null, Vector2.Zero);

        }

        public static Spell_Stats Empty()
        {
            var b = new Spell_Stats();

            b.cost = 0;
            b.damage = 0;
            b.duration = 0;

            return b;
        }

        static public Spell_Stats operator +(Spell_Stats first, Spell_Stats second)
        {
            first.damage += second.damage;
            first.duration += second.duration;
            first.cost += second.cost;

            return first;
        }

        static public Spell_Stats operator -(Spell_Stats first, Spell_Stats second)
        {
            first.damage -= second.damage;
            first.duration -= second.duration;
            first.cost -= second.cost;

            return first;
        }

        static public Spell_Stats SetCost(Spell_Stats first, Spell_Stats second)
        {
            first.cost = second.cost;

            return first;
        }

        public SpellType Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public float Range
        {
            get { return range; }
            set { range = value; }
        }

        public float Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        public float Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public Point Size
        {
            get { return size; }
            set { size = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public float Tick
        {
            get { return tick; }
            set { tick = value; }
        }

        public float Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public Guid ID
        {
            get { return id; }
            set { id = value; }
        }

        public InfoPanel Info
        {
            get { return info; }
            set { info = value; }
        }

        public string Descr
        {
            get { return descr; }
            set { descr = value; }
        }

        public float OriginalCost
        {
            get { return orig_cost; }
            set { orig_cost = value; }
        }

	}
}
