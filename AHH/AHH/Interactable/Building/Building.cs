using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AHH.Base;
using AHH.UI;
using AHH.Extensions;
using System.Threading;
using AHH.UI.Elements;
using AHH.User;

namespace AHH.Interactable.Building
{
	class Building : InteractableMovingSprite
	{
		public static Texture2D[] statusBarTexture;

		int cost { get; set; }
		Dictionary<Corner, Vector2> corners { get; set; }
		List<Vector2> adjacentTiles { get; set; }
		OffloadThread adjThread;
        List<Point> children { get; set; } //tiles that the building encompasses

		BuildingData data;
		Type_Data<BuildingTypes> type;
        BuildingData stats { get; set; }
        BuildingStates state { get; set; }
		StatusBar statusBar;
		float temp_health;
		float temp_cost = 0;
		public Building(Vector2 position, BuildingData bd, Type_Data<BuildingTypes> bt)
			: base(position, bd.Size, bt.Texture, bt.H_texture, bt.C_texture, 0, bt.Animations)
		{
			data = bd;
			type = bt;
			corners = new Dictionary<Corner, Vector2>();
            state = BuildingStates.Building;
            this.stats = bd;
            base.CurrentState = "Building";
            CalculateCorners();
			statusBar = new StatusBar(new Point(bd.Size.X, bd.Size.Y/5), (int)stats.Health, statusBarTexture);
			this.temp_health = this.GetBuildingData().Health;
			this.GetBuildingData().Health = 1;

		}

		public Building()
			: base(new Vector2(), new Point(), null, null, null, 0)
		{ }
        protected void RefreshInfo()
        {
            Info = new InfoPanel(
                new Dictionary<Text, Text>()
                {
                    { new Text(Vector2.One, "Name: ", Color.White), new Text(Vector2.One, data.Name, Color.White) },
                    { new Text(Vector2.One, "Health: ", Color.White), new Text(Vector2.One, data.Health.ToString(), Color.White) },
                    { new Text(Vector2.One, "Descr: ", Color.White), new Text(Vector2.One, data.Descr.ToString(), Color.White) }
                }, type.Texture, Vector2.Zero);
        }

        public void InitAdjacent(Grid grid)
		{
			adjThread = new OffloadThread();
			adjThread.Th_Offload = new ThreadStart(() => {
				this.adjacentTiles = GetAdjacent(grid);
			});
			adjThread.Th_Child = new Thread(adjThread.Th_Offload);
            Console.WriteLine("Adjacent calculation Thread starting " + DateTime.Now.ToString("h:mm:ss"));
			adjThread.Th_Child.Start();
		}

		public void Update(Player player, GameTime gt, Architech arch, Grid grid, List<Rectangle> units, UiMaster ui)
		{
			base.Update(player.Cursor, gt);

			statusBar.Update(data.Health);
			statusBar.UpdatePosition(Position);

            RefreshInfo();
            if (IsHighlighted)
                ui.RecieveInfo(new KeyValuePair<Guid, InfoPanel>(this.ID, Info));
            else ui.RemoveInfo(this.ID);

            switch (state)
            {
                case BuildingStates.Building:
                    Build(gt, arch, grid, units, player, ui);
                    break;
                case BuildingStates.Disabled:
                    break;
            }


		}

		void Build(GameTime gt, Architech arch, Grid grid, List<Rectangle> units, Player player, UiMaster ui)
        { 
			bool impassable = false;

			foreach (Rectangle rect in units)
			{
				if (this.Box.Intersects(rect))
				{
					impassable = true;
					break;
				}
			}

            float tc_inc = GetBuildingData().Cost / 10;
            
            if ((Options.GetTick && !impassable && player.Energy >= tc_inc )|| temp_cost >= GetBuildingData().Cost)
			{

				if (temp_cost < GetBuildingData().Cost )
				{
					temp_cost += tc_inc;
					var pct = Extensions.Extensions.PercentAofB(temp_cost, GetBuildingData().Cost) / 100;
					GetBuildingData().Health = pct * temp_health;
					player.IncreaseEnergy -= tc_inc;
				}

				else
				{
					this.GetBuildingData().Health = temp_health;
					//finish building
					state = BuildingStates.Production; //start producing 
					arch.BuildComplete(this, grid, ui);
					base.CurrentState = "Production";
				}
            }

            else if(Options.GetTick && player.Energy <= tc_inc && !impassable)
            {
				float count = Math.Abs(player.IncreaseEnergy);
				float increase = 1 / (count == 0 ? 1 : count);
				temp_cost += player.Energy + 1/(Math.Abs(player.IncreaseEnergy) + 1);

				var pct = Extensions.Extensions.PercentAofB(temp_cost, GetBuildingData().Cost) / 100;
				GetBuildingData().Health = pct * temp_health;

				player.IncreaseEnergy -= player.Energy + 1 / (Math.Abs(player.IncreaseEnergy) + 1);
			}

        }

        public void Produce(Player p, GameTime gt, AI.Overseer os)
        {
            if (Options.GetTick)
            {

                switch (data.Type)
                {
                    case BuildingTypes.EnergyConduit:
                        p.IncreaseEnergy += (int)stats.Production;
                        break;
                    case BuildingTypes.NTower:
                        p.IncreaseEnergy += (int)stats.Production;
                        break;
                    case BuildingTypes.Wall:
                        p.IncreaseEnergy += (int)stats.Production;
                        break;
                    case BuildingTypes.Grave:
                        p.IncreaseEnergy += (int)stats.Production;
                        break;
                    case BuildingTypes.NecroticOrrery:
                        p.IncreaseEnergy += (int)stats.Production;
                        break;

                }
			}


            if (p.Energy <= 0)
                GetBuildingData().Health -= 0.1f;
            else if (p.Energy > 0 && p.IncreaseEnergy > 0)
                GetBuildingData().Health += 0.1f;


            GetBuildingData().Health = MathHelper.Clamp(GetBuildingData().Health, 0, temp_health);
        }

