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

        public ArmourType Armour
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
    struct Building_Type
    {
        BuildingTypes type { get; set; }
        Texture2D texture { get; set; }
        Texture2D h_texture { get; set; }
        Texture2D c_texture { get; set; }
        Dictionary<string, Vector3> animations { get; set; }

        public Building_Type(Building_Type ut)
        {
            this.type = ut.Type;
            this.texture = ut.texture;
            this.h_texture = ut.h_texture;
            this.c_texture = ut.c_texture;
            this.animations = ut.animations;
        }

        public BuildingTypes Type
        {
            get { return type; }
            set { type = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Texture2D H_texture
        {
            get { return h_texture; }
            set { h_texture = value; }
        }
        public Texture2D C_texture
        {
            get { return c_texture; }
            set { c_texture = value; }
        }

        public Dictionary<string, Vector3> Animations
        {
            get { return animations; }
            set { animations = value; }
        }
    }
}
