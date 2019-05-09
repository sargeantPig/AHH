using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AHH.AI;
using System.Collections;

namespace AHH.Extensions
{

	public static class Extensions
	{
		public static Vector2 DirectionTo(this Vector2 from, Vector2 to)
		{
			Vector2 direction = to - from;
			direction.Normalize();
			return direction;
		}
		public static float PercentAofB(this float a, float b)
		{
			return (a / b) * 100;
		}

        public static float PercentT(this float a, float b)
        {
            return (a * b);
        }

		public static float PercentDecrease(this float current, float original)
		{
			float decrease = original - current;
			return (decrease / original) * 100;
		}
		public static Vector2 ClosestVector(this Vector2 to, List<Vector2> vecs)
		{
			if (vecs.Count == 0)
				return Vector2.Zero;
			float distance = 9999;
			Vector2 closest = new Vector2();
			foreach (Vector2 v in vecs)
			{
				float t_dis = Vector2.Distance(to, v);
				if (t_dis < distance)
				{
					closest = v;
					distance = t_dis;
				}
			}

			return closest;
		}
		public static T GetRandom<T>(this List<T> list, Random rng)
		{
			var rnd = rng.Next(0, list.Count);
           
			return list[rnd]; 

		}
		public static List<Vector2> GetEdges(this Vector2[,] shape)
		{
			List<Vector2> edges = new List<Vector2>();

			//top
			for (int x = 0; x < shape.GetLength(0); x++)
			{
				if(edges.FindIndex(z => z == shape[x, 0]) == -1)
					edges.Add(shape[x, 0]);
			}
			//left side
			for (int y = 0; y < shape.GetLength(1); y++)
			{
				if (edges.FindIndex(z => z == shape[0, y]) == -1)
					edges.Add(shape[0, y]);
			}
			//bottom side
			for (int x = 0; x < shape.GetLength(0); x++)
			{
				if (edges.FindIndex(z => z == shape[x, shape.GetLength(1)-1]) == -1)
					edges.Add(shape[x, shape.GetLength(1) - 1]);
			}
			//right side
			for (int y = 0; y < shape.GetLength(1); y++)
			{
				if (edges.FindIndex(z => z == shape[shape.GetLength(0) - 1, y]) == -1)
					edges.Add(shape[shape.GetLength(0) - 1, y]);
			}

			return edges;
		}
		public static T RandomFlag<T>(Random rnd)
		{
			Array flags = Enum.GetValues(typeof(T));
			var a = (T)flags.GetValue(rnd.Next(flags.Length));

			return a;
		}
		public static T RandomFlag<T>(Random rnd, int min, int max)
		{
			Array flags = Enum.GetValues(typeof(T));
			var a = (T)flags.GetValue(rnd.Next(min, max));

			return a;
		}
		public static Matrix_ ToMatrix(this Vector2 vec)
		{
			Matrix_ m = new Matrix_(new Vector2[,] {
				{ new Vector2(vec.X, 0)},
				{ new Vector2(0, vec.Y)} 
			});
			return m;
		}
		public static float Midpoint(float a, float b)
		{
			return (a + b) / 2;
		}
		public static WTuple<Vector2, Point, int> GetLowestValue(this List<WTuple<Vector2, Point, int>> tuple)
		{
			var temp = new WTuple<Vector2, Point, int>(tuple.First());
			int counter = int.MaxValue;

			foreach (var tup in tuple)
			{
				if (tup.Item3 <= counter)
				{
					counter = tup.Item3;
					temp = new WTuple<Vector2, Point, int>(tup);
				}
			}

			return temp;

		}
		
        

	}

    public class Stager<t1, t3> : DicWTuple<t1, bool, t3>
    {
        public Stager(List<WTuple<t1, bool, t3>> dic)
          :  base(dic)
        {

        }

        new public void SetItem3(t1 key, t3 item3)
        {
            int index = GetIndex(key);
            if(items[index].Item2 == false)
                items[index].Item3 = item3;
        }
    }

    public class DicWTuple<t1, t2, t3>
    {
        protected List<WTuple<t1, t2, t3>> items;
        public DicWTuple(List<WTuple<t1, t2, t3>> wTuples)
        {
            items = new List<WTuple<t1, t2, t3>>(wTuples);
        }

        protected WTuple<t1, t2, t3> GetValue(t1 ind)
        {
            int index = GetIndex(ind);
            return items[index];
        }

        protected int GetIndex(t1 ind)
        {
            int i = 0;
            foreach (var tuple in items)
            {
                
                if (tuple.Item1.Equals(ind))
                    return i;
                i++;
            }

            return -1;
        }

        void Set(int index, WTuple<t1, t2, t3> value)
        {
            items[index].Item2 = value.Item2;
            items[index].Item3 = value.Item3;
        }

        public void SetItem3(t1 key, t3 item3)
        {
            int index = GetIndex(key);

            items[index].Item3 = item3;
        }

        public void SetItem2(t1 key, t2 item2)
        {
            int index = GetIndex(key);

            items[index].Item2 = item2;
        }

        public WTuple<t1, t2, t3> this[t1 ind]
        {
            get { return GetValue(ind); }

            set { Set(GetIndex(ind), value); }
        }