		public void CalculateCorners()
		{
			corners.Clear(); //clear to recalculate
			corners.Add(Corner.TopLeft, new Vector2(Position.X, Position.Y));
			corners.Add(Corner.TopRight, new Vector2(Position.X + size.X, Position.Y));
			corners.Add(Corner.BottomLeft, new Vector2(Position.X, Position.Y + size.Y));
			corners.Add(Corner.BottomRight, new Vector2(Position.X + size.X, Position.Y + size.Y));
		}


		public List<Vector2> AdjacentTiles
		{

			get { return adjacentTiles; }
		}

		public Dictionary<Corner, Vector2> Corners
		{
			get { return corners; }
		}

		public List<Vector2> GetAdjacent(Grid grid)
		{
			Extensions.Matrix_ points = new Extensions.Matrix_(new Vector2[,]
			{ {corners[Corner.TopLeft], corners[Corner.BottomLeft], },
			{ corners[Corner.TopRight], corners[Corner.BottomRight]} });

			Vector2[] vecP = new Vector2[4];
			vecP[0] = corners[Corner.TopLeft];
			vecP[1] = corners[Corner.TopRight];
			vecP[2] = corners[Corner.BottomLeft];
			vecP[3] = corners[Corner.BottomRight];
			Vector2 cp = new Vector2(Extensions.Extensions.Midpoint(corners[Corner.TopLeft].X, corners[Corner.TopRight].X),
				Extensions.Extensions.Midpoint(corners[Corner.TopRight].Y, corners[Corner.BottomRight].Y));

			Matrix_[] translations = new Matrix_[4];
			Vector2 trans = new Vector2(size.X / 2, size.Y / 2);

			translations[0] = new Matrix_(new Vector2[,]
			{{new Vector2(trans.X, 0)},
			{ new Vector2(0, trans.Y)}});

			translations[1] = new Matrix_(new Vector2[,]
			{{new Vector2(-trans.X, 0)},
			{ new Vector2(0, trans.Y )}});

			translations[2] = new Matrix_(new Vector2[,]
			{{new Vector2(trans.X, 0)},
			{ new Vector2(0, -trans.Y)}});

			translations[3] = new Matrix_(new Vector2[,]
			{{new Vector2(-trans.X, 0)},
			{ new Vector2(0, -trans.Y)}});

			Extensions.Matrix_ origin = new Matrix_(new Vector2[,] {
				{ new Vector2(trans.X + corners[Corner.TopRight].X, 0)},
				{ new Vector2(0, trans.Y + corners[Corner.TopRight].Y)}
			});

			Matrix_ IOrigin = origin.Inverse();
			
			Extensions.Matrix_ translate = new Extensions.Matrix_(new Vector2[,]
			{{new Vector2(trans.X, 0)},
			{ new Vector2(0, trans.Y)}});

			Matrix_ IT = translate.Inverse();


			Extensions.Matrix_ scaler = new Extensions.Matrix_(new Vector2[,] {
				{new Vector2(2, 0) },
				{ new Vector2(0, 2)}
			});
			Extensions.Matrix_ combination = Extensions.Matrix_.Multiply(translate, scaler);
			

			Vector2[] finalPoints = new Vector2[4];
			int I = 0;
			foreach (Matrix_ m in translations)
			{
				Extensions.Matrix_ t = Extensions.Matrix_.Minus(vecP[I].ToMatrix(), cp.ToMatrix());
				Extensions.Matrix_ c = Extensions.Matrix_.Multiply(t, scaler);
				finalPoints[I] = Matrix_.Add(c, cp.ToMatrix()).ReturnVector();
				I++;
			}

			int xpos = (int)finalPoints[1].X;
			int sizex = xpos - (int)finalPoints[0].X;
			Vector2[,] allPoints = new Vector2[sizex/(size.X/2), sizex/(size.Y/2)];
			//get vectors
			int a = 0;
			int b = 0;
			for (int x = (int)finalPoints[0].X; x < finalPoints[1].X; x += size.X/2)
			{
				for (int y = (int)finalPoints[0].Y; y < finalPoints[2].Y; y += size.Y/2)
				{
					allPoints[a, b] = new Vector2(x, y);

					b++;
				}
				a++;
				b = 0;
			}

			List<Vector2> adj = allPoints.GetEdges();

            adj = grid.CheckPositions(adj);

			return adj;


		}

        public void Draw_Debug(SpriteBatch sb)
        {
            if (adjacentTiles == null)
                return;

            foreach (Vector2 v in adjacentTiles)
            {
                sb.Draw(Texture, new Rectangle((int)v.X, (int)v.Y, 64, 64), Color.White);

            }

			sb.Draw(Texture, Box, Color.Red);
		}

		public void Draw_Status(SpriteBatch sb)
		{
			statusBar.Draw(sb, null);
		}

        public List<Point> GetChildren
        {
            get { return children; }
            set { children = value; }
        }

		public ref BuildingData GetBuildingData()
		{
			return ref data;
		}

		public BuildingStates State
		{
			get { return state; }
		}

	}
}
