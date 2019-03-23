using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHH.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace AHH.Interactable.Building
{

    enum BuildingTypes
    {
        Wall,
        NTower,
        EnergyConduit,
        NecroticOrrery,
        Grave
    }
    struct BuildingData
    {
        BuildingTypes type { get; set; }
        string name { get; set; }
        float health { get; set; }
        ArmourType armour { get; set; }

        float production { get; set; }

        Point size { get; set; }

        public BuildingData(BuildingData bd)
        {
            this.type = bd.type;
            this.name = bd.name;
            this.health = bd.health;
            this.armour = bd.armour;
            this.production = bd.production;
            this.size = bd.size;
        }

        public BuildingTypes Type
        {
            get { return type; }
            set { type = value; }
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
    }


}
