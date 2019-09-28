using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.AI;
using AHH.UI;
using AHH.UI.Elements;
using AHH.UI.Elements.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace AHH.Interactable.Building
{

    enum BuildingStates
    {
        Disabled,
        Production,
        Building
    }
    enum BuildingTypes
    {
        Wall,
        NTower,
        EnergyConduit,
        NecroticOrrery,
        Grave,
        Demolish
    }
    struct BuildingData
    {
        Guid id { get; set; }
        BuildingTypes type { get; set; }
        string name { get; set; }
        float health { get; set; }
        ArmourType armour { get; set; }
        float production { get; set; }
        Point size { get; set; }
        float build_time { get; set; }
        float cost { get; set; }

        float orig_cost { get; set; }
        string descr { get; set; }
        InfoPanel info { get; set; }

        Prerequisites tier { get; set; }

        Prerequisites requiredTier { get; set; }
        public BuildingData(BuildingData bd)
        {
            this.type = bd.type;
            this.name = bd.name;
            this.health = bd.health;
            this.armour = bd.armour;
            this.production = bd.production;
            this.size = bd.size;
            this.build_time = bd.build_time;
            this.cost = bd.cost;
            this.id = Guid.NewGuid();
            this.descr = bd.descr;
            this.orig_cost = bd.orig_cost;
            this.tier = bd.tier;
            this.requiredTier = bd.requiredTier;
            Dictionary<Text, Text> items = new Dictionary<Text, Text>();

            items.Add(new Text(Vector2.Zero, ""), new Text(Vector2.Zero, this.name, Color.White));
            items.Add(new Text(Vector2.Zero, "Cost: ", Color.White), new Text(Vector2.Zero, this.cost.ToString() + " Upkeep: " + this.production.ToString(), Color.White));
            items.Add(new Text(Vector2.Zero, "Health: ", Color.White), new Text(Vector2.Zero, this.health.ToString(), Color.White));
            items.Add(new Text(Vector2.Zero, "Descr: ", Color.White), new Text(Vector2.Zero, this.descr, Color.White));

            this.info = new InfoPanel(items, null, Vector2.Zero);
                  
        }

        static public BuildingData operator +(BuildingData first, BuildingData second)
        {
            first.health += second.health;
            first.build_time += second.build_time;
            first.cost += second.cost;
            first.production += second.production;
            return first;
        }

        public static BuildingData Empty()
        {
            var b = new BuildingData();

            b.cost = 0;
            b.build_time = 0;
            b.health = 0;
            b.production = 0;

            return b;
        }

        static public BuildingData SetCost(BuildingData first, BuildingData second)
        {
            first.cost = second.cost;
            return first;
        }

        public BuildingTypes Type
        {
            get { return type; }
            set { type = value; }
        }

        public float OriginalCost
        {
            get { return orig_cost; }
            set { orig_cost = value; }

        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public float Health
        {
            get { return health; }
            set { health = value; }

        }

        public float Production
        {
            get { return production; }
            set { production = value; }
        }

        public ArmourType ArmourType
        {
            get { return armour; }
            set { armour = value; }

        }

        public Point Size
        {
            get { return size; }
            set { size = value; }
        }

        public float BuildTime
        {
            get { return build_time; }
            set { build_time = value; }
        }

        public float Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        public InfoPanel Info
        {
            get { return info; }
            set { info = value; }
        }

        public Guid Id
        {
            get { return id; }
        }

        public string Descr
        {
            get { return descr; }
            set { descr = value; }

        }

        public Prerequisites Tier
        {
            get { return tier; }
            set { tier = value; }
        }
        public Prerequisites RequiredTier
        {
            get { return requiredTier; }
            set { requiredTier = value; }
        }
    }


}