        public bool ContainsKey(t1 key)
        {
            int index = GetIndex(key);

            if (index <= -1)
                return false;

            return true;
        }

        public void Add(WTuple<t1, t2, t3> item)
        {
            if(!ContainsKey(item.Item1))
                items.Add(item);
        }

        public int Count
        {
            get { return items.Count; }
        }
    }


	public class WTuple<t1, t2, t3>
	{
		t1 Item_1 { get; set; }
		t2 Item_2 { get; set; }
		t3 Item_3 { get; set; }

		public WTuple(t1 Item_1, t2 Item_2, t3 Item_3)
		{
			this.Item_1 = Item_1;
			this.Item_2 = Item_2;
			this.Item_3 = Item_3;

		}
		public WTuple(WTuple<t1, t2, t3> wtuple)
		{
			this.Item_1 = wtuple.Item1;
			this.Item_2 = wtuple.Item2;
			this.Item_3 = wtuple.Item3;

		}

		public t1 Item1
		{
			get { return Item_1; }
			set { Item_1 = value; }
		}

		public t2 Item2
		{
			get { return Item_2; }
			set { Item_2 = value; }
		}

		public t3 Item3
		{
			get { return Item_3; }
			set { Item_3 = value; }
		}



	}

	public class Matrix_
	{
		Vector2[,] matrix { get; set; }
		Point order { get; }

		public Matrix_(Point size)
		{
			matrix = new Vector2[size.X, size.Y];
			order = size;
		}

		public Matrix_(Vector2[,] points)
		{
			matrix = new Vector2[points.GetLength(0), points.GetLength(1)];
			order = new Point(points.GetLength(0), points.GetLength(1));

			for (int x = 0; x < order.X; x++)
			{
				for (int y = 0; y < order.Y; y++)
				{
					matrix[x, y] = points[x, y];
				}
			}
		}

		public static Matrix_ Add(Matrix_ a, Matrix_ b)
		{
			if (a.Order != b.Order)
				return null;

			Matrix_ r = new Matrix_(a.order);

			for (int x = 0; x < a.matrix.GetLength(0); x++)
			{
				for (int y = 0; y < a.matrix.GetLength(1); y++)
				{
					r.matrix[x, y] = a.matrix[x, y] + b.matrix[x, y]; 
				}
			}

			return r;
		}

		public static Matrix_ Minus(Matrix_ a, Matrix_ b)
		{
			if (a.Order != b.Order)
				return null;

			Matrix_ r = new Matrix_(a.order);

			for (int x = 0; x < a.matrix.GetLength(0); x++)
			{
				for (int y = 0; y < a.matrix.GetLength(1); y++)
				{
					r.matrix[x, y] = a.matrix[x, y] - b.matrix[x, y];
				}
			}

			return r;
		}

		public static Matrix_ Scaler(Matrix_ a, float scaler)
		{
			for (int x = 0; x < a.matrix.GetLength(0); x++)
			{
				for (int y = 0; y < a.matrix.GetLength(1); y++)
				{
					a.matrix[x, y] *= scaler;
				}
			}

			return a;
		}

		public static Matrix_ Translate(Matrix_ a, Matrix_ translation)
		{
			if (a.order != translation.order)
				return null;

			Matrix_ r = new Matrix_(a.order);

			for (int x = 0; x < a.order.X; x++)
			{
				for (int y = 0; y < a.order.Y; y++)
				{
					r.matrix[x, y] = new Vector2(a.matrix[x, y].X + translation.matrix[x, y].X,
					a.matrix[x, y].Y + translation.matrix[x, y].Y);
				}
			}

			return r;
		}

		public static Matrix_ Multiply(Matrix_ a, Matrix_ b)
		{
			Matrix_ m = new Matrix_(new Vector2[,] {
				{new Vector2((a.matrix[0, 0].X * b.matrix[0,0].X) + (a.matrix[0,0].Y * b.matrix[1, 0].X), 
				(a.matrix[0,0].X * b.matrix[0,0].Y) + (a.matrix[0, 0].Y * b.matrix[1,0].Y))},
				{new Vector2((a.matrix[1, 0].X * b.matrix[0,0].X) + (a.matrix[1,0].Y * b.matrix[1, 0].X),
				(a.matrix[1,0].X * b.matrix[0,0].Y) + (a.matrix[1, 0].Y * b.matrix[1,0].Y))}
			});

			return m;

		}



		public Matrix_ Inverse()
		{
			//2x2
			float ad = matrix[0, 0].X * matrix[1, 0].Y;
			float bc = matrix[0, 0].Y * matrix[1, 0].X;
			float det =  1/ (ad - bc);

			//new matrix 
			Matrix_ a = new Matrix_(new Vector2[,] {
				{ new Vector2(matrix[1,0].Y, -matrix[0,0].Y)},
				{ new Vector2(-matrix[1,0].X, matrix[0,0].X)}

			});

			Matrix_ inverse = Scaler(a, det);

			return inverse;

		}

		public Vector2[,] _Matrix
		{
			get { return matrix; }
			set { matrix = value; }
		}

		public Point Order
		{
			get { return order; }
		}

		public Vector2 ReturnVector()
		{
			return new Vector2(matrix[0,0].X, matrix[1, 0].Y);
		}

	}
}
