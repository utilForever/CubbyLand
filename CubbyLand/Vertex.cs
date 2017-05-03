using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubbyLand
{
    public class Vertex : ICoord
    {
        public static Vertex VERTEX_AT_INFINITY = new Vertex(Double.NaN, Double.NaN);
        private static Stack<Vertex> pool = new Stack<Vertex>();

        private int numV = 0;
        private Point coord;
        private int index;

        private static Vertex Create(double x, double y)
        {
            if (Double.IsNaN(x) || Double.IsNaN(y))
            {
                return VERTEX_AT_INFINITY;
            }

            if (pool.Count > 0)
            {
                return pool.Pop().Initialize(x, y);
            }
            else
            {
                return new Vertex(x, y);
            }
        }

        public Vertex(double x, double y)
        {
            Initialize(x, y);
        }

        private Vertex Initialize(double x, double y)
        {
            coord = new Point(x, y);
            return this;
        }

        public void Dispose()
        {
            coord = null;
            pool.Push(this);
        }

        public Point GetCoord()
        {
            return coord;
        }

        public int GetIndex()
        {
            return index;
        }

        public void SetIndex()
        {
            index = numV++;
        }

        public static Vertex Intersect(HalfEdge hE0, HalfEdge hE1)
        {
            Edge e0;
            Edge e1;
            Edge e;
            HalfEdge hE;
            double determinant;
            double intersectionX;
            double intersectionY;
            bool rightOfSite;


            e0 = hE0.e;
            e1 = hE1.e;
            if (e0 == null || e1 == null)
                return null;
            if (e0.GetRightSite() == e1.GetRightSite())
                return null;

            determinant = (e0.a * e1.b) - (e0.b * e1.a);
            if (determinant > -0.0000000001 && determinant < 0.0000000001)
                return null;

            intersectionX = ((e0.c * e1.b) - (e1.c * e0.b)) / determinant;
            intersectionY = ((e1.c * e0.a) - (e0.c * e1.a)) / determinant;

            if (Voronoi.CompareByYThenX(e0.GetRightSite(), e1.GetRightSite()) < 0)
            {
                hE = hE0;
                e = e0;
            }
            else
            {
                hE = hE1;
                e = e1;
            }

            rightOfSite = intersectionX >= e.GetRightSite().coord.x;
            if ((rightOfSite && (hE.orientation == Orientation.LEFT)) || (!rightOfSite && (hE.orientation == Orientation.RIGHT)))
                return null;

            return Vertex.Create(intersectionX, intersectionY);
        }
    }
}
