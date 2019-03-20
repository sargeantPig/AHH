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
namespace AHH.Interactable.Building
{
	class Building : InteractableStaticSprite
	{
		
		int cost;
		Point size;
		Dictionary<Corner, Vector2> corners { get; set; }
		List<Vector2> adjacentTiles { get; set; }
		OffloadThread adjThread;

		public Building(Vector2 position, Texture2D texture, Texture2D t_highlighted, Texture2D t_clicked, Point size)
			: base(position, size, texture, t_highlighted, t_clicked)
		{
			this.size = size;
			corners = new Dictionary<Corner, Vector2>();
            CalculateCorners();
		}

		public Building(Building b)
			: base(b.Position, b.size, b.Texture, b.Texture, b.Texture)
		{
			CalculateCorners();
		}

		public Building()
			: base(new Vector2(), new Point(), null, null, null)
		{ }

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

		new public void Update(Cursor ms)
		{
			base.Update(ms);
		}

		public void CalculateCorners()
		{
			corners.Clear(); //clear to recalculate
			corners.Add(Corner.TopLeft, new Vector2(Position.X, Position.Y));
			corners.Add(Corner.TopRight, new Vector2(Position.X + size.X, Position.Y));
			corners.Add(Corner.BottomLeft, new Vector2(Position.X, Position.Y + size.Y));
			corners.Add(Corner.BottomRight, new Vector2(Position.X + size.X, Position.Y + size.Y));
		}

		new public Building DeepCopy()
		{
			InteractableStaticSprite iss = base.DeepCopy();
			Building b = (Building)this.MemberwiseClone();
			b.Box = new Rectangle((int)Position.X, (int)Position.Y, iss.Box.Width, iss.Box.Height);
			b.cost = cost;
			b.SetID = iss.ID;
			b.Position = new Vector2(Position.X, Position.Y);
			b.CalculateCorners();
			return b;

		}

		public List<Vector2> AdjacentTiles
		{
			get { return adjacentTiles; }
		}

		public Dictionary<Corner, Vector2> Corners
		{
			get { return corners; }
		}

		List<Vector2> GetAdjacent(Grid grid)
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
        }

	}
}
